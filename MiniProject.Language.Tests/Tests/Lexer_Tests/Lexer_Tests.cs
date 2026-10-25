using MiniProject.Language.Lexing;
using static MiniProject.Language.Lexing.TokenType;

namespace MiniProject.Language.Tests;

// The lexer turns source text into a token list, terminated by a single EOF.
// Each test lexes a snippet and inspects the tokens it produces.
public class Lexer_Tests
{
    private static List<Token> Lex(string src) => new Lexer().Lex(src);

    // Token types, excluding the trailing EOF the lexer always appends.
    private static TokenType[] Types(string src)
    {
        List<Token> toks = Lex(src);
        return toks.Take(toks.Count - 1).Select(t => t.Type).ToArray();
    }

    // ---- EOF -----------------------------------------------------------------

    [Fact]
    public void Empty_input_yields_just_eof()
        => Assert.Equal([new Token(EOF, "")], Lex(""));

    [Fact]
    public void Whitespace_only_yields_just_eof()
        => Assert.Equal([new Token(EOF, "")], Lex("  \t\n  "));

    [Fact]
    public void Stream_is_terminated_by_eof()
        => Assert.Equal(EOF, Lex("1").Last().Type);

    // ---- single symbols ------------------------------------------------------

    [Theory]
    [InlineData("+", PLUS)]
    [InlineData("-", SUB)]
    [InlineData("*", MULT)]
    [InlineData("/", DIV)]
    [InlineData("%", MOD)]   // regression: '%' once mapped to DIV
    [InlineData("<", LT)]    // regression: '<' once mapped to LEQ
    [InlineData(">", GT)]    // regression: '>' once mapped to GEQ
    [InlineData("(", L_PAREN)]
    [InlineData(")", R_PAREN)]
    [InlineData("{", L_BRACE)]
    [InlineData("}", R_BRACE)]
    [InlineData(",", COMMA)]
    [InlineData(".", DOT)]
    [InlineData(":", COLON)]
    [InlineData(";", SEMICOLON)]
    [InlineData("?", QUESTION)]
    [InlineData("=", EQUALS)]
    public void Single_symbol_maps_to_its_token(string src, TokenType expected)
        => Assert.Equal([expected], Types(src));

    // ---- double symbols (maximal munch) --------------------------------------

    [Theory]
    [InlineData("<=", LEQ)]
    [InlineData("==", EQ)]
    [InlineData("!=", NEQ)]
    [InlineData(">=", GEQ)]
    [InlineData("||", OR)]
    [InlineData("&&", AND)]
    [InlineData("->", ARROW)]
    [InlineData("=>", FAT_ARROW)]
    [InlineData(":=", ASSIGN)]
    public void Double_symbol_maps_to_its_token(string src, TokenType expected)
        => Assert.Equal([expected], Types(src));

    [Fact]
    public void Double_symbols_win_over_their_single_prefix()
    {
        // Each of these must NOT split into two single-symbol tokens.
        Assert.Equal([ASSIGN], Types(":="));      // not COLON, EQUALS
        Assert.Equal([EQ], Types("=="));           // not EQUALS, EQUALS
        Assert.Equal([FAT_ARROW], Types("=>"));    // not EQUALS, GT
        Assert.Equal([ARROW], Types("->"));        // not SUB, GT
        Assert.Equal([LEQ], Types("<="));          // not LT, EQUALS
        Assert.Equal([GEQ], Types(">="));          // not GT, EQUALS
    }

    [Fact]
    public void Single_symbol_at_end_of_input_still_lexes()
    {
        // '<' is the last char, so Next is '\0' and no double match is possible.
        Assert.Equal([IDENT, LT], Types("x<"));
    }

    // ---- literals ------------------------------------------------------------

    [Fact]
    public void Integer_literal_keeps_its_digits()
        => Assert.Equal(new Token(INT_LIT, "42"), Lex("42")[0]);

    [Fact]
    public void Real_literal_keeps_its_digits()
        => Assert.Equal(new Token(REAL_LIT, "3.14"), Lex("3.14")[0]);

