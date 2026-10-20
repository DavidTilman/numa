using System.Reflection.Metadata.Ecma335;

namespace MiniProject.Language.Structure;

public abstract record Expr
{
    public abstract Value Evaluate(Env Env);
}
public sealed record E_Lit(object Value) : Expr
{
    public override Value Evaluate(Env Env) => Value switch
    {
        ulong ul => new V_Nat(ul),
        long l   => new V_Int(l),
        double d => new V_Real(d),
        bool b   => new V_Bool(b),
        _ => throw new Exception("Invalid Literal")
    };
}

public sealed record E_UnaryNegate(Expr e) : Expr
{
    // E-Neg: negating a Nat gives an Int, since the result can be below zero.
    public override Value Evaluate(Env Env) => e.Evaluate(Env) switch
    {
        V_Nat n  => new V_Int(-checked((long)n.Value)),
        V_Int i  => new V_Int(checked(-i.Value)),
        V_Real r => new V_Real(-r.Value),
        _ => throw new Exception("Invalid operation.")
    };
}
public sealed record E_Var(string Name) : Expr
{
    public override Value Evaluate(Env Env)
    {
        return Env.GetRef(Name).Value;
    }
}
public sealed record E_Op(Expr E1, Operation Op, Expr E2) : Expr
{
    public override Value Evaluate(Env Env)
    {
        return Op.Evaluate(E1.Evaluate(Env), E2.Evaluate(Env));
    }
}
public sealed record E_Field(Expr Target, string Field) : Expr
{
    public override Value Evaluate(Env Env)
    {
        if (Target.Evaluate(Env) is not V_Record rec)
            throw new Exception("Attempted field access on a non-record value");
        
        IReadOnlyList<Param> rec_params = Env.GetRecord(rec.Name);
        
        int index = -1;
        for (int i = 0; i < rec_params.Count; i++) 
        {
            if (rec_params[i].Name == Field) { index = i; break; }
        }
        if (index == -1) throw new Exception($"{rec.Name} has no field {Field}");
        
        return rec.Fields[index];
    }
}
public sealed record E_Appl(Expr Target, IReadOnlyList<Expr> Args) : Expr
{
    public override Value Evaluate(Env Env)
    {
        Value val = Target.Evaluate(Env);
        if (val is not V_Closure clo) throw new Exception("Attempted application on non-closure.");
        Value[] values = new Value[Args.Count];
        for(int i = 0; i < Args.Count; i++) 
        {
            values[i] = Args[i].Evaluate(Env);
        }
        if (values.Length != clo.Params.Count) throw new Exception("Arguments do not match function parameters."); 
        Env callEnv = clo.Env;
        for (int i = 0; i < values.Length; i++)
            callEnv = callEnv.Bind(clo.Params[i].Name, values[i]);
        Outcome outcome = clo.Body.Execute(callEnv);
        if (outcome is not O_Ret ret) throw new Exception("Function did not return a value.");
        return ret.Value;
    }
}
public sealed record E_RecCon(string Target, IReadOnlyList<Expr> Args) : Expr
{
    public override Value Evaluate(Env Env)
    {
        IReadOnlyList<Param> recordParams = Env.GetRecord(Target);
        if (Args.Count != recordParams.Count) throw new Exception("Record construction does not match definition.");
        IReadOnlyList<Value> values = Args.Select(x => x.Evaluate(Env)).ToList(); 
        return new V_Record(Target, values);
    }
}

public sealed record E_Lambda(IReadOnlyList<Param> Params, Type ReturnType, Expr Body) : Expr
{
    public override Value Evaluate(Env Env) => new V_Closure(Params, new B_Block([new S_Return(Body)]), Env);
}
public sealed record E_Cond(Expr Cond, Expr OnTrue, Expr OnFalse) : Expr
{
    public override Value Evaluate(Env Env)
    {
        if (Cond.Evaluate(Env) is not V_Bool cond) throw new Exception("Expression did not evaluate to a boolean.");
        return cond.Value ? OnTrue.Evaluate(Env) : OnFalse.Evaluate(Env);
    }
}