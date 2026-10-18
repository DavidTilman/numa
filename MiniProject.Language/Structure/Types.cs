namespace MiniProject.Language.Structure;
abstract record Type;
sealed record T_Nat : Type
{
    public override string ToString()
    {
        return "Nat";
    }
}
sealed record T_Int : Type
{
    public override string ToString()
    {
        return "Int";
    }
}
sealed record T_Real : Type
{
    public override string ToString()
    {
        return "Real";
    }
}
sealed record T_Bool : Type
{
    public override string ToString()
    {
        return "Bool";
    }
}
sealed record T_Func(List<Type> ParamTypes, Type ReturnType) : Type
{
    public override string ToString()
    {
        return $"Func ({string.Join(", ", ParamTypes)}) -> {ReturnType}";
    }
}
sealed record T_Record(string Name) : Type
{
    public override string ToString()
    {
        return Name;
    }
}