using System.Globalization;

namespace MiniProject.Language.Structure;

public abstract record Outcome;
public sealed record O_Normal : Outcome; 
public sealed record O_Ret(Value Value) : Outcome; 
public abstract record Stmt
{
    public abstract (Outcome, Env) Execute(Env Env);
}
public sealed record S_Decl(string Variable, Type Type, Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        return (new O_Normal(), Env.Bind(Variable, Expr.Evaluate(Env)));
    }
}

public sealed record S_Read(string Variable, Type Type) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Console.Write($"{Variable} := ");
        string input = Console.ReadLine() ?? throw new Exception("No input left to read.");
        Value v = Type switch
        {
            T_Nat => ulong.TryParse(input, CultureInfo.InvariantCulture, out ulong ul) ? new V_Nat(ul) : throw new Exception($"Unable to parse {Type}"),
            T_Int => long.TryParse(input, CultureInfo.InvariantCulture, out long l) ? new V_Int(l) : throw new Exception($"Unable to parse {Type}"),
            T_Real => double.TryParse(input, CultureInfo.InvariantCulture, out double d) ? new V_Real(d) : throw new Exception($"Unable to parse {Type}"),
            T_Bool => bool.TryParse(input, out bool b) ? new V_Bool(b) : throw new Exception($"Unable to parse {Type}"),
            _ => throw new Exception($"Unable to read to {Type}"),
        };
        return (new O_Normal(), Env.Bind(Variable, v));
    }
}

public sealed record S_Assign(string Variable, Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Value v = Expr.Evaluate(Env);
        Env.GetRef(Variable).Value = v;
        return (new O_Normal(), Env);
    }
}

public sealed record S_While(Expr Cond, B_Block Body) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        while(true)
        {
            Value cond = Cond.Evaluate(Env);
            if (cond is not V_Bool cond_result) throw new Exception("Attempted condition on non bool.");
            if (!cond_result.Value) return (new O_Normal(), Env); 
            Outcome block_out = Body.Execute(Env);
             if (block_out is O_Ret ret) return (ret, Env);
            
        }
    }
}

public sealed record S_Cond(Expr Cond, B_Block OnTrue, B_Block OnFalse) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Value cond = Cond.Evaluate(Env);
        if (cond is not V_Bool cond_result) throw new Exception("Attempted condition on non bool.");
        if(cond_result.Value)
        {
            return (OnTrue.Execute(Env), Env);
        }
        else
        {
            return (OnFalse.Execute(Env), Env);
        }
    }
}

public sealed record S_Return(Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        return (new O_Ret(Expr.Evaluate(Env)), Env);
    }
}

public sealed record S_Print(Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Value v = Expr.Evaluate(Env);
        Console.WriteLine(v.Show(Env));
        return (new O_Normal(), Env);
    }
}