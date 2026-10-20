namespace MiniProject.Language.Lexing;

public enum TokenType
{
    // operators
    PLUS, SUB, MULT, DIV, MOD, LT, LEQ, EQ, NEQ, GEQ, GT, OR, AND,

    // types
    NAT_TYPE, INT_TYPE, REAL_TYPE, BOOL_TYPE,

    // literals
    INT_LIT, REAL_LIT, TRUE_LIT, FALSE_LIT,

    // variables
    IDENT,

    // punctuation
    L_PAREN, R_PAREN, L_BRACE, R_BRACE, COMMA, DOT, COLON, SEMICOLON, QUESTION, ARROW, FAT_ARROW, EQUALS,

    ASSIGN, VAR, WHILE, IF, ELSE, RET, PRINT, RECORD, FUNC, LAMBDA, READ, NEW, EOF
}

public record Token(TokenType Type, string? Lexeme);