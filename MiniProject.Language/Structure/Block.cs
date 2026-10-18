namespace MiniProject.Language.Structure;

sealed record B_Block(IReadOnlyList<Stmt> Statements)
{
    public Outcome Execute(Env Env) => throw new NotImplementedException();
}