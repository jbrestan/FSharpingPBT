#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string }

    type ReservationError =
        | EmptyNameError
        | EmptyEmailError
        | DbUpdateError
        | DbConnectionError

    type Result<'value, 'error> =
        | Success of 'value
        | Failure of 'error

    let validate r =
        if r.name = "" then Failure EmptyNameError
        elif r.email = "" then Failure EmptyEmailError
        else Success r

    let persistAndUpdate r =
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                Success { r with id = Some id }
            with
                | ex -> Failure DbUpdateError
        with
            | ex -> Failure DbConnectionError

    let sendNotification r =
        Smtp.send r.email
        r

    let bind binder result =
        match result with
        | Success v -> binder v
        | Failure message -> Failure message

    let map mapping result =
        match result with
        | Success v -> mapping v |> Success
        | Failure message -> Failure message

    let respond result =
        match result with
            | Success r -> WebApi.Json r
            | Failure message ->
                match message with
                | EmptyNameError -> WebApi.InternalServerError "Please provide a name"
                | EmptyEmailError -> WebApi.InternalServerError "Let us know your e-mail"
                | DbUpdateError -> WebApi.InternalServerError "Unable to save reservation"
                | DbConnectionError -> WebApi.InternalServerError "DB error"

    let createReservation =
        validate
        >> bind persistAndUpdate
        >> map sendNotification
        >> respond
