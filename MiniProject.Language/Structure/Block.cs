namespace MiniProject.Language.Structure;

public abstract record Block
{
    public abstract Outcome Execute(Env Env);
}
public sealed record B_Block(IReadOnlyList<Stmt> Statements) : Block
{
    public override Outcome Execute(Env Env)
    {
        if (Statements.Count == 0) return new O_Normal();
        Stmt cur = Statements[0];
        (Outcome o, Env e) = cur.Execute(Env);
        if (o is O_Ret) return o;
        B_Block newBlock = new([.. Statements.Skip(1)]);
        return newBlock.Execute(e);
    }
}