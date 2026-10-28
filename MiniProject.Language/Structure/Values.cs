namespace MiniProject.Language.Structure;
public abstract record Value
{
    public static bool ValueEquals(Value V1, Value V2) => Operation.Coerce(V1, V2) switch
    {
        (V_Nat a, V_Nat b) => a.Value == b.Value,
        (V_Int a, V_Int b) => a.Value == b.Value,
        (V_Real a, V_Real b) => a.Value == b.Value,
        (V_Bool a, V_Bool b) => a.Value == b.Value,
        (V_Record a, V_Record b) => a.Name == b.Name
            && a.Fields.Count == b.Fields.Count
            && a.Fields.Zip(b.Fields).All(x => ValueEquals(x.First, x.Second)),
        _ => throw new Exception("Invalid operation.")
    };

    public abstract string Show(Env env);
}

public sealed record V_Placeholder : Value
{
    public override string Show(Env env)
    {
        throw new Exception("Congratulations! You did the impossible! (hopefully)");
    }
}
public sealed record V_Nat(ulong Value) : Value
{
    public override string Show(Env env)
    {
        return Value.ToString();
    }
}
public sealed record V_Int(long Value) : Value
{
    public override string Show(Env env)
    {
        return Value.ToString();
    }
}
public sealed record V_Real(double Value) : Value
{
    public override string Show(Env env)
    {
        return Value.ToString();
    }
}
public sealed record V_Bool(bool Value) : Value
{
    public override string Show(Env env)
    {
        return Value.ToString();
    }
}
public sealed record V_Record(string Name, IReadOnlyList<Value> Fields) : Value
{
    public override string Show(Env env) // env is not modified we just need the record table 
    {
        IReadOnlyList<Param> _params = env.GetRecord(Name);
        List<(string, Value)> field_values = _params.Select(x => x.Name).Zip(Fields).ToList();
        string field_value_string = string.Join(", ", field_values.Select(x => $"{x.Item1} = {x.Item2.Show(env)}"));
        return $"{Name}({field_value_string})";
    }
}
public sealed record V_Closure(IReadOnlyList<Param> Params, B_Block Body, Env Env) : Value
{
    public override string Show(Env env)
    {
        return $"<function({string.Join(", ", Params.Select(x => $"{x.Name}: {x.Type}"))})>";
    }
}

public sealed record V_List(IReadOnlyList<Value> Elements) : Value
{
    public Value Access(V_Nat n)
    {
        if (n.Value >= (ulong)Elements.Count)
        {
            throw new Exception($"Index {n} out of range.");
        }
        return Elements[(int)n.Value];
    }
    public override string Show(Env env)
    {
        return $"[{string.Join(", ", Elements.Select(x => x.Show(env)))}]";
    }
}