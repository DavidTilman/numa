# Example programs

Each `.mpl` file is a small, self-contained program that exercises a slice of
the language. They all parse and type-check; every one except `io.mpl` also
runs non-interactively (shown outputs below).

> The language has **no comment syntax** (`//` lexes as two `/` operators), so
> the programs themselves carry no comments — the explanations live here.

| File | Features demonstrated | Output |
|------|----------------------|--------|
| `arithmetic.mpl` | The three numeric types (`Nat <: Int <: Real`), widening, `+ - * / %`, unary negate. Literals are `Nat`; a decimal point gives `Real`; unary minus and subtraction yield at least `Int`. | `0  24.5  -1  3  -10  -49` |
| `conditionals.mpl` | Comparisons, logical `&&` / `\|\|`, the `e ? e1 : e2` ternary, and an `If/Else` statement. | `True  True  1  8` |
| `loops.mpl` | A `While` loop with assignment, summing `0..9`. | `45` |
| `recursion.mpl` | `Func` declarations and self-recursion (`fact`, `fib`). | `120  55` |
| `mutual_recursion.mpl` | `isEven` / `isOdd` calling each other — every top-level function is in scope regardless of declaration order. | `True  True` |
| `higher_order.mpl` | First-class functions: `Lambda`, function-typed parameters `(Int) -> Int`, a function **returning** a function, and closures capturing a variable. | `36  81  15  201` |
| `records.mpl` | `Record` declarations, `New` construction, (nested) field access, and record `==`. | `3  True  12` |
| `io.mpl` | `Read` a `Nat` from input, then branch (one step of the Collatz map). Interactive. | — |

## Syntax reminders

- Every statement and top-level command is **terminated** by `;` — including
  the ones that end in `}` (`While`, `If/Else`, `Func`, `Record`).
- Keywords are capitalised: `Var`, `While`, `If`, `Else`, `Ret`, `Print`,
  `Read`, `Func`, `Record`, `New`, `Lambda`, and the types `Nat` / `Int` /
  `Real` / `Bool`. Lower-case words are identifiers.
- Note: `Var x : T = e` binds `x` at the **inferred** type of `e`; the
  annotation is not currently enforced. Use `Var x : Int = Read;` (or an
  `Int`-typed initialiser such as `0 - 0`) when you need a mutable `Int`,
  because `Nat - Nat` is `Int` and would not be assignable back to a `Nat`.
