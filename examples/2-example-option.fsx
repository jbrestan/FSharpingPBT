#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string }

    let validate r =
        if r.name = "" then None
        elif r.email = "" then None
        else Some r

    let persistAndUpdate r =
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                Some { r with id = Some id }
            with
                | ex -> None
        with
            | ex -> None

    let sendNotification r =
        Smtp.send r.email
        r

    let respond ``r option`` =
        match ``r option`` with
            | Some r -> WebApi.Json r
            | None -> WebApi.InternalServerError ""

    let createReservation =
        validate
        >> Option.bind persistAndUpdate
        >> Option.map sendNotification
        >> respond
