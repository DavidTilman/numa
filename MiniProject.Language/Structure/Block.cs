namespace MiniProject.Language.Structure;

sealed record B_Block(IReadOnlyList<Stmt> Statements)
{
    public Outcome Execute(Env Env)
    {
        if (Statements.Count == 0) return new O_Normal();
        Stmt cur = Statements[0];
        (Outcome o, Env e) = cur.Execute(Env);
        if (o is O_Ret) return o;
        B_Block newBlock = new([.. Statements.Skip(1)]);
        return newBlock.Execute(e);
    }
}