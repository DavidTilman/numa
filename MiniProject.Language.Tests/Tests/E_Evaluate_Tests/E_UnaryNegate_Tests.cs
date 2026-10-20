using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Neg: evaluate e, then apply unary minus
public class E_UnaryNegate_Tests
{
    private static Value Eval(object literal) => new E_UnaryNegate(new E_Lit(literal)).Evaluate(TestEnv.Empty);

    [Fact]
    public void Negating_a_nat_gives_an_int() => Assert.Equal(new V_Int(-5), Eval(5UL));

    [Fact]
    public void Negating_an_int() => Assert.Equal(new V_Int(3), Eval(-3L));

    [Fact]
    public void Negating_a_real() => Assert.Equal(new V_Real(-2.5), Eval(2.5));

    [Fact]
    public void Double_negation()
    {
        var expr = new E_UnaryNegate(new E_UnaryNegate(new E_Lit(5UL)));

        Assert.Equal(new V_Int(5), expr.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Operand_is_evaluated_in_the_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Int(4));

        Assert.Equal(new V_Int(-4), new E_UnaryNegate(new E_Var("x")).Evaluate(env));
    }

    [Fact]
    public void Negating_a_bool_throws() => Assert.Throws<Exception>(() => Eval(true));

    [Fact]
    public void Nat_too_large_for_an_int_throws() => Assert.Throws<OverflowException>(() => Eval(ulong.MaxValue));
}
