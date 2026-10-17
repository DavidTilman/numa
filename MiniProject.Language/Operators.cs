namespace MiniProject.Language;

abstract record Operation;

sealed record O_Plus(Expr E1, Expr E2) : Operation;
sealed record O_Sub(Expr E1, Expr E2) : Operation;
sealed record O_Mult(Expr E1, Expr E2) : Operation;
sealed record O_Div(Expr E1, Expr E2) : Operation;
sealed record O_Mod(Expr E1, Expr E2) : Operation;
sealed record O_LT(Expr E1, Expr E2) : Operation;
sealed record O_LEQ(Expr E1, Expr E2) : Operation;
sealed record O_EQ(Expr E1, Expr E2) : Operation;
sealed record O_NEQ(Expr E1, Expr E2) : Operation;
sealed record O_GET(Expr E1, Expr E2) : Operation;
sealed record O_GT(Expr E1, Expr E2) : Operation;
sealed record O_OR(Expr E1, Expr E2) : Operation;
sealed record O_AND(Expr E1, Expr E2) : Operation;
