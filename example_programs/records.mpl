Record Point (x : Int, y : Int);
Record Rectangle (topLeft : Point, bottomRight : Point);

Func area (r : Rectangle) : Int = {
    Ret (r.bottomRight.x - r.topLeft.x) * (r.topLeft.y - r.bottomRight.y);
};

Var p    : Point     = New Point(1, 2);
Var q    : Point     = New Point(1, 2);
Var rect : Rectangle = New Rectangle(New Point(0, 4), New Point(3, 0));

Print p.x + p.y;
Print p == q;
Print area(rect);
