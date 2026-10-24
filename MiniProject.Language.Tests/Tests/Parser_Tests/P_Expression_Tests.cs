using MiniProject.Language.Structure;
using static MiniProject.Language.Tests.PT;

namespace MiniProject.Language.Tests;

// Expression grammar:
//   e ? e1 : e2                                  (loosest)
//   || && == != < <= > >= + - * / %              binary, levels 1-6
//   -e                                           unary
//   e(args)  e.f                                 postfix
//   literals, x, (e), New R(..), Lambda          (tightest)
public class P_Expression_Tests
{
    // ---- literals & variables ----------------------------------------------
    [Fact] public void Nat_literal() => Assert.Equal(new E_Lit((ulong)42), ParseE(Num("42")));
    [Fact] public void Nat_literal_boxes_a_ulong() => Assert.IsType<ulong>(((E_Lit)ParseE(Num("42"))).Value);
    [Fact] public void Real_literal() => Assert.Equal(new E_Lit(3.5), ParseE(RealLit("3.5")));
    [Fact] public void True_literal() => Assert.Equal(new E_Lit(true), ParseE(True_));
    [Fact] public void False_literal() => Assert.Equal(new E_Lit(false), ParseE(False_));
    [Fact] public void Variable() => Assert.Equal(new E_Var("x"), ParseE(Id("x")));

    // ---- binary precedence --------------------------------------------------
    [Fact]
    public void Mult_binds_tighter_than_plus_on_the_right()
        => Assert.Equal(EOp(ENat(1), new OP_Plus(), EOp(ENat(2), new OP_Mult(), ENat(3))),
                        ParseE(Num("1"), Plus, Num("2"), Mult, Num("3")));

    [Fact]
    public void Mult_binds_tighter_than_plus_on_the_left()
        => Assert.Equal(EOp(EOp(ENat(1), new OP_Mult(), ENat(2)), new OP_Plus(), ENat(3)),
                        ParseE(Num("1"), Mult, Num("2"), Plus, Num("3")));

    [Fact]
    public void Comparison_binds_looser_than_arithmetic()
        => Assert.Equal(EOp(EOp(ENat(1), new OP_Plus(), ENat(2)), new OP_LT(), ENat(3)),
                        ParseE(Num("1"), Plus, Num("2"), Lt, Num("3")));

    [Fact]
    public void Equality_binds_looser_than_comparison()
        => Assert.Equal(EOp(EOp(EVar("a"), new OP_LT(), EVar("b")), new OP_EQ(), EVar("c")),
                        ParseE(Id("a"), Lt, Id("b"), EqOp, Id("c")));

    [Fact]
    public void And_binds_tighter_than_or()
        => Assert.Equal(EOp(EVar("a"), new OP_OR(), EOp(EVar("b"), new OP_AND(), EVar("c"))),
                        ParseE(Id("a"), Or, Id("b"), And, Id("c")));

    [Fact]
    public void And_binds_looser_than_comparison()
        => Assert.Equal(EOp(EOp(EVar("a"), new OP_LT(), EVar("b")), new OP_AND(), EVar("c")),
                        ParseE(Id("a"), Lt, Id("b"), And, Id("c")));

    // ---- binary associativity (left) ----------------------------------------
    [Fact]
    public void Subtraction_is_left_associative()
        => Assert.Equal(EOp(EOp(ENat(1), new OP_Sub(), ENat(2)), new OP_Sub(), ENat(3)),
                        ParseE(Num("1"), Sub, Num("2"), Sub, Num("3")));

    [Fact]
    public void Division_is_left_associative()
        => Assert.Equal(EOp(EOp(ENat(8), new OP_Div(), ENat(4)), new OP_Div(), ENat(2)),
                        ParseE(Num("8"), Div, Num("4"), Div, Num("2")));

    // ---- grouping -----------------------------------------------------------
    [Fact]
    public void Parentheses_override_precedence()
        => Assert.Equal(EOp(EOp(ENat(1), new OP_Plus(), ENat(2)), new OP_Mult(), ENat(3)),
                        ParseE(LP, Num("1"), Plus, Num("2"), RP, Mult, Num("3")));

    // ---- unary negation ------------------------------------------------------
    [Fact]
    public void Unary_negation_binds_tighter_than_binary()
        => Assert.Equal(EOp(ENeg(ENat(2)), new OP_Mult(), ENat(3)),
                        ParseE(Sub, Num("2"), Mult, Num("3")));

    [Fact]
    public void Unary_negation_stacks()
        => Assert.Equal(ENeg(ENeg(ENat(1))), ParseE(Sub, Sub, Num("1")));

    [Fact]
    public void Unary_negation_applies_to_postfix_not_the_whole_term()
        => Assert.Equal(ENeg(EField(EVar("a"), "b")), ParseE(Sub, Id("a"), Dot, Id("b")));

