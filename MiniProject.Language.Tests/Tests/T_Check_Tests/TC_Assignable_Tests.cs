using MiniProject.Language.Structure;
using static MiniProject.Language.Tests.TC;

namespace MiniProject.Language.Tests;

// Assignable(from, to): same type, or a numeric widening Nat <= Int <= Real.
// A return site accepts `from` exactly when it is assignable to the return type.
public class TC_Assignable_Tests
{
    // same type
    [Fact] public void Nat_to_nat() => Ok(Returns(TNat, Nat(1)));
    [Fact] public void Int_to_int() => Ok(Returns(TInt, Int(1)));
    [Fact] public void Real_to_real() => Ok(Returns(TReal, Real(1)));
    [Fact] public void Bool_to_bool() => Ok(Returns(TBool, Bool(true)));

    // widening up the ladder is allowed
    [Fact] public void Nat_to_int() => Ok(Returns(TInt, Nat(1)));
    [Fact] public void Nat_to_real() => Ok(Returns(TReal, Nat(1)));
    [Fact] public void Int_to_real() => Ok(Returns(TReal, Int(1)));

    // narrowing down the ladder is rejected
    [Fact] public void Int_to_nat_rejected() => Rejected(Returns(TNat, Int(1)));
    [Fact] public void Real_to_int_rejected() => Rejected(Returns(TInt, Real(1)));
    [Fact] public void Real_to_nat_rejected() => Rejected(Returns(TNat, Real(1)));

    // bool does not mix with the numeric ladder in either direction
    [Fact] public void Bool_to_nat_rejected() => Rejected(Returns(TNat, Bool(true)));
    [Fact] public void Nat_to_bool_rejected() => Rejected(Returns(TBool, Nat(1)));

    // records are assignable only to the same-named record
    [Fact]
    public void Record_to_same_record()
        => Ok(Rec("Pt", P("x", TNat)), Fn("F", [], TRec("Pt"), Ret(RecCon("Pt", Nat(1)))));

    [Fact]
    public void Record_to_different_record_rejected()
        => Rejected(Rec("A", P("x", TNat)), Rec("B", P("y", TNat)),
                    Fn("F", [], TRec("B"), Ret(RecCon("A", Nat(1)))));

    // functions are assignable only to an identical signature (invariant)
    [Fact]
    public void Function_to_same_signature()
        => Ok(Fn("Id", [P("x", TNat)], TNat, Ret(Var("x"))),
              Fn("Use", [P("g", TFunc([TNat], TNat))], TNat, Ret(Call("g", Nat(1)))),
              Do(PrintS(Call("Use", Var("Id")))));

    [Fact]
    public void Function_to_different_return_rejected()
        => Rejected(Fn("Id", [P("x", TNat)], TNat, Ret(Var("x"))),
                    Fn("Use", [P("g", TFunc([TNat], TBool))], TBool, Ret(Call("g", Nat(1)))),
                    Do(PrintS(Call("Use", Var("Id")))));

    [Fact]
    public void Function_to_different_arity_rejected()
        => Rejected(Fn("Id", [P("x", TNat)], TNat, Ret(Var("x"))),
                    Fn("Use", [P("g", TFunc([TNat, TNat], TNat))], TNat, Ret(Nat(0))),
                    Do(PrintS(Call("Use", Var("Id")))));
}
