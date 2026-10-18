namespace MiniProject.Language.Structure;
abstract record Value;
sealed record V_Nat(ulong Value) : Value;
sealed record V_Int(long Value) : Value;
sealed record V_Real(double Value) : Value;
sealed record V_Bool(bool Value) : Value;
sealed record V_Record(string Name, IReadOnlyList<Value> Fields) : Value;
sealed record V_Closure(IReadOnlyList<Param> Params, B_Block Body, Env Env) : Value;