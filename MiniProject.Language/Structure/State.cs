using System.Collections.Immutable;
namespace MiniProject.Language.Structure;

sealed class Ref(Value value)
{
    public Value Value { get; set; } = value;
}
sealed record Env(ImmutableDictionary<string, Ref> Vars, ImmutableDictionary<string, IReadOnlyList<Param>> Records)
{
    public Ref GetRef(string name)
    {
        return Vars.TryGetValue(name, out Ref? r) ? r : throw new Exception("Undefined variable access.");
    }

    public IReadOnlyList<Param> GetRecord(string name)
    {
        return Records.TryGetValue(name, out IReadOnlyList<Param>? r) ? r : throw new Exception("Undefined record access.");
    }

    public Env Bind(string name, Value val)
        => this with { Vars = Vars.SetItem(name, new Ref(val)) };
}