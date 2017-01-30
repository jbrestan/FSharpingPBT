- title : ROP
- description : Railway Oriented Programming @ FSharping
- author : Honza Brestan
- theme : simple
- transition : default

***

### Railway Oriented Programming

Functional approach to error handling

***

### Happy path

Surely this is all that can happen...

---

### Imperative example

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        Validate(reservation);
        PersistAndUpdate(reservation);
        SendNotification(reservation);
        return Json(reservation);
    }

---

### Functional example

    let createReservation reservation =
        validate reservation
        |> persistAndUpdate
        |> sendNotification
        |> Json

---

### Let's aim for point-free

    let createReservation =
        validate
        >> persistAndUpdate
        >> sendNotification
        >> Json

***

### But we can't have nice things

- Validations fail
- DB connections drop
- SMTP servers get overloaded

---

### Imperative

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        Validate(reservation);
        PersistAndUpdate(reservation);
        SendNotification(reservation);
        return Json(reservation);
    }

---

### Imperative

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        var validated = Validate(reservation);
        if (!validated) {
            return BadRequest("Reservation invalid!");
        }
        PersistAndUpdate(reservation);
        SendNotification(reservation);
        return Json(reservation);
    }

---

### Imperative

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        var validated = Validate(reservation);
        if (!validated) {
            return BadRequest("Reservation invalid!");
        }
        var updatedReservation = PersistAndUpdate(reservation);
        if (updatedReservation == null) {
            return BadRequest("Unable to persist reservation!");
        }
        SendNotification(updatedReservation);
        return Json(reservation);
    }

---

### Imperative

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        var validated = Validate(reservation);
        if (!validated) {
            return BadRequest("Reservation invalid!");
        }
        try {
            var updatedReservation = PersistAndUpdate(reservation);
            if (updatedReservation == null) {
                return BadRequest("Unable to update reservation!");
            }
        } catch {
            return InternalServerError("DB error: unable to persist reservation!");
        }
        SendNotification(updatedReservation);
        return Json(updatedReservation);
    }

---

### Imperative

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        var validated = Validate(reservation);
        if (!validated) {
            return BadRequest("Reservation invalid!");
        }
        try {
            var updatedReservation = PersistAndUpdate(reservation);
            if (updatedReservation == null) {
                return BadRequest("Unable to update reservation!");
            }
        } catch {
            return InternalServerError("DB error: unable to persist reservation!");
        }
        try {
            SendNotification(updatedReservation);
        } catch {
            log.Error("Confirmation not sent!");
        }
        return Json(updatedReservation);
    }

---

### Functional?

    let createReservation =
        validate
        >> persistAndUpdate
        >> sendNotification
        >> respond

***

### What does any of this have to do with railways?

---

    let f: 'a -> 'b = ...

![Single-track function](images/f1.png)

---

    let f: 'a -> 'b = ...
    let g: 'b -> 'c = ...

![Two single-track functions](images/f1f2.png)

---

    let f: 'a -> 'b = ...
    let g: 'b -> 'c = ...
    let h = f >> g

![Single-track function composition](images/f1compf2.png)

---

    let h: 'a -> 'c = f >> g

![Single-track function composition done](images/f3.png)

---

### Switches

![Switch](images/switch.png)

---

    let validate: Reservation -> Reservation option = ...
    let persist:  Reservation -> Reservation option = ...

![Switch composition](images/switchcomp.png)

---

    let validate: Reservation        -> Reservation option = ...
    let persist': Reservation option -> Reservation option = ...

![Switches composed](images/switchcomposed.png)

---

    let persist': Reservation option -> Reservation option = 
        Option.bind persist

![Bind adapter](images/bind.png)

    let bind binder option =
        match option with
        | Some value -> binder value
        | None -> None

---

    let sendNotification': Reservation option -> Reservation option = 
        Option.map sendNotification

![Map adapter](images/map.png)

    let map mapping option =
        match option with
        | Some value -> Some (mapping value)
        | None -> None

---

### And many more...

- unit-returning functions
- exception-throwing functions
- inspections
- ...

***

### Functional "real path"

    let createReservation =
        validate
        >> persistAndUpdate
        >> sendNotification
        >> respond

***

Result<'TSuccess, 'TError> in FSharp.Core 4.1 (coming soon)

***

### Thank you!

***

### Sources

- [F# for fund and profit](http://fsharpforfunandprofit.com/rop/)
- [Chessie Result implementation](https://fsprojects.github.io/Chessie/railway.html)

#
