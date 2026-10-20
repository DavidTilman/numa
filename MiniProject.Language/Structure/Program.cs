using System.Collections.Immutable;

namespace MiniProject.Language.Structure;
public abstract record Prog
{
    public abstract void Execute();
}
public sealed record P_Prog(IReadOnlyList<Com> Commands) : Prog
{
    public List<D_Func> Funcs = [];
    public List<Stmt> Stmts = [];
    public List<D_Record> Records = [];
    public Env Establish()
    {

        foreach (Com c in Commands)
        {
            switch (c)
            {
                case C_Decl d:
                    switch (d.Decl)
                    {
                        case D_Func f: Funcs.Add(f); break;
                        case D_Record r: Records.Add(r); break;
                    } 
                    break;
                case C_Stmt s:
                    Stmts.Add(s.Stmt); 
                    break;
            }
        }
        
        HashSet<string> seen = [];
        foreach (D_Func func in Funcs)
        {
            if (!seen.Add(func.Name))
                throw new Exception($"Function {func.Name} is defined more than once.");
            HashSet<string> seen_param = [];
            foreach (Param p in func.Params)
                if (!seen_param.Add(p.Name))
                    throw new Exception($"Function {func.Name} has duplicate parameters {p.Name}.");
        }
        
        Dictionary<string, IReadOnlyList<Param>> delta = [];
        foreach (D_Record rec in Records)
        {
            if (delta.TryGetValue(rec.Name, out _)) throw new Exception($"Record {rec.Name} is defined more than once.");
            delta.Add(rec.Name, rec.Params);
        }

        Env global_env = new([], delta.ToImmutableDictionary());
        
        foreach (D_Func func in Funcs)
        {
            global_env = global_env.Bind(func.Name, new V_Placeholder());
        }
        foreach (D_Func func in Funcs)
        {
            global_env.GetRef(func.Name).Value = new V_Closure(func.Params, func.Body, global_env);
        }

        return global_env;
    }
    public override void Execute()
    {
        Env global_env = Establish();
        B_Block main = new(Stmts);
        Outcome o = main.Execute(global_env);
        if (o is not O_Normal) throw new Exception("Program did not exit normally.");
    }
}