#load "stubs.fsx"

open System
open Stubs

module Example =

    type Reservation =
        { id: int option
          name: string
          email: string}

    let validate r =
        r

    let persistAndUpdate r =
        use conn = Db.connect ()
        let id = Db.persistEntity conn r
        { r with id = Some id }

    let sendNotification r =
        Smtp.send r.email
        r

    let createReservation =
        validate
        >> persistAndUpdate
        >> sendNotification
        >> WebApi.Json
