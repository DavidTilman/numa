using System.Collections.Immutable;
namespace MiniProject.Language;

sealed class Ref(Value value)
{
    public Value Value { get; set; } = value;
}
sealed record Env(ImmutableDictionary<string, Ref> Vars);