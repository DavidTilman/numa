namespace MiniProject.Language.Structure;

abstract record Stmt
{
    public abstract (Outcome, Env) Execute(Env Env);
}
sealed record S_Decl(string Variable, Type Type, Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        return (new O_Normal(), Env.Bind(Variable, Expr.Evaluate(Env)));
    }
}

sealed record S_Assign(string Variable, Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Value v = Expr.Evaluate(Env);
        Env.GetRef(Variable).Value = v;
        return (new O_Normal(), Env);
    }
}

sealed record S_While(Expr Cond, B_Block Body) : Stmt
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

sealed record S_Cond(Expr Cond, B_Block OnTrue, B_Block OnFalse) : Stmt
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

sealed record S_Return(Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        return (new O_Ret(Expr.Evaluate(Env)), Env);
    }
}

sealed record S_Print(Expr Expr) : Stmt
{
    public override (Outcome, Env) Execute(Env Env)
    {
        Value v = Expr.Evaluate(Env);
        Console.WriteLine(v.Show(Env));
        return (new O_Normal(), Env);
    }
}