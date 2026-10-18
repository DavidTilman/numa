namespace MiniProject.Language.Structure;

abstract record Stmt;
sealed record S_Decl(string Variable, Type Type, Expr Value) : Stmt;
sealed record S_Assign(string Variable, Expr Value) : Stmt;
sealed record S_While(Expr Cond, B_Block Body) : Stmt;
sealed record S_Cond(Expr Cond, B_Block OnTrue, B_Block OnFalse) : Stmt;
sealed record S_Return(Expr Value) : Stmt;