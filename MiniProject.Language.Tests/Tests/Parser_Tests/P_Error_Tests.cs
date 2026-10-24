using MiniProject.Language.Exceptions;
using static MiniProject.Language.Tests.PT;

namespace MiniProject.Language.Tests;

// Malformed token streams must be rejected with a ParseException.
public class P_Error_Tests
{
    // ---- missing terminators / delimiters -----------------------------------
    [Fact] public void Missing_top_level_semicolon() => Rejects<UnexpectedTokenException>(Print_, Num("1"));
    [Fact] public void Unclosed_grouping() => Rejects(Print_, LP, Num("1"), Semi);
    [Fact] public void Unclosed_call_args() => Rejects(Print_, Id("f"), LP, Num("1"), Semi);
    [Fact] public void Ternary_missing_colon() => Rejects(Print_, Id("a"), Q, Id("b"), Semi);

    // ---- statements ---------------------------------------------------------
    [Fact] public void Bare_expression_is_not_a_statement() => Rejects<UnexpectedStartException>(Num("1"), Semi);
    [Fact] public void Empty_print_operand() => Rejects<UnexpectedStartException>(Print_, Semi);
    [Fact] public void Assignment_requires_walrus_not_equals() => Rejects(Id("x"), Eq_, Num("1"), Semi);
    [Fact] public void Field_target_cannot_be_assigned() => Rejects(Id("r"), Dot, Id("x"), Assign, Num("1"), Semi);
    [Fact] public void While_body_must_be_a_block() => Rejects(While_, LP, True_, RP, Print_, Num("1"), Semi);

    // ---- types --------------------------------------------------------------
    [Fact] public void Missing_type_after_colon() => Rejects<InvalidParseException>(Var_, Id("x"), Colon, Eq_, Num("1"), Semi);

    // ---- declarations -------------------------------------------------------
    [Fact] public void Record_requires_a_name() => Rejects(Record_, LP, RP, Semi);
    [Fact]
    public void Function_requires_equals_before_body()
        => Rejects(Func_, Id("F"), LP, RP, Colon, NatT, LB, Ret_, Num("1"), Semi, RB, Semi);

    // ---- expressions --------------------------------------------------------
    [Fact]
    public void Lambda_requires_fat_arrow()
        => Rejects(Print_, Lambda_, LP, RP, Colon, NatT, Num("1"), Semi);

    [Fact] public void Dangling_binary_operator() => Rejects(Print_, Num("1"), Plus, Semi);
}
