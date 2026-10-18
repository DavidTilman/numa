using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// S-Assign: evaluate e, rho(x) = l, result normal with the same rho and sigma[l -> v]
public class S_Assign_Tests
{
    [Fact]
    public void Updates_the_variable()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));

        var (outcome, after) = new S_Assign("x", new E_Lit(2UL)).Execute(env);

        Assert.IsType<O_Normal>(outcome);
        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(after));
    }

    [Fact]
    public void Returns_the_same_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));

        var (_, after) = new S_Assign("x", new E_Lit(2UL)).Execute(env);

        Assert.Same(env, after);
    }

    [Fact]
    public void Right_hand_side_sees_the_old_value()
    {
        // x := x + 1
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        var stmt = new S_Assign("x", new E_Op(new E_Var("x"), new OP_Plus(), new E_Lit(1UL)));

        stmt.Execute(env);

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Updates_the_existing_location()
    {
        // Another environment sharing x's location sees the change.
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        Env other = env.Bind("y", new V_Nat(0));

        new S_Assign("x", new E_Lit(2UL)).Execute(env);

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(other));
    }

    [Fact]
    public void Closure_sees_assignment_to_captured_variable()
    {
        // Var x = 1; f = Lambda () => x; x := 2; f() gives 2
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        env = env.Bind("f", new E_Lambda([], new T_Nat(), new E_Var("x")).Evaluate(env));

        new S_Assign("x", new E_Lit(2UL)).Execute(env);

        Assert.Equal(new V_Nat(2), new E_Appl(new E_Var("f"), []).Evaluate(env));
    }

    [Fact]
    public void Undefined_variable_throws()
    {
        Assert.Throws<Exception>(() => new S_Assign("x", new E_Lit(1UL)).Execute(TestEnv.Empty));
    }
}
