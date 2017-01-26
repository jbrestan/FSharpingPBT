- title : ROP
- description : Railway Oriented Programming @ FSharping
- author : Honza Brestan
- theme : night
- transition : default

***

### Railway Oriented Programming

Functional approach to error handling

***

### Happy path

Surely this is all that can happen...

---

### Imperative happy path

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation)
    {
        Validate(reservation);
        PersistAndUpdate(reservation);
        SendNotification(reservation);
        return Json(reservation);
    }

---

### Functional happy path

    let createReservation reservation =
        validate reservation
        |> persistAndUpdate
        |> sendNotification
        |> Json

---

### ...alternatively point-free

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
- other things can go wrong

---

### Imperative real path

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation) {
        Validate(reservation);
        PersistAndUpdate(reservation);
        SendNotification(reservation);
        return Json(reservation);
    }

---

### Imperative real path

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

### Imperative real path

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation)
    {
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

### Imperative real path

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation)
    {
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

### Imperative real path

    [lang=cs]
    public IHttpActionResult CreateReservation(ReservationDTO reservation)
    {
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

***

### What does any of this have to do with railways?

---

todo image

    f: 'a -> 'b
    g: 'b -> 'c

---

todo image

    f >> g: 'a -> 'c

***

#
