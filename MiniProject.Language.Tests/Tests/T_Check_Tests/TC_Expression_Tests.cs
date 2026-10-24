using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using static MiniProject.Language.Tests.TC;

namespace MiniProject.Language.Tests;

// Typing of the remaining expression forms: variables, field access, record
// construction, application, lambdas and conditionals.
public class TC_Expression_Tests
{
    // ---- E_Var --------------------------------------------------------------
    [Fact] public void Unbound_variable_is_rejected() => Rejected<UnBoundVarException>(Prints(Var("x")));

    [Fact]
    public void Declared_variable_is_visible_to_later_statements()
        => Ok(Do(Decl("x", TNat, Nat(1))), Do(PrintS(Var("x"))));

    [Fact]
    public void Declared_variable_keeps_its_expression_type()
        // x is declared from a Nat, so it is Nat: a Real cannot be assigned back.
        => Rejected(Do(Decl("x", TNat, Nat(1))), Do(Assign("x", Real(2))));

    // ---- E_Field ------------------------------------------------------------
    [Fact]
    public void Field_has_its_declared_type()
        => Ok(Rec("Pt", P("x", TNat), P("y", TInt)),
              Fn("F", [], TInt, Ret(Field(RecCon("Pt", Nat(1), Int(2)), "y"))));

    [Fact]
    public void Field_type_is_exact_not_widened()
        // y is Int, so a Nat return type must not accept it.
        => Rejected(Rec("Pt", P("x", TNat), P("y", TInt)),
                    Fn("F", [], TNat, Ret(Field(RecCon("Pt", Nat(1), Int(2)), "y"))));

    [Fact]
    public void Field_access_on_non_record_is_rejected()
        => Rejected<NotWellTypedException>(Prints(Field(Nat(1), "x")));

    [Fact]
    public void Field_access_on_unknown_field_is_rejected()
        => Rejected<NotWellTypedException>(
              Rec("Pt", P("x", TNat)),
              Do(PrintS(Field(RecCon("Pt", Nat(1)), "nope"))));

    // ---- E_RecCon -----------------------------------------------------------
    [Fact]
    public void Record_construction_with_matching_args()
        => Ok(Rec("Pt", P("x", TNat), P("y", TNat)),
              Do(PrintS(RecCon("Pt", Nat(1), Nat(2)))));

    [Fact]
    public void Record_construction_widens_args()
        => Ok(Rec("R", P("x", TReal)), Do(PrintS(RecCon("R", Nat(1)))));

    [Fact]
    public void Record_construction_unknown_type_is_rejected()
        => Rejected<UndefinedTypeException>(Prints(RecCon("Nope")));

    [Fact]
    public void Record_construction_wrong_arity_is_rejected()
        => Rejected<InvalidCallException>(
              Rec("Pt", P("x", TNat), P("y", TNat)),
              Do(PrintS(RecCon("Pt", Nat(1)))));

    [Fact]
    public void Record_construction_narrowing_arg_is_rejected()
        => Rejected(Rec("R", P("x", TNat)), Do(PrintS(RecCon("R", Int(1)))));

    // ---- E_Appl -------------------------------------------------------------
    [Fact]
    public void Application_returns_the_functions_return_type()
        => Ok(Fn("Id", [P("x", TNat)], TInt, Ret(Var("x"))),
              Fn("Use", [], TInt, Ret(Call("Id", Nat(1)))));

    [Fact]
    public void Application_return_type_is_exact()
        // Id returns Int, so a Nat return type must not accept the call.
        => Rejected(Fn("Id", [P("x", TNat)], TInt, Ret(Var("x"))),
                    Fn("Use", [], TNat, Ret(Call("Id", Nat(1)))));

    [Fact]
    public void Application_on_non_function_is_rejected()
        => Rejected<NotWellTypedException>(
              Do(Decl("x", TNat, Nat(1))),
              Do(PrintS(CallExpr(Var("x")))));

    [Fact]
    public void Application_wrong_arity_is_rejected()
        => Rejected<InvalidCallException>(
              Fn("Id", [P("x", TNat)], TNat, Ret(Var("x"))),
              Do(PrintS(Call("Id"))));

    [Fact]
    public void Application_wrong_arg_type_is_rejected()
        => Rejected(Fn("Id", [P("x", TNat)], TNat, Ret(Var("x"))),
                    Do(PrintS(Call("Id", Bool(true)))));

    [Fact]
    public void Application_widening_arg_is_allowed()
        => Ok(Fn("Id", [P("x", TReal)], TReal, Ret(Var("x"))),
              Do(PrintS(Call("Id", Nat(1)))));

    // ---- E_Lambda -----------------------------------------------------------
    [Fact]
    public void Lambda_with_matching_body()
        => Ok(Prints(Lambda([P("x", TNat)], TNat, Var("x"))));

    [Fact]
    public void Lambda_body_widens_to_return_type()
        => Ok(Prints(Lambda([P("x", TNat)], TReal, Var("x"))));

    [Fact]
    public void Lambda_body_not_assignable_is_rejected()
        => Rejected(Prints(Lambda([P("x", TNat)], TBool, Var("x"))));

    [Fact]
    public void Lambda_parameter_is_in_scope_in_body()
        => Ok(Prints(Lambda([P("x", TNat)], TNat, Op(Var("x"), new OP_Plus(), Nat(1)))));

    [Fact]
    public void Lambda_body_unbound_variable_is_rejected()
        => Rejected<UnBoundVarException>(Prints(Lambda([P("x", TNat)], TNat, Var("y"))));

    [Fact]
    public void Lambda_can_be_applied()
        => Ok(Returns(TNat, CallExpr(Lambda([P("x", TNat)], TNat, Var("x")), Nat(1))));

    // ---- E_Cond -------------------------------------------------------------
    [Fact]
    public void Cond_equal_branches_keep_their_type()
        => Ok(Returns(TNat, Cond(Bool(true), Nat(1), Nat(2))));

    [Fact]
    public void Cond_branches_widen_to_common_type()
        => Ok(Returns(TInt, Cond(Bool(true), Nat(1), Int(2))));

    [Fact]
    public void Cond_widened_branches_are_not_narrower()
        => Rejected(Returns(TNat, Cond(Bool(true), Nat(1), Int(2))));

    [Fact]
    public void Cond_bool_branches()
        => Ok(Returns(TBool, Cond(Bool(true), Bool(true), Bool(false))));

    [Fact]
    public void Cond_non_bool_condition_is_rejected()
        => Rejected<UnexpectedTypeException>(Prints(Cond(Nat(1), Nat(1), Nat(2))));

    [Fact]
    public void Cond_incompatible_branches_are_rejected()
        => Rejected(Prints(Cond(Bool(true), Nat(1), Bool(false))));

    [Fact]
    public void Cond_same_record_branches()
        => Ok(Rec("Pt", P("x", TNat)),
              Fn("F", [], TRec("Pt"),
                 Ret(Cond(Bool(true), RecCon("Pt", Nat(1)), RecCon("Pt", Nat(2))))));

    [Fact]
    public void Cond_different_record_branches_are_rejected()
        => Rejected(Rec("A", P("x", TNat)), Rec("B", P("y", TNat)),
                    Do(PrintS(Cond(Bool(true), RecCon("A", Nat(1)), RecCon("B", Nat(2))))));
}
