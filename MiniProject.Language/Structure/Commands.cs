namespace MiniProject.Language.Structure;
abstract record Com;

sealed record C_Decl(Decl Decl) : Com;
sealed record C_Stmt(Stmt Stmt) : Com;