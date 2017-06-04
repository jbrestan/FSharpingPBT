- title : PBT
- description : Property-Based Testing @ FSharping
- author : Honza Brestan
- theme : simple
- transition : default

***

### Property-Based Testing with FsCheck

***

### Testing examples

---

- Hardcoded input
- Hardcoded expected output
- `Assert.AreEqual(expected, actual)`

---

### Testing examples

![Testing hardcoded examples](images/example-testing.png)

---

### Testing examples

![Testing hardcoded examples](images/example-testing2.png)

***

### Testing properties

![Testing properties](images/test-boundaries.png)

---

1. Write speficication using properties
2. ???
3. Profit!

---

How?

---

Edge Cases


v


Equivalence Classes

---

Hardcoded input


v


Random input (within an equivalence class)

--- 

Hardcoded "magic" output


v


Inverse operations, idempotence, commutativity...

***

![FsCheck](images/fscheck-logo.png)

---

### FsCheck

- Randomized testing library
- Built-in generators for many types
- Can easily generate complex values too
- Standalone or integrated with many testing libs

---

- Generate input
- Execute test
- Verify properties

---

Arbitrary = Generator + Shrinker

- Generate -> find error -> shrink

---

Advanced features

- Value classification
- Model-based testing

---

https://github.com/fscheck/FsCheck

***

### Thank you!
