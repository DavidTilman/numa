using MiniProject.Language.Structure;
using MiniProject.Language.Lexing;
namespace MiniProject.Language.Parsing;

public class Parser
{
    List<Token> Tokens = [];
    private int pos = 0;
    private Token Peek() => Tokens[pos];
    private Token Advance() => Tokens[pos++];
    bool Check(TokenType t) => Peek().Type == t;
    Token Expect(TokenType t)
    {
        if (!Check(t))
            throw new Exception($"Expected {t} but found '{Peek().Lexeme}' (token {pos})");
        return Advance();
    }

    static readonly Dictionary<TokenType, (int Prec, Operation Op)> BinOps = new()
    {
        [TokenType.OR]   = (1, new OP_OR()),
        [TokenType.AND]  = (2, new OP_AND()),
        [TokenType.EQ]   = (3, new OP_EQ()),   [TokenType.NEQ] = (3, new OP_NEQ()),
        [TokenType.LT]   = (4, new OP_LT()),   [TokenType.LEQ] = (4, new OP_LEQ()),
        [TokenType.GT]   = (4, new OP_GT()),   [TokenType.GEQ] = (4, new OP_GEQ()),
        [TokenType.PLUS] = (5, new OP_Plus()), [TokenType.SUB] = (5, new OP_Sub()),
        [TokenType.MULT] = (6, new OP_Mult()), [TokenType.DIV] = (6, new OP_Div()),
        [TokenType.MOD]  = (6, new OP_Mod()),
    };

    #region Args & Params
    private List<Expr> ParseArgs()
    {
        Expect(TokenType.L_PAREN);
        List<Expr> args = [];
        while (!Check(TokenType.R_PAREN))
        {
            args.Add(ParseExpression());
            if (!Check(TokenType.COMMA)) break;
            Advance();
        }
        Expect(TokenType.R_PAREN);
        return args;
    }

    private Param ParseParam()
    {
        Token ident = Expect(TokenType.IDENT);
        Expect(TokenType.COLON);
        Structure.Type t = ParseType();
        return new Param(ident.Lexeme!, t);
    }

    private List<Param> ParseParams()
    {
        List<Param> _params = [];
        while(!Check(TokenType.R_PAREN))
        {
            Param p = ParseParam();
            _params.Add(p);
            if(Check(TokenType.COMMA))
            {
                Advance();
            } else {break;}
        }
        return _params;
    }
    #endregion

    #region Program 
    public P_Prog Parse(List<Token> tokens)
    {
        this.Tokens = tokens;
        this.pos = 0;
        List<Com> commands = [];
        while(!Check(TokenType.EOF))
        {
            commands.Add(ParseCommand()); 
            Expect(TokenType.SEMICOLON);
        }
        return new P_Prog(commands);
    }

    #endregion

    #region Commands
    private Com ParseCommand()
    {
        if (Check(TokenType.RECORD) || Check(TokenType.FUNC)) return ParseDeclaration();
        return ParseCStatement();
    }

    private C_Decl ParseDeclaration()
    {
        if (Check(TokenType.RECORD)) return new C_Decl(ParseRecordDeclaration());
        if (Check(TokenType.FUNC)) return new C_Decl(ParseFuncDeclaration());
        throw new Exception($"Attempted to parse {Peek().Type} as declaration.");
    }

    private C_Stmt ParseCStatement()
    {
        return new C_Stmt(ParseSStatement());
    }
    #endregion

    #region Declarations
    private D_Record ParseRecordDeclaration()
    {
        Expect(TokenType.RECORD);
        Token ident = Expect(TokenType.IDENT);
        Expect(TokenType.L_PAREN);
        List<Param> _params = ParseParams();
        Expect(TokenType.R_PAREN);

        return new D_Record(ident.Lexeme!, _params);
    }

    private D_Func ParseFuncDeclaration()
    {
        Expect(TokenType.FUNC);
        Token ident = Expect(TokenType.IDENT);
        Expect(TokenType.L_PAREN);
        List<Param> _params = ParseParams();
        Expect(TokenType.R_PAREN);
        Expect(TokenType.COLON);
        Structure.Type t = ParseType();
        Expect(TokenType.EQUALS);
        B_Block body = ParseBlock();
        return new D_Func(ident.Lexeme!, _params, t, body);
    }
    #endregion

    #region Type
    private Structure.Type ParseType()
    {
        Structure.Type t;
        Token tok = Advance();
        switch(tok.Type)
        {
            case (TokenType.NAT_TYPE):
                t = new T_Nat(); break;
            case (TokenType.INT_TYPE):
                t = new T_Int(); break;
            case (TokenType.REAL_TYPE):
                t = new T_Real(); break;
            case (TokenType.BOOL_TYPE):
                t = new T_Bool(); break;
            case (TokenType.IDENT):
                t = new T_Record(tok.Lexeme!); break;
            case (TokenType.L_PAREN):
                List<Structure.Type> types = [];
                while (!Check(TokenType.R_PAREN))
                {
                    types.Add(ParseType());
                    if (Check(TokenType.COMMA))
                    {
                        Advance();
                    } 
                    else
                    {
                        break;   
                    }
                } 
                Expect(TokenType.R_PAREN);
                Expect(TokenType.ARROW);
                t = new T_Func(types, ParseType());
                break;
            default:
                throw new Exception($"Attempted to parse {tok.Type} as type.");
        }

        return t;
    }
    #endregion
    
