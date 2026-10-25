Func isEven (n : Int) : Bool = {
    If (n == 0) {
        Ret True;
    }
    Else {
        Ret isOdd(n - 1);
    };
};

Func isOdd (n : Int) : Bool = {
    If (n == 0) {
        Ret False;
    }
    Else {
        Ret isEven(n - 1);
    };
};

Print isEven(10);
Print isOdd(7);
