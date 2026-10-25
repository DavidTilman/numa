Var x : Nat = 8;

Print (x < 10) && (x > 0);
Print (x == 0) || (x != 0);

Var sign : Int = (x < 5) ? -1 : 1;
Print sign;

If (x % 2 == 0) {
    Print x;
}
Else {
    Print 0 - x;
};
