#load "stubs.fsx"

open System
open Stubs

module Result =
    type Result<'value, 'error> =
        | Success of 'value
        | Failure of 'error

    let bind binder result =
        match result with
        | Success v -> binder v
        | Failure message -> Failure message

    let map mapping result =
        match result with
        | Success v -> mapping v |> Success
        | Failure message -> Failure message

    type ResultBuilder () =
        member inline this.Bind (m, f) = bind f m
        member inline this.Return a = Success a
        member inline this.ReturnFrom m = m
        member inline this.Delay(f) = f
        member inline this.Run(r) = r ()
        member inline this.TryWith(body, handler) =
            try this.ReturnFrom(body ())
            with e -> handler e
        member inline this.TryFinally(body, compenstaion) =
            try this.ReturnFrom(body ())
            finally compenstaion ()
        member inline this.Using(d:#IDisposable, body) =
            let body' = fun () -> body d
            this.TryFinally(body', fun () ->
                match d with
                | null -> ()
                | disp -> disp.Dispose())

    let result = ResultBuilder ()

module Example =
    open Result

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

    let validate r = result {
        if r.name = "" then
            return! Failure <| ValidationError EmptyNameError
        elif r.email = "" then
            return! Failure <| ValidationError EmptyEmailError
        else
            return r
    }

    let persistAndUpdate r = result {
        try
            use conn = Db.connect ()
            try
                let id = Db.persistEntity conn r
                return { r with id = Some id }
            with
                | ex -> return! Failure <| UpdateError DbUpdateError
        with
            | ex -> return! Failure <| UpdateError DbConnectionError
    }

    let sendNotification r =
        Smtp.send r.email
        r

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

    let sendNotificationR = map sendNotification

    let createReservation =
        validate
        >> persistAndUpdateR
        >> sendNotificationR
        >> respond
