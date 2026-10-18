namespace MiniProject.Language.Structure;

abstract record Operation
{
    public abstract Value Evaluate(Value V1, Value V2);
}

sealed record O_Plus : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value + b.Value),
            (V_Int a, V_Int b) => new V_Int(a.Value + b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value + b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_Sub : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value <= b.Value ? a.Value - b.Value : 0UL),
            (V_Int a, V_Int b) => new V_Int(a.Value - b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value - b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_Mult : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value * b.Value),
            (V_Int a, V_Int b) => new V_Int(a.Value * b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value * b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_Div : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value / b.Value),
            (V_Int a, V_Int b) => new V_Int(a.Value / b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value / b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_Mod : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value % b.Value),
            (V_Int a, V_Int b) => new V_Int(a.Value % b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value % b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_LT : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Bool(a.Value < b.Value),
            (V_Int a, V_Int b) => new V_Bool(a.Value < b.Value),
            (V_Real a, V_Real b) => new V_Bool(a.Value < b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_LEQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Bool(a.Value <= b.Value),
            (V_Int a, V_Int b) => new V_Bool(a.Value <= b.Value),
            (V_Real a, V_Real b) => new V_Bool(a.Value <= b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_EQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return new V_Bool(Value.ValueEquals(V1, V2));
    }
}

sealed record O_NEQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return new V_Bool(!Value.ValueEquals(V1, V2));
    }
}

sealed record O_GET : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Bool(a.Value >= b.Value),
            (V_Int a, V_Int b) => new V_Bool(a.Value >= b.Value),
            (V_Real a, V_Real b) => new V_Bool(a.Value >= b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_GT : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Bool(a.Value > b.Value),
            (V_Int a, V_Int b) => new V_Bool(a.Value > b.Value),
            (V_Real a, V_Real b) => new V_Bool(a.Value > b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_OR : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Bool a, V_Bool b) => new V_Bool(a.Value || b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record O_AND : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Bool a, V_Bool b) => new V_Bool(a.Value && b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}