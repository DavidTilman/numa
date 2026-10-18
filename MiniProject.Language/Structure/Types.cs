namespace MiniProject.Language.Structure;
abstract record Type;
sealed record T_Nat : Type;
sealed record T_Int : Type;
sealed record T_Real : Type;
sealed record T_Bool : Type;
sealed record T_Func(List<Type> ParamTypes, Type ReturnType) : Type;
sealed record T_Record(string Name) : Type;