using MiniProject.Language.Structure;
using MiniProject.Language.Lexing;
using static MiniProject.Language.Lexing.TokenType;
using Type = MiniProject.Language.Structure.Type;
using MiniProject.Language.Exceptions;
using System.Security;

namespace MiniProject.Language.Parsing;

public class Parser
{
    List<Token> Tokens = [];
    private int pos = 0;
    private Token Peek()
        => Tokens[pos];
    private Token Advance() 
        => Tokens[pos++];
    bool Check(TokenType t) 
        => Peek().Type == t;
    Token Expect(TokenType t) {
        if (!Check(t))
            throw new UnexpectedTokenException(t,Peek().Type,pos);
        
        return Advance();
    }

    static readonly Dictionary<TokenType, (int Prec, Operation Op)> BinOps = new() {
        [OR]   = (1, new OP_OR()),
        [AND]  = (2, new OP_AND()),
        [EQ]   = (3, new OP_EQ()),   [NEQ] = (3, new OP_NEQ()),
        [LT]   = (4, new OP_LT()),   [LEQ] = (4, new OP_LEQ()),
        [GT]   = (4, new OP_GT()),   [GEQ] = (4, new OP_GEQ()),
        [PLUS] = (5, new OP_Plus()), [SUB] = (5, new OP_Sub()),
        [MULT] = (6, new OP_Mult()), [DIV] = (6, new OP_Div()),
        [MOD]  = (6, new OP_Mod()),
    };

    #region Args & Params
    private List<Expr> ParseArgs() {
        // (x1, ...)
        
        Expect(L_PAREN);
        
        List<Expr> args = [];
        while (!Check(R_PAREN)) {
            args.Add(ParseExpression());
            
            if (!Check(COMMA)) break;
            
            Advance();
        }
        
        Expect(R_PAREN);
        
        return args;
    }

    private Param ParseParam() {
        // x:t
        
        Token ident = Expect(IDENT);
        Expect(COLON);
        
        Type t = ParseType();
        
        return new Param(ident.Lexeme!, t);
    }

    private List<Param> ParseParams() {
        // x1:t1,...
        
        List<Param> _params = [];
        while(!Check(R_PAREN)) {
            Param p = ParseParam();
            _params.Add(p);

            if(Check(COMMA))
                Advance();
            else 
                break;
        }

        return _params;
    }
    #endregion

    #region Program 
    public P_Prog Parse(List<Token> tokens) {
        Tokens = tokens;
        pos = 0;
        List<Com> commands = [];

        while(!Check(EOF)) {
            commands.Add(ParseCommand()); 
            Expect(SEMICOLON);
        }

        return new P_Prog(commands);
    }

    #endregion

    #region Commands
    private Com ParseCommand() {
        // d | s
        if (Check(RECORD) || Check(FUNC))
            // d 
            return ParseDeclaration();
        else 
            // s
            return ParseCStatement();
    }

    private C_Decl ParseDeclaration() {
        // Record R | Func F
        
        if (Check(RECORD))
            // Record R
            return new C_Decl(ParseRecordDeclaration());
        
        if (Check(FUNC)) 
            // Func F
            return new C_Decl(ParseFuncDeclaration());
        
        throw new InvalidParseException(Peek().Type.GetType(), typeof(C_Decl));
    }

    private C_Stmt ParseCStatement() => /* s */ new(ParseSStatement());
    
    #endregion

    #region Declarations
    private D_Record ParseRecordDeclaration() {
        // Record R (params, ... )
        Expect(RECORD);

        Token ident = Expect(IDENT);

        Expect(L_PAREN);
        List<Param> _params = ParseParams();
        Expect(R_PAREN);

        return new D_Record(ident.Lexeme!, _params);
    }

    private D_Func ParseFuncDeclaration() {
        // Func F (params) : retType = b
        Expect(FUNC);

        Token ident = Expect(IDENT);

        Expect(L_PAREN);
        List<Param> _params = ParseParams();
        Expect(R_PAREN);
        
        Expect(COLON);
        
        Type t = ParseType();
        
        Expect(EQUALS);
        
        B_Block body = ParseBlock();
        
        return new D_Func(ident.Lexeme!, _params, t, body);
    }
    #endregion

    #region Type
    private Type ParseType() {
        // Nat | Int | Real | Bool | (τ_1,...,τ_n) -> τ | R
        Type t;
        Token tok = Advance();
        switch(tok.Type) {
            case NAT_TYPE:  t = new T_Nat(); break;
            case INT_TYPE:  t = new T_Int(); break;
            case REAL_TYPE: t = new T_Real(); break;
            case BOOL_TYPE: t = new T_Bool(); break;
            case IDENT:     t = new T_Record(tok.Lexeme!); break;
            case L_PAREN:
                // (τ_1,...,τ_n) -> τ
                List<Type> types = [];
                while (!Check(R_PAREN)) {
                    types.Add(ParseType());
                    
                    if (Check(COMMA))
                        Advance();
                    else
                        break;   
                } 
                Expect(R_PAREN);

                Expect(ARROW);
                
                t = new T_Func(types, ParseType());
                
                break;
            case L_SQUARE:
                Type lt = ParseType();
                Expect(R_SQUARE);
                t = new T_List(lt);
                break;
            default: throw new InvalidParseException(tok.Type.GetType(), typeof(Type));
        }

        return t;
    }
    #endregion
    
    #region Block
    private B_Block ParseBlock() {
        // { s; , ... }
        Expect(L_BRACE);

        List<Stmt> stmts = [];
        while (!Check(R_BRACE)) {
            stmts.Add(ParseSStatement());
            Expect(SEMICOLON);
        }

        Expect(R_BRACE);

        return new B_Block(stmts);
    }
    #endregion

