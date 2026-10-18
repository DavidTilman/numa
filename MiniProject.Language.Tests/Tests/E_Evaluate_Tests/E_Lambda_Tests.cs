using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-Lambda: result clo(x, {Ret e;}, rho)
public class E_Lambda_Tests
{
    private static readonly IReadOnlyList<Param> Params = [new Param("x", new T_Nat())];

    [Fact]
    public void Evaluates_to_closure_with_its_parameters()
    {
        Value result = new E_Lambda(Params, new T_Nat(), new E_Var("x")).Evaluate(TestEnv.Empty);

        V_Closure clo = Assert.IsType<V_Closure>(result);
        Assert.Equal(Params, clo.Params);
    }

    [Fact]
    public void Body_is_wrapped_as_a_single_return()
    {
        Expr body = new E_Var("x");

        V_Closure clo = Assert.IsType<V_Closure>(new E_Lambda(Params, new T_Nat(), body).Evaluate(TestEnv.Empty));

        S_Return ret = Assert.IsType<S_Return>(Assert.Single(clo.Body.Statements));
        Assert.Same(body, ret.Expr);
    }

    [Fact]
    public void Captures_the_creation_environment()
    {
        Env env = TestEnv.Empty.Bind("y", new V_Nat(1));

        V_Closure clo = Assert.IsType<V_Closure>(new E_Lambda(Params, new T_Nat(), new E_Var("y")).Evaluate(env));

        Assert.Same(env, clo.Env);
    }

    [Fact]
    public void Body_is_not_evaluated()
    {
        // The body refers to an undefined variable, which would throw if it were run.
        var lambda = new E_Lambda(Params, new T_Nat(), new E_Var("undefined"));

        Assert.IsType<V_Closure>(lambda.Evaluate(TestEnv.Empty));
    }
}
