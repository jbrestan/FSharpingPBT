namespace Stubs

open System

module Db =
    [<AllowNullLiteral>]
    type Connection () =
        interface IDisposable with
            member __.Dispose() = ()

    let connect () = new Connection ()

    let persistEntity (_: Connection) _ =
        1

module Smtp =
    let send _ = ()

module WebApi =
    type IHttpActionResult () = class end

    let Json _ = IHttpActionResult ()

    let BadRequest _ = IHttpActionResult ()

    let InternalServerError _ = IHttpActionResult ()