    #region Statement
    private Stmt ParseSStatement() => 
        /*
              Var x : τ = e 
            | Var x : τ = Read 
            | x := e 
            | While (e) b 
            | If (e) b 
            | If (e) b1 Else b2 
            | Ret e 
            | Print e  
        */
        Peek().Type switch {
            VAR => ParseSDecl(),
            IDENT => ParseSAssign(),
            WHILE => ParseSWhile(),
            IF => ParseSCond(),
            RET => ParseSRet(),
            PRINT => ParseSPrint(),
            _ => throw new UnexpectedStartException(Peek().Type, "statement", pos)
        };

    private Stmt ParseSDecl() {
        // Var x : τ = (e | Read) 
        Expect(VAR);

        Token ident = Expect(IDENT);
        
        Expect(COLON);
        
        Type t = ParseType(); 
        
        Expect(EQUALS);
        if (Check(READ)) {
            Advance();
            return new S_Read(ident.Lexeme!, t);
        }
        else 
            return new S_Decl(ident.Lexeme!, t, ParseExpression());
    }

    private S_Assign ParseSAssign() {
        // x := e 
        Token ident = Expect(IDENT);
        Expect(ASSIGN);
        return new S_Assign(ident.Lexeme!, ParseExpression());
    }

    private S_While ParseSWhile() {
        // While (e) b
        Expect(WHILE);  

        Expect(L_PAREN);
        Expr e = ParseExpression();
        Expect(R_PAREN);
        
        return new S_While(e, ParseBlock());
    }

    private S_Cond ParseSCond() {
        // If (e) b (Else b)
        
        Expect(IF);

        Expect(L_PAREN);
        Expr e = ParseExpression();
        Expect(R_PAREN);
        
        B_Block b1 = ParseBlock();
        
        // check for else, if not empty block
        B_Block b2;
        if (Check(ELSE)) {
            Expect(ELSE);
            b2 = ParseBlock();
        } 
        else {
            b2 = new B_Block([]);
        }

        return new S_Cond(e, b1, b2);
    }

    private S_Return ParseSRet() {
        // Ret e
        Expect(RET);
        return new S_Return(ParseExpression());
    }

    private S_Print ParseSPrint() {
        // Print e
        Expect(PRINT);
        return new S_Print(ParseExpression());
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
    private Expr ParseExpression() {
        if (Check(L_SQUARE))
        {
            return ParseList();
        }

        Expr left = ParseBinary();

        // e
        if (Check(DOLLAR))
        {
            Expect(DOLLAR);
            return new E_Index(left, ParseExpression());
        }

        if (Check(QUESTION))
        {
            // e ? e1 : e2
            Expect(QUESTION);
            Expr e1 = ParseExpression();
            Expect(COLON);
            Expr e2 = ParseExpression();
            
            return new E_Cond(left, e1, e2);
        }

        return left;
    }

    private Expr ParseList()
    {
        Expect(L_SQUARE);
        List<Expr> exprs = [];
        while(!Check(R_SQUARE))
        {
            exprs.Add(ParseExpression());
            if (!Check(COMMA))
            {
                break;
            }
            else { Expect(COMMA); }
        }
        Expect(R_SQUARE);
        return new E_List(exprs);
    }

    private Expr ParseBinary(int minPrec = 1) {
        // e (op e)
        Expr left = ParseUnary();
        
        while (BinOps.ContainsKey(Peek().Type) && BinOps[Peek().Type].Prec >= minPrec)
        {
            // build op with correct precedence
            (int prec, Operation op) = BinOps[Peek().Type];
            Advance();
            left = new E_Op(left, op, ParseBinary(prec + 1));
        }

        // e (op e)
        return left;
    }

    private Expr ParseUnary() {
        // (-)e
        
        if (Check(SUB)){
            Advance();
            return new E_UnaryNegate(ParseUnary());
        }

        return ParsePostfix();
    }

    private Expr ParsePostfix() {
        // e((args)|.f)
        Expr e = ParsePrimary();
        while (true) {
            // application
            if (Check(L_PAREN))
                e = new E_Appl(e, ParseArgs());
            // field access
            else if (Check(DOT)) {
                Advance();
                e = new E_Field(e, Expect(IDENT).Lexeme!);
            }
            
            else return e;
        }
    }

    private Expr ParsePrimary() {
        // literals, x, (e), New R(...), Lambda   (tightest)
        Token tok = Advance();

        switch (tok.Type) {
            case INT_LIT:   return new E_Lit(ulong.Parse(tok.Lexeme!));
            case REAL_LIT:  return new E_Lit(double.Parse(tok.Lexeme!));
            case TRUE_LIT:  return new E_Lit(true);
            case FALSE_LIT: return new E_Lit(false);
            case IDENT:     return new E_Var(tok.Lexeme!);
            
            case L_PAREN:
                // (new expression)
                Expr e = ParseExpression();
                Expect(R_PAREN);
                return e;
           
            case NEW: return new E_RecCon(Expect(IDENT).Lexeme!, ParseArgs());
           
            case LAMBDA:
                // (x1:t1, ...) : t => e
                Expect(L_PAREN);
                var _params = ParseParams();
                Expect(R_PAREN);
                
                Expect(COLON);
                
                var retType = ParseType();
                
                Expect(FAT_ARROW);
                
                return new E_Lambda(_params, retType, ParseExpression());
           
            default: throw new UnexpectedStartException(tok.Type, "primary",pos);
        }
    }
    #endregion
}
