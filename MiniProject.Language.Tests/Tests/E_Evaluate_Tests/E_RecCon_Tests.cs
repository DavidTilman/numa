using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Record: evaluate each argument left to right, result R(v1..vn)
public class E_RecCon_Tests
{
    private static readonly Env Env = TestEnv.Empty
        .WithRecord("Vect2", new Param("x", new T_Int()), new Param("y", new T_Int()))
        .WithRecord("Unit");

    [Fact]
    public void Builds_record_with_name_and_fields_in_order()
    {
        Value result = new E_RecCon("Vect2", [new E_Lit(1L), new E_Lit(2L)]).Evaluate(Env);

        V_Record rec = Assert.IsType<V_Record>(result);
        Assert.Equal("Vect2", rec.Name);
        Assert.Equal([new V_Int(1), new V_Int(2)], rec.Fields);
    }

    [Fact]
    public void Arguments_are_evaluated_in_the_environment()
    {
        Env env = Env.Bind("a", new V_Int(7));

        V_Record rec = Assert.IsType<V_Record>(
            new E_RecCon("Vect2", [new E_Var("a"), new E_Lit(2L)]).Evaluate(env));
        Assert.Equal(new V_Int(7), rec.Fields[0]);
    }

    [Fact]
    public void Record_with_no_fields()
    {
        V_Record rec = Assert.IsType<V_Record>(new E_RecCon("Unit", []).Evaluate(Env));

        Assert.Empty(rec.Fields);
    }

    [Fact]
    public void Wrong_number_of_arguments_throws()
    {
        Assert.Throws<Exception>(() => new E_RecCon("Vect2", [new E_Lit(1L)]).Evaluate(Env));
    }

    [Fact]
    public void Undeclared_record_throws()
    {
        Assert.Throws<Exception>(() => new E_RecCon("Missing", []).Evaluate(Env));
    }
}
