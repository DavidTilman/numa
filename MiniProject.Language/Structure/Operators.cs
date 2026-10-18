namespace MiniProject.Language.Structure;

abstract record Operation
{
    public abstract Value Evaluate(Value V1, Value V2);
}

sealed record OP_Plus : Operation
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

sealed record OP_Sub : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return (V1, V2) switch
        {
            (V_Nat a, V_Nat b) => new V_Nat(a.Value >= b.Value ? a.Value - b.Value : 0UL),
            (V_Int a, V_Int b) => new V_Int(a.Value - b.Value),
            (V_Real a, V_Real b) => new V_Real(a.Value - b.Value),
            _ => throw new Exception("Invalid operation.")
        };
    }
}

sealed record OP_Mult : Operation
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

sealed record OP_Div : Operation
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

sealed record OP_Mod : Operation
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

sealed record OP_LT : Operation
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

sealed record OP_LEQ : Operation
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

sealed record OP_EQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return new V_Bool(Value.ValueEquals(V1, V2));
    }
}

sealed record OP_NEQ : Operation
{
    public override Value Evaluate(Value V1, Value V2)
    {
        return new V_Bool(!Value.ValueEquals(V1, V2));
    }
}

sealed record OP_GET : Operation
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

sealed record OP_GT : Operation
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

sealed record OP_OR : Operation
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

sealed record OP_AND : Operation
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