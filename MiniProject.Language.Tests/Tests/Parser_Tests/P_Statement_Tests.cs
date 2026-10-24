using MiniProject.Language.Structure;
using static MiniProject.Language.Tests.PT;

namespace MiniProject.Language.Tests;

// Statement grammar: Var decl / Read, assignment, While, If/Else, Ret, Print,
// and blocks. Each statement inside a block is terminated by a semicolon.
public class P_Statement_Tests
{
    // ---- Var declaration -----------------------------------------------------
    [Fact]
    public void Var_declaration()
        => Assert.Equal(new S_Decl("x", TNat, new E_Lit((ulong)1)),
                        ParseS(Var_, Id("x"), Colon, NatT, Eq_, Num("1")));

    [Fact]
    public void Var_declaration_keeps_the_annotated_type()
        => Assert.Equal(new S_Decl("x", TReal, new E_Lit((ulong)1)),
                        ParseS(Var_, Id("x"), Colon, RealT, Eq_, Num("1")));

    [Fact]
    public void Var_declaration_with_a_compound_initialiser()
        => Assert.Equal(new S_Decl("x", TNat, EOp(ENat(1), new OP_Plus(), ENat(2))),
                        ParseS(Var_, Id("x"), Colon, NatT, Eq_, Num("1"), Plus, Num("2")));

    // ---- Read ---------------------------------------------------------------
    [Fact]
    public void Read_statement()
        => Assert.Equal(new S_Read("x", TInt), ParseS(Var_, Id("x"), Colon, IntT, Eq_, Read_));

    // ---- Assignment ---------------------------------------------------------
    [Fact]
    public void Assignment()
        => Assert.Equal(new S_Assign("x", new E_Lit((ulong)5)), ParseS(Id("x"), Assign, Num("5")));

    [Fact]
    public void Assignment_rhs_is_a_full_expression()
        => Assert.Equal(new S_Assign("x", EOp(EVar("x"), new OP_Plus(), ENat(1))),
                        ParseS(Id("x"), Assign, Id("x"), Plus, Num("1")));

    // ---- Return / Print ------------------------------------------------------
    [Fact]
    public void Return_statement()
        => Assert.Equal(new S_Return(new E_Lit((ulong)1)), ParseS(Ret_, Num("1")));

    [Fact]
    public void Print_statement()
        => Assert.Equal(new S_Print(new E_Lit((ulong)1)), ParseS(Print_, Num("1")));

    // ---- While --------------------------------------------------------------
    [Fact]
    public void While_statement()
    {
        S_While w = Assert.IsType<S_While>(
            ParseS(While_, LP, True_, RP, LB, Print_, Num("1"), Semi, RB));
        Assert.Equal(new E_Lit(true), w.Cond);
        Assert.Equal(new S_Print(new E_Lit((ulong)1)), Assert.Single(w.Body.Statements));
    }

    [Fact]
    public void While_condition_is_a_full_expression()
    {
        S_While w = Assert.IsType<S_While>(
            ParseS(While_, LP, Id("n"), Gt, Num("0"), RP, LB, RB));
        Assert.Equal(EOp(EVar("n"), new OP_GT(), ENat(0)), w.Cond);
    }

    // ---- If / Else ----------------------------------------------------------
    [Fact]
    public void If_with_else()
    {
        S_Cond c = Assert.IsType<S_Cond>(
            ParseS(If_, LP, True_, RP, LB, Ret_, Num("1"), Semi, RB, Else_, LB, Ret_, Num("2"), Semi, RB));
        Assert.Equal(new E_Lit(true), c.Cond);
        Assert.Equal(new S_Return(new E_Lit((ulong)1)), Assert.Single(c.OnTrue.Statements));
        Assert.Equal(new S_Return(new E_Lit((ulong)2)), Assert.Single(c.OnFalse.Statements));
    }

    [Fact]
    public void If_without_else_has_an_empty_else_block()
    {
        S_Cond c = Assert.IsType<S_Cond>(
            ParseS(If_, LP, True_, RP, LB, Ret_, Num("1"), Semi, RB));
        Assert.Equal(new S_Return(new E_Lit((ulong)1)), Assert.Single(c.OnTrue.Statements));
        Assert.Empty(c.OnFalse.Statements);
    }

    // ---- blocks -------------------------------------------------------------
    [Fact]
    public void Empty_block()
    {
        S_While w = Assert.IsType<S_While>(ParseS(While_, LP, False_, RP, LB, RB));
        Assert.Empty(w.Body.Statements);
    }

    [Fact]
    public void Block_with_several_statements()
    {
        S_While w = Assert.IsType<S_While>(
            ParseS(While_, LP, True_, RP, LB, Print_, Num("1"), Semi, Print_, Num("2"), Semi, RB));
        Assert.Equal(2, w.Body.Statements.Count);
        Assert.Equal(new S_Print(new E_Lit((ulong)1)), w.Body.Statements[0]);
        Assert.Equal(new S_Print(new E_Lit((ulong)2)), w.Body.Statements[1]);
    }

    [Fact]
    public void Nested_blocks()
    {
        // While (True) { If (False) { Print 1; } }
        S_While w = Assert.IsType<S_While>(
            ParseS(While_, LP, True_, RP, LB, If_, LP, False_, RP, LB, Print_, Num("1"), Semi, RB, Semi, RB));
        S_Cond inner = Assert.IsType<S_Cond>(Assert.Single(w.Body.Statements));
        Assert.Equal(new S_Print(new E_Lit((ulong)1)), Assert.Single(inner.OnTrue.Statements));
    }
}