    [Fact]
    public void Dot_after_a_whole_number_is_a_separate_token()
    {
        // The real only swallows one dot; a second dot stays a DOT token.
        Assert.Equal([REAL_LIT, DOT, INT_LIT], Types("1.2.3"));
    }

    // ---- identifiers and keywords --------------------------------------------

    [Fact]
    public void Lowercase_word_is_an_identifier()
        => Assert.Equal(new Token(IDENT, "foo"), Lex("foo")[0]);

    [Theory]
    [InlineData("Nat", NAT_TYPE)]
    [InlineData("Int", INT_TYPE)]
    [InlineData("Real", REAL_TYPE)]
    [InlineData("Bool", BOOL_TYPE)]   // regression: 'Bool' once lexed as IDENT
    [InlineData("True", TRUE_LIT)]
    [InlineData("False", FALSE_LIT)]
    [InlineData("New", NEW)]
    [InlineData("Lambda", LAMBDA)]
    [InlineData("Var", VAR)]
    [InlineData("Read", READ)]
    [InlineData("While", WHILE)]
    [InlineData("If", IF)]
    [InlineData("Else", ELSE)]
    [InlineData("Ret", RET)]
    [InlineData("Print", PRINT)]
    [InlineData("Record", RECORD)]
    [InlineData("Func", FUNC)]
    public void Keyword_maps_to_its_token(string src, TokenType expected)
        => Assert.Equal([expected], Types(src));

    [Fact]
    public void Keywords_are_case_sensitive()
    {
        // Only the exact spelling is a keyword; others are identifiers.
        Assert.Equal(new Token(IDENT, "nat"), Lex("nat")[0]);
        Assert.Equal(new Token(IDENT, "WHILE"), Lex("WHILE")[0]);
    }

    // ---- whitespace ----------------------------------------------------------

    [Fact]
    public void Whitespace_separates_but_is_discarded()
        => Assert.Equal([INT_LIT, PLUS, INT_LIT], Types("1 + \t 2\n"));

    [Fact]
    public void Adjacent_tokens_need_no_whitespace()
        => Assert.Equal([L_PAREN, INT_LIT, PLUS, INT_LIT, R_PAREN], Types("(1+2)"));

    // ---- regression: scratchpad was not reset between symbol tokens ----------

    [Fact]
    public void Assignment_does_not_leak_a_phantom_colon()
    {
        // Before the fix, ':=' left ":=" in the scratchpad and the following
        // token was lexed from stale characters, emitting a spurious COLON.
        Assert.Equal([IDENT, ASSIGN, IDENT], Types("a := b"));
        Assert.Equal([IDENT, ASSIGN, INT_LIT], Types("x:=1"));
    }

    [Fact]
    public void Back_to_back_double_symbols_do_not_corrupt_each_other()
        => Assert.Equal([ASSIGN, ASSIGN], Types(":=:="));

    [Fact]
    public void Single_symbol_does_not_leak_into_next_token()
        => Assert.Equal([SEMICOLON, IDENT], Types(";x"));

    // ---- a small program -----------------------------------------------------

    [Fact]
    public void Lexes_a_representative_statement()
    {
        TokenType[] expected =
        [
            VAR, IDENT, COLON, INT_TYPE, ASSIGN, INT_LIT, SEMICOLON,
            WHILE, IDENT, LT, INT_LIT, L_BRACE,
                IDENT, ASSIGN, IDENT, PLUS, INT_LIT, SEMICOLON,
            R_BRACE,
        ];
        Assert.Equal(expected, Types("Var x : Int := 0; While x < 10 { x := x + 1; }"));
    }

    // ---- errors --------------------------------------------------------------

    [Fact]
    public void Unknown_character_throws()
        => Assert.Throws<Exception>(() => Lex("@"));

    [Fact]
    public void Lone_bang_is_unknown()
        => Assert.Throws<Exception>(() => Lex("!"));
}
