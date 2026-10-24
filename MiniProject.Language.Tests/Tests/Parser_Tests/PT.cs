using MiniProject.Language.Lexing;
using MiniProject.Language.Structure;
using MiniProject.Language.Parsing;
using MiniProject.Language.Exceptions;
using static MiniProject.Language.Lexing.TokenType;
using Type = MiniProject.Language.Structure.Type;

namespace MiniProject.Language.Tests;

// Shared helpers for the parser tests. The parser consumes a token list (there
// is no lexer), so each test hands it an exact token stream and inspects the
// AST it produces. EOF is appended automatically.
internal static class PT
{
    // ---- running the parser -------------------------------------------------
    public static P_Prog ParseP(params Token[] toks) => new Parser().Parse([.. toks, Eof]);

    // Parse a single expression by wrapping it as `Print <toks> ;`.
    public static Expr ParseE(params Token[] toks)
    {
        P_Prog p = ParseP([Print_, .. toks, Semi]);
        return ((S_Print)((C_Stmt)p.Commands[0]).Stmt).Expr;
    }

    // Parse a single statement by appending its terminating `;`.
    public static Stmt ParseS(params Token[] toks)
        => ((C_Stmt)ParseP([.. toks, Semi]).Commands[0]).Stmt;

    // Parse a single declaration (Record/Func) by appending its terminating `;`.
    public static Decl ParseD(params Token[] toks)
        => ((C_Decl)ParseP([.. toks, Semi]).Commands[0]).Decl;

    public static void Rejects(params Token[] toks) => Assert.ThrowsAny<ParseException>(() => ParseP(toks));

    public static void Rejects<T>(params Token[] toks) where T : ParseException
        => Assert.Throws<T>(() => ParseP(toks));

    // ---- tokens -------------------------------------------------------------
    private static Token Tok(TokenType t) => new(t, null);

    public static Token Id(string name) => new(IDENT, name);
    public static Token Num(string digits) => new(INT_LIT, digits);
    public static Token RealLit(string digits) => new(REAL_LIT, digits);

    public static readonly Token
        LP = Tok(L_PAREN), RP = Tok(R_PAREN), LB = Tok(L_BRACE), RB = Tok(R_BRACE),
        Comma = Tok(COMMA), Dot = Tok(DOT), Colon = Tok(COLON), Semi = Tok(SEMICOLON),
        Q = Tok(QUESTION), Arrow = Tok(ARROW), FatArrow = Tok(FAT_ARROW), Eq_ = Tok(EQUALS),
        Assign = Tok(ASSIGN), Eof = Tok(EOF),
        Var_ = Tok(VAR), While_ = Tok(WHILE), If_ = Tok(IF), Else_ = Tok(ELSE),
        Ret_ = Tok(RET), Print_ = Tok(PRINT), Record_ = Tok(RECORD), Func_ = Tok(FUNC),
        Lambda_ = Tok(LAMBDA), Read_ = Tok(READ), New_ = Tok(NEW),
        NatT = Tok(NAT_TYPE), IntT = Tok(INT_TYPE), RealT = Tok(REAL_TYPE), BoolT = Tok(BOOL_TYPE),
        Plus = Tok(PLUS), Sub = Tok(SUB), Mult = Tok(MULT), Div = Tok(DIV), Mod = Tok(MOD),
        Lt = Tok(LT), Leq = Tok(LEQ), Gt = Tok(GT), Geq = Tok(GEQ), EqOp = Tok(EQ), Neq = Tok(NEQ),
        And = Tok(AND), Or = Tok(OR), True_ = Tok(TRUE_LIT), False_ = Tok(FALSE_LIT);

    // ---- expected-AST shorthands (leaves, so Assert.Equal compares them) -----
    public static Expr ENat(ulong n) => new E_Lit(n);
    public static Expr EReal(double d) => new E_Lit(d);
    public static Expr EBool(bool b) => new E_Lit(b);
    public static Expr EVar(string x) => new E_Var(x);
    public static Expr EOp(Expr l, Operation op, Expr r) => new E_Op(l, op, r);
    public static Expr ENeg(Expr e) => new E_UnaryNegate(e);
    public static Expr EField(Expr t, string f) => new E_Field(t, f);
    public static Expr ECond(Expr c, Expr t, Expr f) => new E_Cond(c, t, f);

    public static readonly Type TNat = new T_Nat();
    public static readonly Type TInt = new T_Int();
    public static readonly Type TReal = new T_Real();
    public static readonly Type TBool = new T_Bool();
}
