namespace MiniProject.Language.Exceptions;

using MiniProject.Language.Structure;
using Type = MiniProject.Language.Structure.Type;
public abstract class TypeException(string innerMessage) : Exception($"Type Exception: {innerMessage}")
{
}

public sealed class OperationNotDefinedException : TypeException
{
    public OperationNotDefinedException(Operation op, Type t1, Type t2) : base($"{op} not defined on operands {(t1 is T_Record r1 ? r1.Name : t1)}, {(t2 is T_Record r2 ? r2.Name : t2)}") {}
}

public sealed class NotWellTypedException : TypeException
{
    public NotWellTypedException(Expr e) : base($"expression {e} is not well typed.") {}
    public NotWellTypedException(Stmt s) : base($"statement {s} is not well typed.") {}
    public NotWellTypedException(B_Block b) : base($"block {b} is not well typed.") {}
}

public sealed class UnBoundVarException : TypeException
{
    public UnBoundVarException(string var) : base($"Undeclared variable {var}") { }
}

public sealed class UndefinedTypeException : TypeException
{
    public UndefinedTypeException(string var) : base($"Undeclared type {var}") { }
}

public sealed class InternalException : TypeException
{
    public InternalException(string msg) : base($"INTERNAL: {msg}") { }
}

public sealed class UndefinedUnaryNegationException : TypeException
{
    public UndefinedUnaryNegationException(Expr e) : base($"Undefined unary negation on {e}.") { }
}

public sealed class UnexpectedTypeException : TypeException
{
    public UnexpectedTypeException(Type ex, Type got) : base($"Expected {ex} got {got}.") { }
}

public sealed class TypeMismatchException : TypeException
{
    public TypeMismatchException(Type t1, Type t2) : base($"{t1} and {t2} must be same or equivalent.") { }
}

public sealed class InvalidCallException : TypeException
{
    public InvalidCallException(int expected, int got) : base($"Expected {expected} args, got {got}.") { }
}