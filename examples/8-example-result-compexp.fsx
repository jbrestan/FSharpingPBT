#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string }

    type ReservationError<'a,'b> =
        | ValidationError of 'a
        | UpdateError of 'b

    type ValidationError =
        | EmptyNameError
        | EmptyEmailError

    type UpdateError =
        | DbUpdateError
        | DbConnectionError

    type Result<'value, 'error> =
        | Success of 'value
        | Failure of 'error

    let validate r =
        if r.name = "" then Failure <| ValidationError EmptyNameError
        elif r.email = "" then Failure <| ValidationError EmptyEmailError
        else Success r

    let persistAndUpdate r =
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                Success { r with id = Some id }
            with
                | ex -> Failure <| UpdateError DbUpdateError
        with
            | ex -> Failure <| UpdateError DbConnectionError

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

    let respondToValidationError ve =
        match ve with
        | EmptyNameError -> WebApi.InternalServerError "Please provide a name"
        | EmptyEmailError -> WebApi.InternalServerError "Let us know your e-mail"

    let respondToUpdateError ue =
        match ue with
        | DbUpdateError -> WebApi.InternalServerError "Unable to save reservation"
        | DbConnectionError -> WebApi.InternalServerError "DB error"

    let respondToError e =
        match e with
        | ValidationError ve -> respondToValidationError ve
        | UpdateError ue -> respondToUpdateError ue

    let respond result =
        match result with
            | Success r -> WebApi.Json r
            | Failure message -> respondToError message

    let persistAndUpdateR = bind persistAndUpdate

    let sendNotificationR = (map sendNotification)

    let createReservation =
        validate
        >> persistAndUpdateR
        >> sendNotificationR
        >> respond
