using MiniProject.Language.Lexing;
using MiniProject.Language.Parsing;
using MiniProject.Language.Structure;
using MiniProject.Language.TypeChecking;

namespace MiniProject.Language.Cli;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0) return;

        string file = args[0];

        string programText = File.ReadAllText(file);

        List<Token> tokens = new Lexer().Lex(programText);
            
        P_Prog p = new Parser().Parse(tokens);
        new TypeChecker().Check(p);
        p.Execute();
    }
}