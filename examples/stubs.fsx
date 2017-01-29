namespace Stubs

open System

module Db =
    type Connection () =
        interface IDisposable with
            member __.Dispose() = ()

    let connect () = new Connection ()

    let persistEntity (_: Connection) _ =
        1

module Smtp =
    let send _ = ()

module WebApi =
    let Json _ = ()

    let BadRequest _ = ()

    let InternalServerError _ = ()
