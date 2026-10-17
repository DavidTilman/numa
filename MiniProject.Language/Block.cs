namespace MiniProject.Language;

sealed record Block(IReadOnlyList<Stmt> Statements);