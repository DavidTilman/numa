using System.Collections.Immutable;
using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using Type = MiniProject.Language.Structure.Type;
namespace MiniProject.Language.TypeChecking;

public sealed record Context(ImmutableDictionary<string, Type> Vars) {
    public Type Lookup(string name)
        => Vars.TryGetValue(name, out Type? t) 
            ? t 
            : throw new UnBoundVarException(name);

    public Context Bind(string name, Type type)
        => this with { Vars = Vars.SetItem(name, type) };
}

public class TypeChecker {
    static Type? Neg(Type t) => t switch {
        T_Nat  => new T_Int(),
        T_Int  => new T_Int(),
        T_Real => new T_Real(),
        _      => null
    };      

    static Type? Widest(Type? t1, Type? t2) => (t1, t2) switch {
        (T_Nat, T_Nat) => new T_Nat(),  
        (T_Nat, T_Int) => new T_Int(),  
        (T_Nat, T_Real) => new T_Real(),
        (T_Int, T_Nat) => new T_Int(),  
        (T_Int, T_Int) => new T_Int(),  
        (T_Int, T_Real) => new T_Real(),
        (T_Real, T_Nat) => new T_Real(),  
        (T_Real, T_Int) => new T_Real(),  
        (T_Real, T_Real) => new T_Real(),  
        _ => null
    };

    static Type Sig(Operation op, Type t1, Type t2) => (op, t1, t2) switch {
        (OP_Plus o, Type ot1, Type ot2) 
            => Widest(ot1, ot2) 
            ?? throw new OperationNotDefinedException(o, ot1, ot2),
        (OP_Sub o, Type ot1, Type ot2) 
            => Widest(Widest(ot1, ot2), new T_Int()) 
            ?? throw new OperationNotDefinedException(o, ot1, ot2),
        (OP_Mult o, Type ot1, Type ot2) 
            =>  Widest(ot1, ot2) 
            ?? throw new OperationNotDefinedException(o, ot1, ot2),
        (OP_Div o, Type ot1, Type ot2) 
            =>  Widest(ot1, ot2) 
            ?? throw new OperationNotDefinedException(o, ot1, ot2),
        (OP_Mod o, Type ot1, Type ot2)
            => Widest(ot1, ot2) 
            ?? throw new OperationNotDefinedException(o, ot1, ot2),
        (OP_LT or OP_LEQ or 
         OP_GT or OP_GEQ or 
         OP_EQ or OP_NEQ, 
         Type a, Type b) when Widest(a, b) is not null 
            => new T_Bool(),
        (OP_EQ, T_Record r1, T_Record r2) 
            => r1.Name == r2.Name
            ? new T_Bool() 
            : throw new OperationNotDefinedException(op, r1, r2),
        (OP_EQ, T_Bool, T_Bool) => new T_Bool(),
        (OP_NEQ, T_Record r1, T_Record r2) 
            => r1.Name == r2.Name 
            ? new T_Bool() 
            : throw new OperationNotDefinedException(op, r1, r2),
        (OP_NEQ, T_Bool, T_Bool) 
            => new T_Bool(),
        (OP_AND or OP_OR, T_Bool, T_Bool) 
            => new T_Bool(),
        _ => throw new OperationNotDefinedException(op, t1, t2),
    };

    static bool Assignable(Type from, Type to)
        => SameType(from, to) 
        || (Widest(from, to) is Type w 
        && SameType(w, to));
    private static bool FuncsAssignable(T_Func f1, T_Func f2)
        => f1.ParamTypes.Count == f2.ParamTypes.Count
        && f1.ParamTypes.Zip(f2.ParamTypes).All(p 
            => SameType(p.First, p.Second))
        && SameType(f1.ReturnType, f2.ReturnType);

    private static bool SameType(Type a, Type b)
        => (a, b) switch {
            (T_Func f1, T_Func f2) 
                => FuncsAssignable(f1, f2),
            (T_Record r1, T_Record r2) 
                => r1.Name == r2.Name,
            _ => a.GetType() == b.GetType(), 
        };


    private readonly Dictionary<string, IReadOnlyList<Param>> delta = []; 
    
    public void Check(P_Prog p) {
        foreach(D_Record r in p.Records) {
            delta.Add(r.Name, r.Params);
        }

        Dictionary<string, Type> vars = [];
        foreach (D_Func f in p.Funcs) {
            vars.Add(f.Name, new T_Func(f.Params.Select(x => x.Type).ToList(), f.ReturnType));
        }

        CheckProgram(p, new Context(vars.ToImmutableDictionary()));
    } 

    Type CheckExpr(Expr e, Context g) => e switch {
            E_Lit l => l.Value switch {
                ulong  => new T_Nat(),
                long   => new T_Int(),
                double => new T_Real(),
                bool   => new T_Bool(),
                _ => throw new InternalException($"unknown literal type")},
            E_Var v         => g.Lookup(v.Name),
            E_UnaryNegate n 
                            => Neg(CheckExpr(n.e, g)) 
                                ?? throw new UndefinedUnaryNegationException(n.e),
            E_Op o          => Sig(o.Op, CheckExpr(o.E1, g), CheckExpr(o.E2, g)), 
            E_Field f       => FieldType(f, g),
            E_Appl a        => ApplicationType(a, g),
            E_RecCon rc     => RecordType(rc, g),
            E_Lambda l      => LambdaType(l, g),
            E_Cond c        => CondType(c, g),
            _ => throw new InternalException("Unhandled expression.")
    };

