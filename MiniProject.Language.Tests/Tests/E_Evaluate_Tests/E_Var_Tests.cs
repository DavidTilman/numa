using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Var: rho(x) = l, sigma(l) = v
public class E_Var_Tests
{
    [Fact]
    public void Bound_variable_evaluates_to_its_value()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));

        Assert.Equal(new V_Nat(1), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Reads_the_current_value_in_the_store()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        env.GetRef("x").Value = new V_Nat(2);

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Inner_binding_shadows_outer_binding()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1)).Bind("x", new V_Nat(2));

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Undefined_variable_throws()
    {
        Assert.Throws<Exception>(() => new E_Var("x").Evaluate(TestEnv.Empty));
    }
}
