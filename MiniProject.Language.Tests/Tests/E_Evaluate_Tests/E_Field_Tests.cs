using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Field: evaluate to R(v1..vn), Delta(R) = (f1..fn), f = fi, result vi
public class E_Field_Tests
{
    private static readonly Env Env = TestEnv.Empty
        .WithRecord("Vect2", new Param("x", new T_Int()), new Param("y", new T_Int()))
        .Bind("p", new V_Record("Vect2", [new V_Int(1), new V_Int(2)]));

    [Fact]
    public void First_field() => Assert.Equal(new V_Int(1), new E_Field(new E_Var("p"), "x").Evaluate(Env));

    [Fact]
    public void Second_field() => Assert.Equal(new V_Int(2), new E_Field(new E_Var("p"), "y").Evaluate(Env));

    [Fact]
    public void Unknown_field_throws()
    {
        Assert.Throws<Exception>(() => new E_Field(new E_Var("p"), "z").Evaluate(Env));
    }

    [Fact]
    public void Field_access_on_non_record_throws()
    {
        Assert.Throws<Exception>(() => new E_Field(new E_Lit(1UL), "x").Evaluate(Env));
    }

    [Fact]
    public void Undeclared_record_throws()
    {
        Env env = Env.Bind("q", new V_Record("Missing", [new V_Int(1)]));

        Assert.Throws<Exception>(() => new E_Field(new E_Var("q"), "x").Evaluate(env));
    }
}
