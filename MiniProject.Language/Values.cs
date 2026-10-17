namespace MiniProject.Language;

abstract record Value;

sealed record VNat(ulong Value) : Value;
sealed record VInt(long Value) : Value;
sealed record VReal(double Value) : Value;
sealed record VBool(bool Value) : Value;
sealed record VRecord(string Name, IReadOnlyList<Value> Fields) : Value;
sealed record VClosure(IReadOnlyList<Param> Params, Expr Body, Env Env) : Value;