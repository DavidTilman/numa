using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// S-While-F, S-While-T, S-While-Ret
public class S_While_Tests
{
    // x := x + 1
    private static readonly Stmt Increment =
        new S_Assign("x", new E_Op(new E_Var("x"), new OP_Plus(), new E_Lit(1UL)));

    // x < n
    private static Expr XLessThan(ulong n) => new E_Op(new E_Var("x"), new OP_LT(), new E_Lit(n));

    [Fact]
    public void False_condition_does_not_run_the_body()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var loop = new S_While(new E_Lit(false), new B_Block([Increment]));

        var (outcome, _) = loop.Execute(env);

        Assert.IsType<O_Normal>(outcome);
        Assert.Equal(new V_Nat(0), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Loops_until_the_condition_is_false()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var loop = new S_While(XLessThan(3), new B_Block([Increment]));

        var (outcome, _) = loop.Execute(env);

        Assert.IsType<O_Normal>(outcome);
        Assert.Equal(new V_Nat(3), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Return_in_the_body_stops_the_loop()
    {
        // While (x < 10) { x := x + 1; Ret x; }
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var loop = new S_While(XLessThan(10), new B_Block([Increment, new S_Return(new E_Var("x"))]));

        var (outcome, _) = loop.Execute(env);

        Assert.Equal(new O_Ret(new V_Nat(1)), outcome);
        Assert.Equal(new V_Nat(1), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Returns_the_same_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));

        var (_, after) = new S_While(XLessThan(3), new B_Block([Increment])).Execute(env);

        Assert.Same(env, after);
    }

    [Fact]
    public void Variables_declared_in_the_body_do_not_leak()
    {
        // While (x < 1) { Var y = 1; x := x + 1; }
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var body = new B_Block([new S_Decl("y", new T_Nat(), new E_Lit(1UL)), Increment]);

        var (_, after) = new S_While(XLessThan(1), body).Execute(env);

        Assert.Throws<Exception>(() => new E_Var("y").Evaluate(after));
    }

    [Fact]
    public void Non_boolean_condition_throws()
    {
        var loop = new S_While(new E_Lit(1UL), new B_Block([]));

        Assert.Throws<Exception>(() => loop.Execute(TestEnv.Empty));
    }
}
