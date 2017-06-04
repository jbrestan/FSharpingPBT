namespace Examples

module Math =

    let add x y = x + y

open FsCheck
open FsCheck.Xunit

open Math

module MathTests =

    [<Property(QuietOnSuccess = true)>]
    let ``Addition is commutative`` x y =
        add x y = add y x

    [<Property(QuietOnSuccess = true)>]
    let ``Addition is associative`` x y z =
        add x (add y z) = add (add x y) z

    [<Property(QuietOnSuccess = true)>]
    let ``Zero is neutral element`` x =
        add x 0 = x
