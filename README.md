# `DateTimeDebug`

A pausable clock for C#

## Overview

`DateTimeDebug` is useful for unit-testing code that uses `DateTime.Now`.
 The only changes required in the tested code are substitutions of `DateTime.Now`
 with `DateTimeDebug.Now`.
 Unit tests that use `DateTimeDebug.Now` are fast
 because they simulate the passage of time without actually waiting,
 and they are easier to write because they aren't subject to variations
 in elapsed time caused by outside influences.

`DateTimeDebug` can also make interactive debugging easier.
This use case requires the use of debugger hooks to cause `DateTimeDebug` to be paused
while the program being debugged is paused.
Apparently there are no such debugger hooks for C# debugging as of this writing,
but if you're using C# to build a simulation with its own debugger,
your simulation can provide and use its own hooks.

`DateTimeDebug` is not a data type; all `public` members are `static`.

## The `Now` property

- `Now` is a `DateTime` value, initially identical to the value of `DateTime.Now`,
  possibly paused, possibly lagging behind `DateTime.Now`.
- The behavior of `Now` can be influenced by the other properties.
- Changing `Now` affects `Lag`.

## The `Running` property

- `Running` is the running/paused state of `Now`.
- `Running` is `true` by default.
- Changing `Running` to `false` causes `Now` to pause and make no further progress.
- Changing `Running` to `true` causes `Now` to resume, lagging behind `DateTime.Now` 
  by the accumulated `Lag` caused by pausing.

## The `Lag` property

- `Lag` is the amount of time that `Now` lags behind `DateTime.Now`.
- `Lag` is `0` by default.
- The behavior of `Lag` can be influenced by the other properties.
- Changing `Lag` affects `Now`.

The code:
- [DateTimeDebug.cs](TimeLibrary/DateTimeDebug.cs)
- [Demo.cs](Demo/Demo.cs)
  (called by
  [Test.cs](Testing/Test.cs) and
  [Program.cs](ConsoleApp/Program.cs))
