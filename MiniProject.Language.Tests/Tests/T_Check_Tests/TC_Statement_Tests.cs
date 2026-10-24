using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using static MiniProject.Language.Tests.TC;

namespace MiniProject.Language.Tests;

// Typing of statements: declarations, reads, assignments, loops, conditionals,
// returns and prints. Conditions must be Bool; returns must match the function
// return type; later statements see earlier bindings.
public class TC_Statement_Tests
{
    // ---- S_Decl -------------------------------------------------------------
    [Fact]
    public void Decl_binds_the_variable()
        => Ok(Do(Decl("x", TNat, Nat(1))), Do(PrintS(Var("x"))));

    [Fact]
    public void Decl_from_ill_typed_expression_is_rejected()
        => Rejected(Do(Decl("x", TNat, Op(Bool(true), new OP_Plus(), Bool(false)))));

    // ---- S_Read -------------------------------------------------------------
    [Fact]
    public void Read_binds_the_variable_at_its_declared_type()
        => Ok(Do(Read("b", TBool)), Do(If(Var("b"), [], [])));

    [Fact]
    public void Read_variable_type_is_used()
        // b is read as Nat, so it cannot drive an If condition (needs Bool).
        => Rejected<UnexpectedTypeException>(Do(Read("b", TNat)), Do(If(Var("b"), [], [])));

    // ---- S_Assign -----------------------------------------------------------
    [Fact]
    public void Assign_same_type()
        => Ok(Do(Decl("x", TInt, Int(0))), Do(Assign("x", Int(5))));

    [Fact]
    public void Assign_widening()
        => Ok(Do(Decl("x", TInt, Int(0))), Do(Assign("x", Nat(5))));

    [Fact]
    public void Assign_narrowing_is_rejected()
        => Rejected<UnexpectedTypeException>(Do(Decl("x", TNat, Nat(0))), Do(Assign("x", Neg(Nat(1)))));

    [Fact]
    public void Assign_wrong_type_is_rejected()
        => Rejected<UnexpectedTypeException>(Do(Decl("x", TNat, Nat(0))), Do(Assign("x", Bool(true))));

    [Fact]
    public void Assign_to_unbound_variable_is_rejected()
        => Rejected<UnBoundVarException>(Do(Assign("y", Nat(1))));

    // ---- S_While ------------------------------------------------------------
    [Fact]
    public void While_with_bool_condition()
        => Ok(Do(While(Op(Nat(1), new OP_LT(), Nat(2)))));

    [Fact]
    public void While_non_bool_condition_is_rejected()
        => Rejected<UnexpectedTypeException>(Do(While(Nat(1))));

    [Fact]
    public void While_body_is_checked_against_the_return_type()
        => Rejected(Fn("F", [], TNat, While(Bool(true), Ret(Bool(false)))));

    [Fact]
    public void While_body_sees_outer_variables()
        => Ok(Do(Decl("x", TNat, Nat(0))), Do(While(Bool(false), Assign("x", Nat(1)))));

    // ---- S_Cond -------------------------------------------------------------
    [Fact]
    public void If_with_bool_condition()
        => Ok(Do(If(Bool(true), [PrintS(Nat(1))], [PrintS(Nat(2))])));

    [Fact]
    public void If_non_bool_condition_is_rejected()
        => Rejected<UnexpectedTypeException>(Do(If(Nat(1), [], [])));

    [Fact]
    public void If_branches_are_checked_against_the_return_type()
        => Ok(Fn("F", [P("n", TInt)], TInt,
                 If(Op(Var("n"), new OP_GT(), Nat(0)), [Ret(Nat(1))], [Ret(Int(-1))])));

    [Fact]
    public void If_branch_with_bad_return_is_rejected()
        => Rejected(Fn("F", [], TNat,
                       If(Bool(true), [Ret(Bool(false))], [Ret(Nat(0))])));

    // ---- S_Return -----------------------------------------------------------
    [Fact]
    public void Return_assignable_to_return_type()
        => Ok(Fn("F", [], TInt, Ret(Nat(1))));

    [Fact]
    public void Return_widening()
        => Ok(Fn("F", [], TReal, Ret(Nat(1))));

    [Fact]
    public void Return_not_assignable_is_rejected()
        => Rejected<UnexpectedTypeException>(Fn("F", [], TNat, Ret(Int(1))));

    [Fact]
    public void Top_level_return_is_rejected()
        => Rejected<NotWellTypedException>(Do(Ret(Nat(1))));

    // ---- S_Print ------------------------------------------------------------
    [Fact]
    public void Print_well_typed_expression()
        => Ok(Do(PrintS(Op(Nat(1), new OP_Plus(), Nat(2)))));

    [Fact]
    public void Print_ill_typed_expression_is_rejected()
        => Rejected<UnBoundVarException>(Do(PrintS(Var("missing"))));
}
