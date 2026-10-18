using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// B-Empty, B-Seq, B-Ret
public class B_Block_Tests
{
    private static Stmt Assign(string x, ulong n) => new S_Assign(x, new E_Lit(n));

    private static Stmt Decl(string x, Expr e) => new S_Decl(x, new T_Nat(), e);

    [Fact]
    public void Empty_block_finishes_normally()
    {
        Assert.IsType<O_Normal>(new B_Block([]).Execute(TestEnv.Empty));
    }

    [Fact]
    public void Block_without_return_finishes_normally()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));

        Assert.IsType<O_Normal>(new B_Block([Assign("x", 1)]).Execute(env));
    }

    [Fact]
    public void Statements_run_in_order()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));

        new B_Block([Assign("x", 1), Assign("x", 2)]).Execute(env);

        Assert.Equal(new V_Nat(2), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Later_statements_see_earlier_declarations()
    {
        // { Var y = 3; Ret y; }
        var block = new B_Block([Decl("y", new E_Lit(3UL)), new S_Return(new E_Var("y"))]);

        Assert.Equal(new O_Ret(new V_Nat(3)), block.Execute(TestEnv.Empty));
    }

    [Fact]
    public void Return_skips_the_remaining_statements()
    {
        // { Ret 1; x := 5; }
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var block = new B_Block([new S_Return(new E_Lit(1UL)), Assign("x", 5)]);

        Outcome outcome = block.Execute(env);

        Assert.Equal(new O_Ret(new V_Nat(1)), outcome);
        Assert.Equal(new V_Nat(0), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Return_from_a_nested_statement_ends_the_block()
    {
        // { If (True) { Ret 1; } Else { }; x := 5; }
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));
        var inner = new B_Block([new S_Return(new E_Lit(1UL))]);
        var block = new B_Block([new S_Cond(new E_Lit(true), inner, new B_Block([])), Assign("x", 5)]);

        Assert.Equal(new O_Ret(new V_Nat(1)), block.Execute(env));
        Assert.Equal(new V_Nat(0), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Declarations_do_not_leak_out_of_the_block()
    {
        Env env = TestEnv.Empty;

        new B_Block([Decl("y", new E_Lit(1UL))]).Execute(env);

        Assert.Throws<Exception>(() => new E_Var("y").Evaluate(env));
    }

    [Fact]
    public void Inner_declaration_shadows_without_changing_the_outer_variable()
    {
        // x = 1 outside; { Var x = 2; Ret x; }
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        var block = new B_Block([Decl("x", new E_Lit(2UL)), new S_Return(new E_Var("x"))]);

        Assert.Equal(new O_Ret(new V_Nat(2)), block.Execute(env));
        Assert.Equal(new V_Nat(1), new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Assignments_to_outer_variables_persist_after_the_block()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(0));

        new B_Block([Assign("x", 7)]).Execute(env);

        Assert.Equal(new V_Nat(7), new E_Var("x").Evaluate(env));
    }
}
