using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Op: evaluate both operands, then apply the operator
public class E_Op_Tests
{
    private static Value Eval(object left, Operation op, object right)
        => new E_Op(new E_Lit(left), op, new E_Lit(right)).Evaluate(TestEnv.Empty);

    [Fact]
    public void Plus_nat() => Assert.Equal(new V_Nat(5), Eval(2UL, new OP_Plus(), 3UL));

    [Fact]
    public void Plus_real() => Assert.Equal(new V_Real(2.5), Eval(1.5, new OP_Plus(), 1.0));

    [Fact]
    public void Sub_int_can_go_negative() => Assert.Equal(new V_Int(-3), Eval(2L, new OP_Sub(), 5L));

    [Fact]
    public void Mult_nat() => Assert.Equal(new V_Nat(6), Eval(2UL, new OP_Mult(), 3UL));

    [Fact]
    public void Div_nat_truncates() => Assert.Equal(new V_Nat(3), Eval(7UL, new OP_Div(), 2UL));

    [Fact]
    public void Mod_nat() => Assert.Equal(new V_Nat(1), Eval(7UL, new OP_Mod(), 2UL));

    [Fact]
    public void LT_true() => Assert.Equal(new V_Bool(true), Eval(2UL, new OP_LT(), 3UL));

    [Fact]
    public void LT_false() => Assert.Equal(new V_Bool(false), Eval(3UL, new OP_LT(), 2UL));

    [Fact]
    public void LEQ_equal() => Assert.Equal(new V_Bool(true), Eval(3UL, new OP_LEQ(), 3UL));

    [Fact]
    public void EQ_true() => Assert.Equal(new V_Bool(true), Eval(3UL, new OP_EQ(), 3UL));

    [Fact]
    public void NEQ_true() => Assert.Equal(new V_Bool(true), Eval(2UL, new OP_NEQ(), 3UL));

    [Fact]
    public void GET_equal() => Assert.Equal(new V_Bool(true), Eval(3UL, new OP_GET(), 3UL));

    [Fact]
    public void GT_true() => Assert.Equal(new V_Bool(true), Eval(3UL, new OP_GT(), 2UL));

    [Fact]
    public void AND() => Assert.Equal(new V_Bool(false), Eval(true, new OP_AND(), false));

    [Fact]
    public void OR() => Assert.Equal(new V_Bool(true), Eval(true, new OP_OR(), false));

    [Fact]
    public void Operands_are_evaluated_in_the_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(2));
        var expr = new E_Op(new E_Var("x"), new OP_Plus(), new E_Lit(3UL));

        Assert.Equal(new V_Nat(5), expr.Evaluate(env));
    }
}
