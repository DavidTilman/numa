namespace MiniProject.Language;

abstract record Com;

sealed record CDecl(Decl Decl) : Com;
sealed record CStmt(Stmt Stmt) : Com;