    #region Block
    private B_Block ParseBlock()
    {
        Expect(TokenType.L_BRACE);
        List<Stmt> stmts = [];
        while (!Check(TokenType.R_BRACE))
        {
            stmts.Add(ParseSStatement());
            Expect(TokenType.SEMICOLON);
        }
        Expect(TokenType.R_BRACE);
        return new B_Block(stmts);
    }
    #endregion

    #region Statement
    private Stmt ParseSStatement()
    {
        if(Check(TokenType.VAR))
        {
            Expect(TokenType.VAR);
            Token ident = Expect(TokenType.IDENT);
            Expect(TokenType.COLON);
            Structure.Type t = ParseType(); 
            Expect(TokenType.EQUALS);
            if (Check(TokenType.READ))
            {
                Advance();
                return new S_Read(ident.Lexeme!, t);
            }
            else
            {
                Expr e = ParseExpression();
                return new S_Decl(ident.Lexeme!, t, e);
            }
        }

        if (Check(TokenType.IDENT))
        {
            Token ident = Expect(TokenType.IDENT);
            Expect(TokenType.ASSIGN);
            Expr e = ParseExpression();
            return new S_Assign(ident.Lexeme!, e);
        }

        if (Check(TokenType.WHILE))
        {
            Expect(TokenType.WHILE);
            Expect(TokenType.L_PAREN);
            Expr e = ParseExpression();
            Expect(TokenType.R_PAREN);
            B_Block b = ParseBlock();
            return new S_While(e, b);
        }

        if (Check(TokenType.IF))
        {
            Expect(TokenType.IF);
            Expect(TokenType.L_PAREN);
            Expr e = ParseExpression();
            Expect(TokenType.R_PAREN);
            B_Block b1 = ParseBlock();
            B_Block b2;
            if (Check(TokenType.ELSE))
            {
                Expect(TokenType.ELSE);
                b2 = ParseBlock();
            } else
            {
                b2 = new B_Block([]);
            }
            return new S_Cond(e, b1, b2);
        }

        if (Check(TokenType.RET))
        {
            Expect(TokenType.RET);
            Expr e = ParseExpression();
            return new S_Return(e);
        }

        if (Check(TokenType.PRINT))
        {
            Expect(TokenType.PRINT);
            Expr e = ParseExpression();
            return new S_Print(e);
        }

        throw new Exception($"Unexpected {Peek().Type} at start of statement.");
    }
    #endregion

    #region Expression

    /*
        ParseExpression    e ? e1 : e2                (loosest)
        └ ParseBinary    || && == != < <= > >= + - * / %   (levels 1–6)
            └ ParseUnary     -e
                └ ParsePostfix   e(args)   e.f
                    └ ParsePrimary   literals, x, (e), New R(...), Lambda   (tightest)
    */
    private Expr ParseExpression()
    {
        Expr left = ParseBinary();
        if (!Check(TokenType.QUESTION)) return left;
        Expect(TokenType.QUESTION);
        Expr e1 = ParseExpression();
        Expect(TokenType.COLON);
        Expr e2 = ParseExpression();
        return new E_Cond(left, e1, e2);
    }

    private Expr ParseBinary(int minPrec = 1)
    {
        Expr left = ParseUnary();
        while (BinOps.ContainsKey(Peek().Type) && BinOps[Peek().Type].Prec >= minPrec)
        {
            (int prec, Operation op) = BinOps[Peek().Type];
            Advance();
            Expr right = ParseBinary(prec + 1);
            left = new E_Op(left, op, right);
        }
        return left;
    }

    private Expr ParseUnary()
    {
        if (Check(TokenType.SUB))
        {
            Advance();
            return new E_UnaryNegate(ParseUnary());
        }
        return ParsePostfix();
    }

    private Expr ParsePostfix()
    {
        Expr e = ParsePrimary();
        while (true)
        {
            if (Check(TokenType.L_PAREN))
                e = new E_Appl(e, ParseArgs());
            else if (Check(TokenType.DOT))
            {
                Advance();
                e = new E_Field(e, Expect(TokenType.IDENT).Lexeme!);
            }
            else return e;
        }
    }

    private Expr ParsePrimary()
    {
        Token tok = Advance();
        switch (tok.Type)
        {
            case TokenType.INT_LIT:
                return new E_Lit(ulong.Parse(tok.Lexeme!));
            case TokenType.REAL_LIT:
                return new E_Lit(double.Parse(tok.Lexeme!));
            case TokenType.TRUE_LIT:
                return new E_Lit(true);
            case TokenType.FALSE_LIT:
                return new E_Lit(false);
            case TokenType.IDENT:
                return new E_Var(tok.Lexeme!);
            case TokenType.L_PAREN:
                Expr e = ParseExpression();
                Expect(TokenType.R_PAREN);
                return e;
            case TokenType.NEW:
                string name = Expect(TokenType.IDENT).Lexeme!;
                return new E_RecCon(name, ParseArgs());
            case TokenType.LAMBDA:
                Expect(TokenType.L_PAREN);
                var _params = ParseParams();
                Expect(TokenType.R_PAREN);
                Expect(TokenType.COLON);
                var retType = ParseType();
                Expect(TokenType.FAT_ARROW);
                return new E_Lambda(_params, retType, ParseExpression());
            default:
                throw new Exception($"Unexpected {tok.Type} while parsing primary.");
        }
    }
    #endregion
}
