using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// P-Prog: Delta from the records, rho_g and sigma_0 from the functions (all
// capturing rho_g), then the top-level statements run as one block
[Collection("Console")]
public class P_Prog_Tests
{
    // Runs the program and returns its printed lines.
    private static string[] Run(params Com[] commands)
    {
        TextWriter original = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);
        try
        {
            new P_Prog(commands).Execute();
        }
        finally
        {
            Console.SetOut(original);
        }
        return writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private static Com Func(string name, string[] parameters, params Stmt[] body)
        => new C_Decl(new D_Func(name, parameters.Select(p => new Param(p, new T_Nat())).ToList(), new T_Nat(), new B_Block(body)));

    private static Com Record(string name, params string[] fields)
        => new C_Decl(new D_Record(name, fields.Select(f => new Param(f, new T_Nat())).ToList()));

    private static Com Do(Stmt stmt) => new C_Stmt(stmt);

    private static Stmt Print(Expr e) => new S_Print(e);

    private static Expr Nat(ulong n) => new E_Lit(n);

    private static Expr Var(string x) => new E_Var(x);

    private static Expr Call(string f, params Expr[] args) => new E_Appl(new E_Var(f), args);

    private static Expr Op(Expr l, Operation op, Expr r) => new E_Op(l, op, r);

    // Func Fact (n : Int) : Int = { If (n == 0) { Ret 1; } Else { Ret n * Fact(n - 1); } }
    // n is Int, not Nat: n - 1 has type Int, so it cannot flow back into a Nat parameter.
    private static readonly Com Fact = new C_Decl(new D_Func("Fact",
        [new Param("n", new T_Int())], new T_Int(),
        new B_Block([
            new S_Cond(Op(Var("n"), new OP_EQ(), Nat(0)),
                new B_Block([new S_Return(Nat(1))]),
                new B_Block([new S_Return(Op(Var("n"), new OP_Mult(), Call("Fact", Op(Var("n"), new OP_Sub(), Nat(1)))))]))
        ])));

    [Fact]
    public void Empty_program_runs()
    {
        Assert.Empty(Run());
    }

    [Fact]
    public void Top_level_statements_run_in_order()
    {
        Assert.Equal(["1", "2"], Run(Do(Print(Nat(1))), Do(Print(Nat(2)))));
    }

    [Fact]
    public void Top_level_variables_are_visible_to_later_statements()
    {
        Assert.Equal(["4"], Run(Do(new S_Decl("x", new T_Nat(), Nat(4))), Do(Print(Var("x")))));
    }

    [Fact]
    public void Function_can_be_called()
    {
        Assert.Equal(["5"], Run(Func("Id", ["x"], new S_Return(Var("x"))), Do(Print(Call("Id", Nat(5))))));
    }

    [Fact]
    public void Function_can_be_called_before_its_declaration()
    {
        Assert.Equal(["5"], Run(Do(Print(Call("Id", Nat(5)))), Func("Id", ["x"], new S_Return(Var("x")))));
    }

    [Fact]
    public void Recursion()
    {
        Assert.Equal(["120"], Run(Fact, Do(Print(Call("Fact", Nat(5))))));
    }

    [Fact]
    public void Mutual_recursion_in_either_order()
    {
        // Func IsEven (n : Int) : Bool = { If (n == 0) { Ret True; } Else { Ret IsOdd(n - 1); } }
        // Func IsOdd (n : Int)  : Bool = { If (n == 0) { Ret False; } Else { Ret IsEven(n - 1); } }
        Com isEven = new C_Decl(new D_Func("IsEven",
            [new Param("n", new T_Int())], new T_Bool(),
            new B_Block([
                new S_Cond(Op(Var("n"), new OP_EQ(), Nat(0)),
                    new B_Block([new S_Return(new E_Lit(true))]),
                    new B_Block([new S_Return(Call("IsOdd", Op(Var("n"), new OP_Sub(), Nat(1))))]))
            ])));
        Com isOdd = new C_Decl(new D_Func("IsOdd",
            [new Param("n", new T_Int())], new T_Bool(),
            new B_Block([
                new S_Cond(Op(Var("n"), new OP_EQ(), Nat(0)),
                    new B_Block([new S_Return(new E_Lit(false))]),
                    new B_Block([new S_Return(Call("IsEven", Op(Var("n"), new OP_Sub(), Nat(1))))]))
            ])));

        Assert.Equal(["True", "False"], Run(isEven, isOdd, Do(Print(Call("IsEven", Nat(4)))), Do(Print(Call("IsOdd", Nat(4))))));
    }

    [Fact]
    public void Function_is_a_value()
    {
        // Var f = Id; Print f(3);
        Assert.Equal(["3"], Run(Func("Id", ["x"], new S_Return(Var("x"))),
            Do(new S_Decl("f", new T_Nat(), Var("Id"))),
            Do(Print(new E_Appl(Var("f"), [Nat(3)])))));
    }

    [Fact]
    public void Functions_can_share_parameter_names()
    {
        Assert.Equal(["2"], Run(
            Func("F", ["x"], new S_Return(Var("x"))),
            Func("G", ["x"], new S_Return(Call("F", Var("x")))),
            Do(Print(Call("G", Nat(2))))));
    }

    [Fact]
    public void Functions_cannot_see_top_level_variables()
    {
        // Var x = 1; Func F () = { Ret x; }; Print F();
        Assert.Throws<Exception>(() => Run(
            Do(new S_Decl("x", new T_Nat(), Nat(1))),
            Func("F", [], new S_Return(Var("x"))),
            Do(Print(Call("F"))))) ;
    }

    [Fact]
    public void Records_are_available_to_field_access()
    {
        // Record Vect2 (x, y); Print Vect2(1, 2).y;
        Assert.Equal(["2"], Run(Record("Vect2", "x", "y"),
            Do(Print(new E_Field(new E_RecCon("Vect2", [Nat(1), Nat(2)]), "y")))));
    }

    [Fact]
    public void Top_level_return_throws()
    {
        Assert.Throws<Exception>(() => Run(Do(new S_Return(Nat(1)))));
    }

    [Fact]
    public void Duplicate_function_throws()
    {
        Assert.Throws<Exception>(() => Run(Func("F", [], new S_Return(Nat(1))), Func("F", [], new S_Return(Nat(2)))));
    }

    [Fact]
    public void Duplicate_parameter_throws()
    {
        Assert.Throws<Exception>(() => Run(Func("F", ["x", "x"], new S_Return(Nat(1)))));
    }

    [Fact]
    public void Duplicate_record_throws()
    {
        Assert.Throws<Exception>(() => Run(Record("R", "a"), Record("R", "b")));
    }
}
