namespace Examples

module FizzBuzz =

    let transform number =
        match number % 3, number % 5 with
        | 0, 0 -> "FizzBuzz"
        | 0, _ -> "Fizz"
        | _, 0 -> "Buzz"
        | _ -> string number

open FsCheck
open FsCheck.Xunit

module FizzBuzzTests =

    [<Property(QuietOnSuccess = true)>]
    let ``FizzBuzz.transform returns number`` (number : int) =
        (number % 3 <> 0 && number % 5 <> 0) ==> lazy
        let actual = FizzBuzz.transform number
        let expected = number.ToString()
        expected = actual

    [<Property(QuietOnSuccess = true)>]
    let ``FizzBuzz.transform returns Fizz`` (number : int) =
        (number % 3 = 0 && number % 5 <> 0) ==> lazy
        let actual = FizzBuzz.transform number
        let expected = "Fizz"
        expected = actual


    [<Property(QuietOnSuccess = true)>]
    let ``FizzBuzz.transform returns Buzz`` (number : int) =
        (number % 5 = 0 && number % 3 <> 0) ==> lazy
        let actual = FizzBuzz.transform number
        let expected = "Buzz"
        expected = actual
    
    [<Property(QuietOnSuccess = true)>]
    let ``FizzBuzz.transform returns FizzBuzz`` () =
        let fiveAndThrees =
            Arb.generate<int> |> Gen.map ((*) (3 * 5)) |> Arb.fromGen
        Prop.forAll fiveAndThrees <| fun number ->

            let actual = FizzBuzz.transform number

            let expected = "FizzBuzz"
            expected = actual
