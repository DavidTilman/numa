using MiniProject.Language.Structure;
using static MiniProject.Language.Tests.PT;

namespace MiniProject.Language.Tests;

// A program is a sequence of commands, each terminated by a semicolon. A
// command is a declaration (Record/Func) or a statement.
public class P_Program_Tests
{
    [Fact]
    public void Empty_program() => Assert.Empty(ParseP().Commands);

    [Fact]
    public void Single_statement_command()
    {
        P_Prog p = ParseP(Print_, Num("1"), Semi);
        Com c = Assert.Single(p.Commands);
        C_Stmt s = Assert.IsType<C_Stmt>(c);
        Assert.Equal(new S_Print(new E_Lit((ulong)1)), s.Stmt);
    }

    [Fact]
    public void Several_statements_in_order()
    {
        P_Prog p = ParseP(Print_, Num("1"), Semi, Print_, Num("2"), Semi);
        Assert.Equal(2, p.Commands.Count);
        Assert.Equal(new S_Print(new E_Lit((ulong)1)), ((C_Stmt)p.Commands[0]).Stmt);
        Assert.Equal(new S_Print(new E_Lit((ulong)2)), ((C_Stmt)p.Commands[1]).Stmt);
    }

    [Fact]
    public void Declarations_and_statements_can_mix()
    {
        // Record Pt(x : Nat); Func F() : Nat = { Ret 1; }; Print 1;
        P_Prog p = ParseP(
            Record_, Id("Pt"), LP, Id("x"), Colon, NatT, RP, Semi,
            Func_, Id("F"), LP, RP, Colon, NatT, Eq_, LB, Ret_, Num("1"), Semi, RB, Semi,
            Print_, Num("1"), Semi);
        Assert.Equal(3, p.Commands.Count);
        Assert.IsType<D_Record>(Assert.IsType<C_Decl>(p.Commands[0]).Decl);
        Assert.IsType<D_Func>(Assert.IsType<C_Decl>(p.Commands[1]).Decl);
        Assert.IsType<C_Stmt>(p.Commands[2]);
    }

    [Fact]
    public void Program_exposes_funcs_records_and_statements()
    {
        P_Prog p = ParseP(
            Record_, Id("Pt"), LP, Id("x"), Colon, NatT, RP, Semi,
            Func_, Id("F"), LP, RP, Colon, NatT, Eq_, LB, Ret_, Num("1"), Semi, RB, Semi,
            Print_, Num("1"), Semi);
        Assert.Single(p.Records);
        Assert.Single(p.Funcs);
        Assert.Single(p.Stmts);
    }
}
