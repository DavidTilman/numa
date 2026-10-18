namespace MiniProject.Language.Structure;

abstract record Operation
{
    public abstract Value Evaluate(Value V1, Value V2);
}

sealed record O_Plus : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_Sub : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_Mult : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_Div : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_Mod : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_LT : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_LEQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_EQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_NEQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_GET : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_GT : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_OR : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}

sealed record O_AND : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        throw new NotImplementedException();
    }
}