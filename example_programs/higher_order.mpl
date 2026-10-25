Func applyTwice (f : (Int) -> Int, x : Int) : Int = {
    Ret f(f(x));
};

Func adder (n : Int) : (Int) -> Int = {
    Ret Lambda (m : Int) : Int => m + n;
};

Var square  : (Int) -> Int = Lambda (n : Int) : Int => n * n;
Var addFive : (Int) -> Int = adder(5);

Print square(6);
Print applyTwice(square, 3);
Print addFive(10);
Print applyTwice(adder(100), 1);
