#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string}

    let validate r =
        if r.name = "" then Choice2Of2 "Please provide a name"
        elif r.email = "" then Choice2Of2 "Let us know your e-mail"
        else Choice1Of2 r

    let persistAndUpdate r =
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                Choice1Of2 { r with id = Some id }
            with
                | ex -> Choice2Of2 "Unable to save reservation"
        with
            | ex -> Choice2Of2 "DB error"

    let sendNotification r =
        Smtp.send r.email
        Choice1Of2 r

    let bind binder choice =
        match choice with
        | Choice1Of2 v -> binder v
        | Choice2Of2 message -> Choice2Of2 message

    let map mapping choice =
        match choice with
        | Choice1Of2 v -> mapping v |> Choice1Of2
        | Choice2Of2 message -> Choice2Of2 message

    let respond ``r choice`` =
        match ``r choice`` with
            | Choice1Of2 r -> WebApi.Json r
            | Choice2Of2 message -> WebApi.InternalServerError message

    let createReservation =
        validate
        >> bind persistAndUpdate
        >> map sendNotification
        >> respond