    // ---- ternary ------------------------------------------------------------
    [Fact]
    public void Ternary_condition_is_a_binary_expression()
        => Assert.Equal(ECond(EOp(EVar("a"), new OP_Plus(), EVar("b")), ENat(1), ENat(2)),
                        ParseE(Id("a"), Plus, Id("b"), Q, Num("1"), Colon, Num("2")));

    [Fact]
    public void Ternary_else_branch_nests_to_the_right()
        => Assert.Equal(ECond(EVar("a"), ENat(1), ECond(EVar("b"), ENat(2), ENat(3))),
                        ParseE(Id("a"), Q, Num("1"), Colon, Id("b"), Q, Num("2"), Colon, Num("3")));

    // ---- postfix: application ------------------------------------------------
    [Fact]
    public void Call_with_no_args()
    {
        E_Appl a = Assert.IsType<E_Appl>(ParseE(Id("f"), LP, RP));
        Assert.Equal(new E_Var("f"), a.Target);
        Assert.Empty(a.Args);
    }

    [Fact]
    public void Call_with_args()
    {
        E_Appl a = Assert.IsType<E_Appl>(ParseE(Id("f"), LP, Num("1"), Comma, Num("2"), RP));
        Assert.Equal(new E_Var("f"), a.Target);
        Assert.Equal([new E_Lit((ulong)1), new E_Lit((ulong)2)], a.Args);
    }

    [Fact]
    public void Calls_chain_left_to_right()
    {
        // f()()
        E_Appl outer = Assert.IsType<E_Appl>(ParseE(Id("f"), LP, RP, LP, RP));
        Assert.Empty(outer.Args);
        E_Appl inner = Assert.IsType<E_Appl>(outer.Target);
        Assert.Equal(new E_Var("f"), inner.Target);
    }

    // ---- postfix: field access ----------------------------------------------
    [Fact]
    public void Field_access() => Assert.Equal(EField(EVar("r"), "x"), ParseE(Id("r"), Dot, Id("x")));

    [Fact]
    public void Field_access_chains()
        => Assert.Equal(EField(EField(EVar("r"), "x"), "y"), ParseE(Id("r"), Dot, Id("x"), Dot, Id("y")));

    [Fact]
    public void Field_then_call()
    {
        // r.f(1)
        E_Appl a = Assert.IsType<E_Appl>(ParseE(Id("r"), Dot, Id("f"), LP, Num("1"), RP));
        Assert.Equal(EField(EVar("r"), "f"), a.Target);
        Assert.Equal([new E_Lit((ulong)1)], a.Args);
    }

    [Fact]
    public void Call_then_field()
    {
        // f(1).x
        E_Field fld = Assert.IsType<E_Field>(ParseE(Id("f"), LP, Num("1"), RP, Dot, Id("x")));
        Assert.Equal("x", fld.Field);
        E_Appl a = Assert.IsType<E_Appl>(fld.Target);
        Assert.Equal(new E_Var("f"), a.Target);
    }

    // ---- record construction -------------------------------------------------
    [Fact]
    public void Record_construction()
    {
        // New Pt(1, 2)
        E_RecCon rc = Assert.IsType<E_RecCon>(ParseE(New_, Id("Pt"), LP, Num("1"), Comma, Num("2"), RP));
        Assert.Equal("Pt", rc.Target);
        Assert.Equal([new E_Lit((ulong)1), new E_Lit((ulong)2)], rc.Args);
    }

    [Fact]
    public void Record_construction_no_args()
    {
        E_RecCon rc = Assert.IsType<E_RecCon>(ParseE(New_, Id("Unit"), LP, RP));
        Assert.Equal("Unit", rc.Target);
        Assert.Empty(rc.Args);
    }

    // ---- lambda --------------------------------------------------------------
    [Fact]
    public void Lambda_with_one_param()
    {
        // Lambda (x : Nat) : Int => x
        E_Lambda l = Assert.IsType<E_Lambda>(
            ParseE(Lambda_, LP, Id("x"), Colon, NatT, RP, Colon, IntT, FatArrow, Id("x")));
        Assert.Equal([new Param("x", TNat)], l.Params);
        Assert.Equal(TInt, l.ReturnType);
        Assert.Equal(new E_Var("x"), l.Body);
    }

    [Fact]
    public void Lambda_with_no_params()
    {
        E_Lambda l = Assert.IsType<E_Lambda>(ParseE(Lambda_, LP, RP, Colon, NatT, FatArrow, Num("1")));
        Assert.Empty(l.Params);
        Assert.Equal(TNat, l.ReturnType);
        Assert.Equal(new E_Lit((ulong)1), l.Body);
    }

    [Fact]
    public void Lambda_body_extends_as_far_as_possible()
    {
        // Lambda (x : Nat) : Nat => x + 1   — body is the whole `x + 1`
        E_Lambda l = Assert.IsType<E_Lambda>(
            ParseE(Lambda_, LP, Id("x"), Colon, NatT, RP, Colon, NatT, FatArrow, Id("x"), Plus, Num("1")));
        Assert.Equal(EOp(EVar("x"), new OP_Plus(), ENat(1)), l.Body);
    }
}
