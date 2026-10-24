using MiniProject.Language.Structure;
using MiniProject.Language.Exceptions;
using static MiniProject.Language.Tests.TC;

namespace MiniProject.Language.Tests;

// Sig: the type of a binary operation. Arithmetic takes the widest operand
// (Nat <= Int <= Real); subtraction is at least Int; comparisons give Bool;
// logical operators are Bool-only. A return type that accepts the result pins it.
public class TC_Operator_Tests
{
    // ---- addition / multiplication / division / modulo : widest operand -----
    [Fact] public void Plus_nat_nat_is_nat() => Ok(Returns(TNat, Op(Nat(1), new OP_Plus(), Nat(2))));
    [Fact] public void Plus_nat_int_is_int() => Ok(Returns(TInt, Op(Nat(1), new OP_Plus(), Int(2))));
    [Fact] public void Plus_nat_int_is_not_nat() => Rejected(Returns(TNat, Op(Nat(1), new OP_Plus(), Int(2))));
    [Fact] public void Plus_int_int_is_int() => Ok(Returns(TInt, Op(Int(1), new OP_Plus(), Int(2))));
    [Fact] public void Plus_nat_real_is_real() => Ok(Returns(TReal, Op(Nat(1), new OP_Plus(), Real(2))));
    [Fact] public void Plus_nat_real_is_not_int() => Rejected(Returns(TInt, Op(Nat(1), new OP_Plus(), Real(2))));
    [Fact] public void Plus_int_real_is_real() => Ok(Returns(TReal, Op(Int(1), new OP_Plus(), Real(2))));
    [Fact] public void Plus_real_real_is_real() => Ok(Returns(TReal, Op(Real(1), new OP_Plus(), Real(2))));
    [Fact] public void Plus_bool_bool_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Bool(true), new OP_Plus(), Bool(false))));

    [Fact] public void Mult_nat_nat_is_nat() => Ok(Returns(TNat, Op(Nat(2), new OP_Mult(), Nat(3))));
    [Fact] public void Mult_int_real_is_real() => Ok(Returns(TReal, Op(Int(2), new OP_Mult(), Real(3))));
    [Fact] public void Div_nat_nat_is_nat() => Ok(Returns(TNat, Op(Nat(6), new OP_Div(), Nat(2))));
    [Fact] public void Mod_nat_nat_is_nat() => Ok(Returns(TNat, Op(Nat(7), new OP_Mod(), Nat(2))));
    [Fact] public void Mult_bool_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Bool(true), new OP_Mult(), Bool(true))));

    // ---- subtraction : result is at least Int -------------------------------
    [Fact] public void Sub_nat_nat_is_int() => Ok(Returns(TInt, Op(Nat(5), new OP_Sub(), Nat(3))));
    [Fact] public void Sub_nat_nat_is_not_nat() => Rejected(Returns(TNat, Op(Nat(5), new OP_Sub(), Nat(3))));
    [Fact] public void Sub_int_int_is_int() => Ok(Returns(TInt, Op(Int(5), new OP_Sub(), Int(3))));
    [Fact] public void Sub_nat_real_is_real() => Ok(Returns(TReal, Op(Nat(5), new OP_Sub(), Real(3))));
    [Fact] public void Sub_real_real_is_real() => Ok(Returns(TReal, Op(Real(5), new OP_Sub(), Real(3))));

    // ---- comparisons : Bool --------------------------------------------------
    [Fact] public void LT_numeric_is_bool() => Ok(Returns(TBool, Op(Nat(1), new OP_LT(), Nat(2))));
    [Fact] public void LT_numeric_is_not_nat() => Rejected(Returns(TNat, Op(Nat(1), new OP_LT(), Nat(2))));
    [Fact] public void LEQ_mixed_numeric_is_bool() => Ok(Returns(TBool, Op(Int(1), new OP_LEQ(), Real(2))));
    [Fact] public void GT_numeric_is_bool() => Ok(Returns(TBool, Op(Nat(3), new OP_GT(), Int(2))));
    [Fact] public void GEQ_numeric_is_bool() => Ok(Returns(TBool, Op(Real(3), new OP_GEQ(), Nat(2))));
    [Fact] public void LT_numeric_vs_bool_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Nat(1), new OP_LT(), Bool(true))));

    // ---- equality / inequality ----------------------------------------------
    [Fact] public void EQ_numeric_is_bool() => Ok(Returns(TBool, Op(Nat(1), new OP_EQ(), Nat(1))));
    [Fact] public void EQ_mixed_numeric_is_bool() => Ok(Returns(TBool, Op(Int(1), new OP_EQ(), Real(1))));
    [Fact] public void EQ_bool_bool_is_bool() => Ok(Returns(TBool, Op(Bool(true), new OP_EQ(), Bool(false))));
    [Fact] public void NEQ_bool_bool_is_bool() => Ok(Returns(TBool, Op(Bool(true), new OP_NEQ(), Bool(false))));
    [Fact] public void EQ_numeric_vs_bool_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Nat(1), new OP_EQ(), Bool(true))));

    [Fact]
    public void EQ_same_record_is_bool()
        => Ok(Rec("Pt", P("x", TNat)),
              Fn("F", [], TBool, Ret(Op(RecCon("Pt", Nat(1)), new OP_EQ(), RecCon("Pt", Nat(2))))));

    [Fact]
    public void EQ_different_records_is_rejected()
        => Rejected<OperationNotDefinedException>(
              Rec("A", P("x", TNat)), Rec("B", P("y", TNat)),
              Do(PrintS(Op(RecCon("A", Nat(1)), new OP_EQ(), RecCon("B", Nat(2))))));

    [Fact]
    public void EQ_record_vs_numeric_is_rejected()
        => Rejected<OperationNotDefinedException>(
              Rec("A", P("x", TNat)),
              Do(PrintS(Op(RecCon("A", Nat(1)), new OP_EQ(), Nat(1)))));

    // ---- logical : Bool-only -------------------------------------------------
    [Fact] public void And_bool_bool_is_bool() => Ok(Returns(TBool, Op(Bool(true), new OP_AND(), Bool(false))));
    [Fact] public void Or_bool_bool_is_bool() => Ok(Returns(TBool, Op(Bool(true), new OP_OR(), Bool(false))));
    [Fact] public void And_numeric_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Nat(1), new OP_AND(), Nat(1))));
    [Fact] public void And_bool_numeric_is_rejected() => Rejected<OperationNotDefinedException>(Prints(Op(Bool(true), new OP_AND(), Nat(1))));

    // ---- unary negation : Nat -> Int, Int -> Int, Real -> Real --------------
    [Fact] public void Neg_nat_is_int() => Ok(Returns(TInt, Neg(Nat(1))));
    [Fact] public void Neg_nat_is_not_nat() => Rejected(Returns(TNat, Neg(Nat(1))));
    [Fact] public void Neg_int_is_int() => Ok(Returns(TInt, Neg(Int(1))));
    [Fact] public void Neg_real_is_real() => Ok(Returns(TReal, Neg(Real(1))));
    [Fact] public void Neg_real_is_not_int() => Rejected(Returns(TInt, Neg(Real(1))));
    [Fact] public void Neg_bool_is_rejected() => Rejected<UndefinedUnaryNegationException>(Prints(Neg(Bool(true))));
}
