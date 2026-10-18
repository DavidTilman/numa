using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Cond-T, E-Cond-F
public class E_Cond_Tests
{
    [Fact]
    public void True_condition_takes_first_branch()
    {
        var expr = new E_Cond(new E_Lit(true), new E_Lit(1UL), new E_Lit(2UL));

        Assert.Equal(new V_Nat(1), expr.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void False_condition_takes_second_branch()
    {
        var expr = new E_Cond(new E_Lit(false), new E_Lit(1UL), new E_Lit(2UL));

        Assert.Equal(new V_Nat(2), expr.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Other_branch_is_not_evaluated()
    {
        // The untaken branch refers to an undefined variable, which would throw if it were run.
        var expr = new E_Cond(new E_Lit(true), new E_Lit(1UL), new E_Var("undefined"));

        Assert.Equal(new V_Nat(1), expr.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Non_boolean_condition_throws()
    {
        var expr = new E_Cond(new E_Lit(1UL), new E_Lit(1UL), new E_Lit(2UL));

        Assert.Throws<Exception>(() => expr.Evaluate(TestEnv.Empty));
    }
}
