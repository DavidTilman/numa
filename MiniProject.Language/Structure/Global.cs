namespace MiniProject.Language.Structure;

sealed record Param(string Name, Type Type);


abstract record Outcome;
sealed record O_Normal : Outcome; 
sealed record O_Ret(Value Value) : Outcome; 