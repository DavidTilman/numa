using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// S-Ret: evaluate e, result ret v
public class S_Return_Tests
{
    [Fact]
    public void Returns_the_value_of_the_expression()
    {
        var (outcome, _) = new S_Return(new E_Lit(5UL)).Execute(TestEnv.Empty);

        Assert.Equal(new O_Ret(new V_Nat(5)), outcome);
    }

    [Fact]
    public void Expression_is_evaluated_in_the_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(3));

        var (outcome, _) = new S_Return(new E_Var("x")).Execute(env);

        Assert.Equal(new O_Ret(new V_Nat(3)), outcome);
    }

    [Fact]
    public void Returns_the_same_environment()
    {
        Env env = TestEnv.Empty;

        var (_, after) = new S_Return(new E_Lit(5UL)).Execute(env);

        Assert.Same(env, after);
    }
}
