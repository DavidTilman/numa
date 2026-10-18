using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// S-If-T, S-If-F
public class S_Cond_Tests
{
    private static B_Block Returns(ulong n) => new([new S_Return(new E_Lit(n))]);

    [Fact]
    public void True_condition_runs_the_first_block()
    {
        var (outcome, _) = new S_Cond(new E_Lit(true), Returns(1), Returns(2)).Execute(TestEnv.Empty);

        Assert.Equal(new O_Ret(new V_Nat(1)), outcome);
    }

    [Fact]
    public void False_condition_runs_the_second_block()
    {
        var (outcome, _) = new S_Cond(new E_Lit(false), Returns(1), Returns(2)).Execute(TestEnv.Empty);

        Assert.Equal(new O_Ret(new V_Nat(2)), outcome);
    }

    [Fact]
    public void Normal_outcome_is_passed_on()
    {
        var (outcome, _) = new S_Cond(new E_Lit(true), new B_Block([]), new B_Block([])).Execute(TestEnv.Empty);

        Assert.IsType<O_Normal>(outcome);
    }

    [Fact]
    public void Other_block_is_not_run()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var other = new B_Block([new S_Assign("x", new E_Lit(1UL))]);

        new S_Cond(new E_Lit(true), new B_Block([]), other).Execute(env);

        Assert.Equal(new V_Nat(0), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Variables_declared_in_a_block_do_not_leak()
    {
        var block = new B_Block([new S_Decl("y", new T_Nat(), new E_Lit(1UL))]);

        var (_, after) = new S_Cond(new E_Lit(true), block, new B_Block([])).Execute(TestEnv.Empty);

        Assert.Throws<Exception>(() => new E_Var("y").Evaluate(after));
    }

    [Fact]
    public void Non_boolean_condition_throws()
    {
        var stmt = new S_Cond(new E_Lit(1UL), new B_Block([]), new B_Block([]));

        Assert.Throws<Exception>(() => stmt.Execute(TestEnv.Empty));
    }
}
