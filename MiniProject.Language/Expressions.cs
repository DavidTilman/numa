namespace MiniProject.Language;

abstract record Expr ;
sealed record ELit(object Value) : Expr;
sealed record EVar(string Name) : Expr;
sealed record EOp(Expr Left, Expr Right) : Expr;
sealed record EField(Expr Target, string Field) : Expr;
sealed record EAppl(Expr Target, IReadOnlyList<Expr> Args) : Expr;
sealed record ERecCon(string Target, IReadOnlyList<Expr> Args) : Expr;
sealed record ELambda(IReadOnlyList<Param> Params, Type ReturnType, Expr Body) : Expr;
sealed record ECond(Expr Cond, Expr OnTrue, Expr OnFalse) : Expr;