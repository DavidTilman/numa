# MiniProject

PLPDI mini-project.

- `MiniProject.Language` — class library: the language implementation
- `MiniProject.Language.Cli` — console front end

```sh
dotnet build
dotnet run --project MiniProject.Language.Cli
```

# Scratchpad
```
    Record Vect2 (x : Int, y : Int);

    Func Dot (a : Vect2, b : Vect2) : Int = {
        ret a.x * b.x + a.y * b.y;
    };

    Func Apply (f : (Int) -> Int, n : Int) : Int = {
        ret f(n);
    };

    Var v : Vect2 = Vect2(3, 4);
    Var sq : (Int) -> Int = Fun (n : Int) : Int => n * n;

    Var total : Int = 0;
    While (total < 100) {
        total := total + Apply(sq, Dot(v, v));
    };
```
