namespace MiniProject.Language;

abstract record Type;

sealed record TNat : Type;
sealed record TInt : Type;
sealed record TReal : Type;
sealed record TBool : Type;
sealed record TFunc(List<Type> ParamTypes, Type ReturnType) : Type;
sealed record TRecord(string Name) : Type;