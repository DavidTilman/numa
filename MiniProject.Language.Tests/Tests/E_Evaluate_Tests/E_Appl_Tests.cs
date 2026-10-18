using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

// E-App: evaluate to a closure, evaluate the arguments, bind fresh locations
// in the closure's environment, run the body, which must end with ret v
public class E_Appl_Tests
{
    private static E_Lambda Lambda(Expr body, params string[] names)
        => new(names.Select(n => new Param(n, new T_Nat())).ToList(), new T_Nat(), body);

    [Fact]
    public void Identity_returns_its_argument()
    {
        var call = new E_Appl(Lambda(new E_Var("x"), "x"), [new E_Lit(5UL)]);

        Assert.Equal(new V_Nat(5), call.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Arguments_are_bound_to_parameters_in_order()
    {
        var call = new E_Appl(Lambda(new E_Var("y"), "x", "y"), [new E_Lit(1UL), new E_Lit(2UL)]);

        Assert.Equal(new V_Nat(2), call.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Arguments_are_evaluated_in_the_callers_environment()
    {
        Env env = TestEnv.Empty.Bind("a", new V_Nat(7));
        var call = new E_Appl(Lambda(new E_Var("x"), "x"), [new E_Var("a")]);

        Assert.Equal(new V_Nat(7), call.Evaluate(env));
    }

    [Fact]
    public void Body_uses_the_closures_environment_not_the_callers()
    {
        // Static scoping: f captures y = 1, and is called where y = 2.
        Env defEnv = TestEnv.Empty.Bind("y", new V_Nat(1));
        Value f = Lambda(new E_Var("y")).Evaluate(defEnv);
        Env callEnv = TestEnv.Empty.Bind("y", new V_Nat(2)).Bind("f", f);

        Assert.Equal(new V_Nat(1), new E_Appl(new E_Var("f"), []).Evaluate(callEnv));
    }

    [Fact]
    public void Parameter_shadows_captured_variable()
    {
        Env env = TestEnv.Empty.Bind("x", new V_Nat(1));
        var call = new E_Appl(Lambda(new E_Var("x"), "x"), [new E_Lit(2UL)]);

        Assert.Equal(new V_Nat(2), call.Evaluate(env));
    }

    [Fact]
    public void Parameters_do_not_leak_into_the_callers_environment()
    {
        Env env = TestEnv.Empty;
        new E_Appl(Lambda(new E_Var("x"), "x"), [new E_Lit(1UL)]).Evaluate(env);

        Assert.Throws<Exception>(() => new E_Var("x").Evaluate(env));
    }

    [Fact]
    public void Applying_a_non_closure_throws()
    {
        Assert.Throws<Exception>(() => new E_Appl(new E_Lit(1UL), []).Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Wrong_number_of_arguments_throws()
    {
        var call = new E_Appl(Lambda(new E_Var("x"), "x"), []);

        Assert.Throws<Exception>(() => call.Evaluate(TestEnv.Empty));
    }

    [Fact]
    public void Body_without_return_throws()
    {
        Env env = TestEnv.Empty.Bind("f", new V_Closure([], new B_Block([]), TestEnv.Empty));

        Assert.Throws<Exception>(() => new E_Appl(new E_Var("f"), []).Evaluate(env));
    }
}