    private Type CondType(E_Cond c, Context g) {
        Type t = CheckExpr(c.Cond, g);
        if (t is not T_Bool)
            throw new UnexpectedTypeException(new T_Bool(), t);
        Type t1 = CheckExpr(c.OnTrue, g);
        Type t2 = CheckExpr(c.OnFalse, g);
        return SameType(t1,t2) 
               ? t1 
               : (Widest(t1,t2) 
                    ?? throw new TypeMismatchException(t1, t2));
    }

    private T_Func LambdaType(E_Lambda l, Context g) {
        foreach (Param p in l.Params)
            g = g.Bind(p.Name, p.Type);
        
        Type body = CheckExpr(l.Body, g);
        
        if (!Assignable(body, l.ReturnType))
            throw new TypeMismatchException(body, l.ReturnType);
        
        return new T_Func([.. l.Params.Select(x => x.Type)], l.ReturnType);
    }

    private T_Record RecordType(E_RecCon rc, Context g)
    {
        if (!delta.TryGetValue(rc.Target, out var params_))
            throw new UndefinedTypeException(rc.Target);
        
        if (rc.Args.Count != params_.Count) 
            throw new InvalidCallException(params_.Count, rc.Args.Count);
        
        for (int i = 0; i < rc.Args.Count; i++) {
            Type t = CheckExpr(rc.Args[i], g);
            if (!Assignable(t, params_[i].Type))
                throw new TypeMismatchException(t, params_[i].Type);
        }

        return new T_Record(rc.Target);
    }

    private Type ApplicationType(E_Appl a, Context g) {
        if (CheckExpr(a.Target, g) is not T_Func f)
            throw new NotWellTypedException(a.Target);
        
        if (a.Args.Count != f.ParamTypes.Count)
            throw new InvalidCallException(f.ParamTypes.Count, a.Args.Count);
        
        for(int i = 0; i < a.Args.Count; i++) {
            Type t = CheckExpr(a.Args[i], g);
            if (!Assignable(t, f.ParamTypes[i]))
                throw new TypeMismatchException(t, f.ParamTypes[i]);
        }

        return f.ReturnType;
    }

    private Type FieldType(E_Field f, Context g) {
        if (CheckExpr(f.Target, g) is not T_Record r)
            throw new NotWellTypedException(f);

        if (!delta.TryGetValue(r.Name, out var fields))
            throw new UndefinedTypeException(r.Name);
        
        return 
            fields.FirstOrDefault(p => p.Name == f.Field)?.Type
            ?? throw new NotWellTypedException(f);
    }

    Context CheckStmt(Stmt s, Context g, Type? ret) => s switch {
        S_Decl d    => g.Bind(d.Variable, CheckExpr(d.Expr, g)),
        S_Read r    => g.Bind(r.Variable, r.Type),
        S_Assign a  => CheckAssignment(a, g),
        S_While w   => CheckWhile(w, g, ret),
        S_Cond c    => CheckCond(c, g, ret),
        S_Return rt => CheckReturn(rt, g, ret),
        S_Print p   => CheckPrint(p, g),
        _ => throw new NotWellTypedException(s)
    };

    private Context CheckReturn(S_Return rt, Context g, Type? ret) {
        if (ret is null)            
            throw new NotWellTypedException(rt);

        Type t = CheckExpr(rt.Expr, g);
        
        if (!Assignable(t, ret))
            throw new UnexpectedTypeException(ret, t);
        
        return g;
    }

    private Context CheckPrint(S_Print p, Context g) {
        CheckExpr(p.Expr, g);
        return g;
    }

    private Context CheckCond(S_Cond c, Context g, Type? ret) {
        Type t = CheckExpr(c.Cond, g);
        if (t is not T_Bool)
            throw new UnexpectedTypeException(new T_Bool(), t);
        
        CheckBlock(c.OnTrue.Statements, g, ret);
        CheckBlock(c.OnFalse.Statements, g, ret);
        
        return g;
    }

    private Context CheckWhile(S_While w, Context g, Type? ret) {
        Type t = CheckExpr(w.Cond, g);
        if (t is not T_Bool)
            throw new UnexpectedTypeException(new T_Bool(), t);
        
        CheckBlock(w.Body.Statements, g, ret);
        
        return g;
    }

    private Context CheckAssignment(S_Assign a, Context g) {
        Type t = g.Lookup(a.Variable);
        Type te = CheckExpr(a.Expr, g);
        
        if (!Assignable(te, t))
            throw new UnexpectedTypeException(t, te);
        
        return g;
    }

    void CheckBlock(IReadOnlyList<Stmt> stmts, Context g, Type? ret) {
        foreach (Stmt s in stmts)
            g = CheckStmt(s, g, ret);
    }

    void CheckProgram(P_Prog p, Context globals) {
        foreach (D_Func f in p.Funcs) {
            Context funcContext = globals;
            
            foreach(Param pm in f.Params) 
                funcContext = funcContext.Bind(pm.Name, pm.Type);
            
            CheckBlock(f.Body.Statements, funcContext, f.ReturnType);
        }

        CheckBlock([.. p.Stmts], globals, null);
    }
}