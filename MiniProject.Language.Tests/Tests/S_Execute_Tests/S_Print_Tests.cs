using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// Console output is global, so tests that capture it must not run in parallel.
[CollectionDefinition("Console", DisableParallelization = true)]
public class ConsoleCollection;

// S-Print: evaluate e to v, output v (informal), result normal with the same rho
[Collection("Console")]
public class S_Print_Tests
{
    private static readonly Env Records = TestEnv.Empty
        .WithRecord("Vect2", new Param("x", new T_Int()), new Param("y", new T_Int()))
        .WithRecord("Line", new Param("start", new T_Record("Vect2")), new Param("end", new T_Record("Vect2")));

    // Runs the statement and returns everything it printed.
    private static string Output(Stmt stmt, Env env)
    {
        TextWriter original = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);
        try
        {
            stmt.Execute(env);
        }
        finally
        {
            Console.SetOut(original);
        }
        return writer.ToString();
    }

    private static string Printed(Value v, Env env)
        => Output(new S_Print(new E_Var("v")), env.Bind("v", v)).TrimEnd('\r', '\n');

    [Fact]
    public void Prints_a_nat() => Assert.Equal("5", Printed(new V_Nat(5), TestEnv.Empty));

    [Fact]
    public void Prints_a_negative_int() => Assert.Equal("-3", Printed(new V_Int(-3), TestEnv.Empty));

    [Fact]
    public void Prints_a_bool() => Assert.Equal("True", Printed(new V_Bool(true), TestEnv.Empty));

    [Fact]
    public void Prints_a_record_with_field_names()
    {
        var v = new V_Record("Vect2", [new V_Int(1), new V_Int(2)]);

        Assert.Equal("Vect2(x = 1, y = 2)", Printed(v, Records));
    }

    [Fact]
    public void Prints_a_nested_record()
    {
        var start = new V_Record("Vect2", [new V_Int(1), new V_Int(2)]);
        var end = new V_Record("Vect2", [new V_Int(3), new V_Int(4)]);

        Assert.Equal("Line(start = Vect2(x = 1, y = 2), end = Vect2(x = 3, y = 4))",
            Printed(new V_Record("Line", [start, end]), Records));
    }

    [Fact]
    public void Prints_a_closure_with_its_parameters()
    {
        var f = new V_Closure([new Param("x", new T_Nat()), new Param("y", new T_Bool())], new B_Block([]), TestEnv.Empty);

        Assert.Equal("<function(x: Nat, y: Bool)>", Printed(f, TestEnv.Empty));
    }

    [Fact]
    public void Prints_one_line_per_statement()
    {
        var block = new B_Block([new S_Print(new E_Lit(1UL)), new S_Print(new E_Lit(2UL))]);

        string output = Output(new S_Cond(new E_Lit(true), block, new B_Block([])), TestEnv.Empty);

        Assert.Equal(["1", "2"], output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void Expression_is_evaluated_in_the_environment()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(7));

        Assert.Equal("7", Output(new S_Print(new E_Var("x")), env).TrimEnd('\r', '\n'));
    }

    [Fact]
    public void Print_inside_a_function_body_happens_when_called()
    {
        // Var r = (Lambda () : Nat => ...) is not enough, a lambda body is one Ret,
        // so build a closure whose block prints then returns.
        var body = new B_Block([new S_Print(new E_Lit(9UL)), new S_Return(new E_Lit(0UL))]);
        Env env = TestEnv.Empty.Bind("f", new V_Closure([], body, TestEnv.Empty));

        string output = Output(new S_Decl("r", new T_Nat(), new E_Appl(new E_Var("f"), [])), env);

        Assert.Equal("9", output.TrimEnd('\r', '\n'));
    }

    [Fact]
    public void Finishes_normally_with_the_same_environment()
    {
        Env env = TestEnv.Empty;
        TextWriter original = Console.Out;
        Console.SetOut(TextWriter.Null);
        try
        {
            var (outcome, after) = new S_Print(new E_Lit(1UL)).Execute(env);

            Assert.IsType<O_Normal>(outcome);
            Assert.Same(env, after);
        }
        finally
        {
            Console.SetOut(original);
        }
    }
}
