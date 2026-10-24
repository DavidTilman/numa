namespace MiniProject.Language.Exceptions;

using MiniProject.Language.Lexing;
public abstract class ParseException(string innerMessage) : Exception($"Parse Exception: {innerMessage}")
{
}

public sealed class InvalidParseException : ParseException
{
    public InvalidParseException(Type attempted, Type expected) : base($"Attempted to parse {attempted.Name} as {expected.Name}.") {}
}

public sealed class UnexpectedTokenException : ParseException
{
    public UnexpectedTokenException(TokenType expected, TokenType found, int pos) : base($"Expected {expected} but found {found} (tok={pos}).") {}
}

public sealed class UnexpectedStartException : ParseException
{
    public UnexpectedStartException(TokenType unexpected, string parsing, int pos) : base($"Unexpected {unexpected} at start of {parsing} (tok={pos}).") {}
}