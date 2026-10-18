using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Num, E-True, E-False
public class E_Lit_Tests
{
    private static Value Eval(object literal) => new E_Lit(literal).Evaluate(TestEnv.Empty);

    [Fact]
    public void Nat_literal_evaluates_to_nat() => Assert.Equal(new V_Nat(5), Eval(5UL));

    [Fact]
    public void Int_literal_evaluates_to_int() => Assert.Equal(new V_Int(-3), Eval(-3L));

    [Fact]
    public void Real_literal_evaluates_to_real() => Assert.Equal(new V_Real(2.5), Eval(2.5));

    [Fact]
    public void True_evaluates_to_true() => Assert.Equal(new V_Bool(true), Eval(true));

    [Fact]
    public void False_evaluates_to_false() => Assert.Equal(new V_Bool(false), Eval(false));

    [Fact]
    public void Unsupported_literal_throws() => Assert.Throws<Exception>(() => Eval("hello"));
}
