using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using MiniProject.Language.TypeChecking;
using Type = MiniProject.Language.Structure.Type;

namespace MiniProject.Language.Tests;

// Shared builders for the type-checker tests. The checker only exposes
// TypeChecker(P_Prog).Check(), so every case is phrased as a whole program and
// asserted by whether Check() throws.
internal static class TC
{
    // ---- running the checker ------------------------------------------------
    public static void Check(params Com[] commands) => new TypeChecker(new P_Prog(commands)).Check();

    // Check() completes without a type error.
    public static void Ok(params Com[] commands) => Check(commands);

    // Check() rejects the program with some type error.
    public static void Rejected(params Com[] commands) => Assert.ThrowsAny<TypeException>(() => Check(commands));

    // Check() rejects the program with a specific type error.
    public static void Rejected<T>(params Com[] commands) where T : TypeException
        => Assert.Throws<T>(() => Check(commands));

    // ---- types --------------------------------------------------------------
    public static readonly Type TNat = new T_Nat();
    public static readonly Type TInt = new T_Int();
    public static readonly Type TReal = new T_Real();
    public static readonly Type TBool = new T_Bool();
    public static Type TRec(string name) => new T_Record(name);
    public static Type TFunc(Type[] ps, Type ret) => new T_Func(ps.ToList(), ret);

    // ---- literals (the CLR type of the boxed value fixes its MiniLang type) --
    public static Expr Nat(ulong n) => new E_Lit(n);
    public static Expr Int(long n) => new E_Lit(n);
    public static Expr Real(double n) => new E_Lit(n);
    public static Expr Bool(bool b) => new E_Lit(b);

    // ---- expressions --------------------------------------------------------
    public static Expr Var(string x) => new E_Var(x);
    public static Expr Op(Expr l, Operation op, Expr r) => new E_Op(l, op, r);
    public static Expr Neg(Expr e) => new E_UnaryNegate(e);
    public static Expr Field(Expr target, string field) => new E_Field(target, field);
    public static Expr RecCon(string name, params Expr[] args) => new E_RecCon(name, args);
    public static Expr Call(string fn, params Expr[] args) => new E_Appl(new E_Var(fn), args);
    public static Expr CallExpr(Expr target, params Expr[] args) => new E_Appl(target, args);
    public static Expr Cond(Expr c, Expr t, Expr f) => new E_Cond(c, t, f);
    public static Expr Lambda(Param[] ps, Type ret, Expr body) => new E_Lambda(ps, ret, body);

    // ---- statements ---------------------------------------------------------
    public static Stmt Decl(string v, Type t, Expr e) => new S_Decl(v, t, e);
    public static Stmt Read(string v, Type t) => new S_Read(v, t);
    public static Stmt Assign(string v, Expr e) => new S_Assign(v, e);
    public static Stmt Ret(Expr e) => new S_Return(e);
    public static Stmt PrintS(Expr e) => new S_Print(e);
    public static Stmt While(Expr cond, params Stmt[] body) => new S_While(cond, new B_Block(body));
    public static Stmt If(Expr cond, Stmt[] onTrue, Stmt[] onFalse)
        => new S_Cond(cond, new B_Block(onTrue), new B_Block(onFalse));

    // ---- commands / declarations --------------------------------------------
    public static Param P(string name, Type t) => new Param(name, t);
    public static Com Do(Stmt s) => new C_Stmt(s);
    public static Com Fn(string name, Param[] ps, Type ret, params Stmt[] body)
        => new C_Decl(new D_Func(name, ps, ret, new B_Block(body)));
    public static Com Rec(string name, params Param[] fields)
        => new C_Decl(new D_Record(name, fields));

    // ---- common program shapes ---------------------------------------------
    // A program whose only effect is to type-check `e` (Print checks its operand).
    public static Com[] Prints(Expr e) => [Do(PrintS(e))];

    // Func F(ps) : ret = { Ret body; }  — type-checks iff body is assignable to ret.
    // Lets a test pin an expression's type by the return type that accepts it.
    public static Com[] Returns(Type ret, Expr body, params Param[] ps)
        => [Fn("F", ps, ret, Ret(body))];
}
