using System.Collections.Immutable;
using MiniProject.Language.Structure;

namespace MiniProject.Language.Tests;

static class TestEnv
{
    public static Env Empty => new(
        ImmutableDictionary<string, Ref>.Empty,
        ImmutableDictionary<string, IReadOnlyList<Param>>.Empty);

    public static Env WithRecord(this Env env, string name, params Param[] fields)
        => env with { Records = env.Records.SetItem(name, fields) };
}
