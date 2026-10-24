using MiniProject.Language.Structure;
using static MiniProject.Language.Tests.PT;

namespace MiniProject.Language.Tests;

// Declaration and type grammar:
//   Record Name ( params )
//   Func Name ( params ) : type = { block }
//   types: Nat | Int | Real | Bool | Ident(record) | ( types ) -> type
public class P_Declaration_Tests
{
    // ---- records ------------------------------------------------------------
    [Fact]
    public void Record_declaration()
    {
        D_Record r = Assert.IsType<D_Record>(
            ParseD(Record_, Id("Pt"), LP, Id("x"), Colon, NatT, Comma, Id("y"), Colon, NatT, RP));
        Assert.Equal("Pt", r.Name);
        Assert.Equal([new Param("x", TNat), new Param("y", TNat)], r.Params);
    }

    [Fact]
    public void Record_declaration_with_no_fields()
    {
        D_Record r = Assert.IsType<D_Record>(ParseD(Record_, Id("Unit"), LP, RP));
        Assert.Equal("Unit", r.Name);
        Assert.Empty(r.Params);
    }

    // ---- functions ----------------------------------------------------------
    [Fact]
    public void Function_declaration()
    {
        D_Func f = Assert.IsType<D_Func>(
            ParseD(Func_, Id("F"), LP, Id("x"), Colon, NatT, RP, Colon, IntT, Eq_,
                   LB, Ret_, Id("x"), Semi, RB));
        Assert.Equal("F", f.Name);
        Assert.Equal([new Param("x", TNat)], f.Params);
        Assert.Equal(TInt, f.ReturnType);
        Assert.Equal(new S_Return(new E_Var("x")), Assert.Single(f.Body.Statements));
    }

    [Fact]
    public void Function_declaration_with_no_params()
    {
        D_Func f = Assert.IsType<D_Func>(
            ParseD(Func_, Id("Zero"), LP, RP, Colon, NatT, Eq_, LB, Ret_, Num("0"), Semi, RB));
        Assert.Equal("Zero", f.Name);
        Assert.Empty(f.Params);
        Assert.Equal(TNat, f.ReturnType);
    }

    [Fact]
    public void Function_declaration_with_several_params()
    {
        D_Func f = Assert.IsType<D_Func>(
            ParseD(Func_, Id("Add"), LP, Id("a"), Colon, NatT, Comma, Id("b"), Colon, NatT, RP,
                   Colon, NatT, Eq_, LB, Ret_, Id("a"), Plus, Id("b"), Semi, RB));
        Assert.Equal([new Param("a", TNat), new Param("b", TNat)], f.Params);
    }

    // ---- types --------------------------------------------------------------
    [Fact]
    public void Base_types_in_parameters()
    {
        D_Record r = Assert.IsType<D_Record>(
            ParseD(Record_, Id("R"), LP,
                   Id("a"), Colon, NatT, Comma, Id("b"), Colon, IntT, Comma,
                   Id("c"), Colon, RealT, Comma, Id("d"), Colon, BoolT, RP));
        Assert.Equal(
            [new Param("a", TNat), new Param("b", TInt), new Param("c", TReal), new Param("d", TBool)],
            r.Params);
    }

    [Fact]
    public void Record_type_as_a_field_type()
    {
        D_Record r = Assert.IsType<D_Record>(ParseD(Record_, Id("Line"), LP, Id("from"), Colon, Id("Pt"), RP));
        Assert.Equal([new Param("from", new T_Record("Pt"))], r.Params);
    }

    [Fact]
    public void Function_type_parameter()
    {
        // Func Use (g : (Nat) -> Int) : Nat = { Ret 0; }
        D_Func f = Assert.IsType<D_Func>(
            ParseD(Func_, Id("Use"), LP, Id("g"), Colon, LP, NatT, RP, Arrow, IntT, RP,
                   Colon, NatT, Eq_, LB, Ret_, Num("0"), Semi, RB));
        T_Func g = Assert.IsType<T_Func>(Assert.Single(f.Params).Type);
        Assert.Equal([TNat], g.ParamTypes);
        Assert.Equal(TInt, g.ReturnType);
    }

    [Fact]
    public void Function_type_with_several_parameters()
    {
        D_Record r = Assert.IsType<D_Record>(
            ParseD(Record_, Id("R"), LP, Id("f"), Colon, LP, NatT, Comma, IntT, RP, Arrow, BoolT, RP));
        T_Func ft = Assert.IsType<T_Func>(Assert.Single(r.Params).Type);
        Assert.Equal([TNat, TInt], ft.ParamTypes);
        Assert.Equal(TBool, ft.ReturnType);
    }

    [Fact]
    public void Function_type_with_no_parameters()
    {
        D_Record r = Assert.IsType<D_Record>(
            ParseD(Record_, Id("R"), LP, Id("thunk"), Colon, LP, RP, Arrow, NatT, RP));
        T_Func ft = Assert.IsType<T_Func>(Assert.Single(r.Params).Type);
        Assert.Empty(ft.ParamTypes);
        Assert.Equal(TNat, ft.ReturnType);
    }

    [Fact]
    public void Function_type_return_is_right_nested()
    {
        // (Nat) -> (Int) -> Bool   parses as  (Nat) -> ((Int) -> Bool)
        D_Record r = Assert.IsType<D_Record>(
            ParseD(Record_, Id("R"), LP, Id("f"), Colon,
                   LP, NatT, RP, Arrow, LP, IntT, RP, Arrow, BoolT, RP));
        T_Func outer = Assert.IsType<T_Func>(Assert.Single(r.Params).Type);
        Assert.Equal([TNat], outer.ParamTypes);
        T_Func inner = Assert.IsType<T_Func>(outer.ReturnType);
        Assert.Equal([TInt], inner.ParamTypes);
        Assert.Equal(TBool, inner.ReturnType);
    }
}
