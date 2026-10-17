namespace MiniProject.Language;

abstract record Stmt;
sealed record SDecl(string Variable, Type Type, Expr Value) : Stmt;
sealed record SAssign(string Variable, Expr Value) : Stmt;
sealed record SWhile(Expr Cond, Block Body) : Stmt;
sealed record SCond(Expr Cond, Block OnTrue, Block OnFalse) : Stmt;
sealed record SReturn(Expr Value) : Stmt;