using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// S-Var: evaluate e, bind x to a fresh location holding v, result normal with rho[x -> l]
public class S_Decl_Tests
{
    [Fact]
    public void Binds_variable_in_the_returned_environment()
    {
        var (outcome, env) = new S_Decl("x", new T_Nat(), new E_Lit(1UL)).Execute(TestEnv.Empty);

        Assert.IsType<O_Normal>(outcome);
        Assert.Equal(new V_Nat(1), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Does_not_change_the_input_environment()
    {
        Env before = TestEnv.Empty;
        new S_Decl("x", new T_Nat(), new E_Lit(1UL)).Execute(before);

        Assert.Throws<Exception>(() => new E_Var("x").Evaluate(before));
    }

    [Fact]
    public void Expression_is_evaluated_in_the_input_environment()
    {
        Env env = TestEnv.Empty.Bind("y", new V_Nat(4));

        var (_, after) = new S_Decl("x", new T_Nat(), new E_Var("y")).Execute(env);

        Assert.Equal(new V_Nat(4), new E_Var("x").Evaluate(after));
    }

    [Fact]
    public void Redeclaring_uses_a_fresh_location()
    {
        Env outer = TestEnv.Empty.Bind("x", new V_Nat(1));

        var (_, inner) = new S_Decl("x", new T_Nat(), new E_Lit(2UL)).Execute(outer);

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(inner));
        Assert.Equal(new V_Nat(1), new E_Var("x").Evaluate(outer));
        Assert.NotSame(outer.GetRef("x"), inner.GetRef("x"));
    }
}
