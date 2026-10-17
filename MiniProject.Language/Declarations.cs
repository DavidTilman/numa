namespace MiniProject.Language;

abstract record Decl;
sealed record DRecord(string Name, IReadOnlyList<Param> Params) : Decl;
sealed record DFunc(string Name, IReadOnlyList<Param> Params, Type ReturnType, Block Body) : Decl;