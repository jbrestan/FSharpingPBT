namespace Examples

type Examples() =
    member this.X = "F#"

module Tests =
    open Xunit
    open FsCheck.Xunit
    open Swensen.Unquote

    [<Fact>]
    let ``This should pass`` () = 
        test <@ true @>
