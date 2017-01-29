#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string}

    type Result<'value, 'error> =
        | Success of 'value
        | Failure of 'error

    let validate r =
        if r.name = "" then Failure "Please provide a name"
        elif r.email = "" then Failure "Let us know your e-mail"
        else Success r

    let persistAndUpdate r =
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                Success { r with id = Some id }
            with
                | ex -> Failure "Unable to save reservation"
        with
            | ex -> Failure "DB error"

    let sendNotification r =
        Smtp.send r.email
        r

    let bind binder choice =
        match choice with
        | Success v -> binder v
        | Failure message -> Failure message

    let map mapping choice =
        match choice with
        | Success v -> mapping v |> Success
        | Failure message -> Failure message

    let respond result =
        match result with
            | Success r -> WebApi.Json r
            | Failure message -> WebApi.InternalServerError message

    let createReservation =
        validate
        >> bind persistAndUpdate
        >> map sendNotification
        >> respond
