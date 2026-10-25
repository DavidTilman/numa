Func fact (n : Int) : Int = {
    If (n <= 0) {
        Ret 1;
    }
    Else {
        Ret n * fact(n - 1);
    };
};

Func fib (n : Int) : Int = {
    If (n < 2) {
        Ret n;
    }
    Else {
        Ret fib(n - 1) + fib(n - 2);
    };
};

Print fact(5);
Print fib(10);
