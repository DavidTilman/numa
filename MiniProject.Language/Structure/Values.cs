namespace MiniProject.Language.Structure;
abstract record Value
{
    public static bool ValueEquals(Value V1, Value V2) => (V1, V2) switch
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
}
sealed record V_Nat(ulong Value) : Value;
sealed record V_Int(long Value) : Value;
sealed record V_Real(double Value) : Value;
sealed record V_Bool(bool Value) : Value;
sealed record V_Record(string Name, IReadOnlyList<Value> Fields) : Value;
sealed record V_Closure(IReadOnlyList<Param> Params, B_Block Body, Env Env) : Value;