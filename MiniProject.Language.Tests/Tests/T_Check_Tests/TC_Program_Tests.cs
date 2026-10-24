using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using static MiniProject.Language.Tests.TC;

namespace MiniProject.Language.Tests;

// Whole-program checking: each function body is checked with its parameters and
// return type in scope and every function name visible, then the top-level
// statements are checked with no return type. Functions do not see top-level
// variables.
public class TC_Program_Tests
{
    [Fact] public void Empty_program_is_ok() => Ok();

    [Fact]
    public void Parameter_is_in_scope_in_the_body()
        => Ok(Fn("F", [P("x", TNat)], TNat, Ret(Var("x"))));

    [Fact]
    public void Parameters_pin_their_type_in_the_body()
        // x is Nat, so returning -x (Int) from a Nat function is rejected.
        => Rejected(Fn("F", [P("x", TNat)], TNat, Ret(Neg(Var("x")))));

    [Fact]
    public void Functions_can_call_each_other()
        => Ok(Fn("F", [P("x", TNat)], TNat, Ret(Var("x"))),
              Fn("G", [P("y", TNat)], TNat, Ret(Call("F", Var("y")))));

    [Fact]
    public void Function_can_be_used_before_its_declaration()
        => Ok(Fn("G", [P("y", TNat)], TNat, Ret(Call("F", Var("y")))),
              Fn("F", [P("x", TNat)], TNat, Ret(Var("x"))));

    [Fact]
    public void Functions_cannot_see_top_level_variables()
        => Rejected<UnBoundVarException>(
              Do(Decl("x", TNat, Nat(1))),
              Fn("F", [], TNat, Ret(Var("x"))));

    [Fact]
    public void Top_level_statements_see_earlier_declarations()
        => Ok(Do(Decl("x", TNat, Nat(1))),
              Do(Decl("y", TNat, Op(Var("x"), new OP_Plus(), Nat(1)))),
              Do(PrintS(Var("y"))));

    [Fact]
    public void Recursive_function_type_checks()
        // Func Fact (n : Int) : Int = { If (n == 0) { Ret 1; } Else { Ret n * Fact(n - 1); } }
        => Ok(Fn("Fact", [P("n", TInt)], TInt,
                 If(Op(Var("n"), new OP_EQ(), Nat(0)),
                    [Ret(Nat(1))],
                    [Ret(Op(Var("n"), new OP_Mult(), Call("Fact", Op(Var("n"), new OP_Sub(), Nat(1)))))])),
              Do(PrintS(Call("Fact", Int(5)))));

    [Fact]
    public void Mutually_recursive_functions_type_check()
        => Ok(Fn("IsEven", [P("n", TInt)], TBool,
                 If(Op(Var("n"), new OP_EQ(), Nat(0)),
                    [Ret(Bool(true))],
                    [Ret(Call("IsOdd", Op(Var("n"), new OP_Sub(), Nat(1))))])),
              Fn("IsOdd", [P("n", TInt)], TBool,
                 If(Op(Var("n"), new OP_EQ(), Nat(0)),
                    [Ret(Bool(false))],
                    [Ret(Call("IsEven", Op(Var("n"), new OP_Sub(), Nat(1))))])));

    [Fact]
    public void Error_in_one_of_several_functions_is_reported()
        => Rejected(Fn("Good", [P("x", TNat)], TNat, Ret(Var("x"))),
                    Fn("Bad", [], TNat, Ret(Bool(true))));

    [Fact]
    public void Function_passed_as_argument_and_applied()
        => Ok(Fn("Inc", [P("x", TNat)], TNat, Ret(Op(Var("x"), new OP_Plus(), Nat(1)))),
              Fn("Twice", [P("f", TFunc([TNat], TNat)), P("x", TNat)], TNat,
                 Ret(Call("f", Call("f", Var("x"))))),
              Do(PrintS(Call("Twice", Var("Inc"), Nat(0)))));
}
