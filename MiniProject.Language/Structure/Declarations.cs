namespace MiniProject.Language.Structure;

abstract record Decl;
sealed record D_Record(string Name, IReadOnlyList<Param> Params) : Decl;
sealed record D_Func(string Name, IReadOnlyList<Param> Params, Type ReturnType, B_Block Body) : Decl;