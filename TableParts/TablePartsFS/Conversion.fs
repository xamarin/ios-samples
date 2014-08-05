namespace TablePartsFS
open System

////Q, R, Z, I, N, or G
module NumericLiteralZ =
    let FromZero() = nfloat.op_Implicit(0)
    let FromOne() = nfloat.op_Implicit(1)
    let FromInt16 (n:int16) = nfloat.op_Implicit(n)
    let FromInt32 (n:int) = nfloat.op_Implicit(n)
    let FromInt64 (n:int64) = nfloat.op_Implicit(n)
    let FromString (n:string) = nfloat.Parse(n)

module NumericLiteralI =
    let FromZero() = nint.op_Implicit(0)
    let FromOne() = nint.op_Implicit(1)
    let FromInt16 (n:int16) = nint.op_Implicit(n)
    let FromInt32 (n:int) = nint.op_Implicit(n)
    let FromString (n:string) = nint.Parse(n)

module NumericLiteralN =
    let FromZero() = nuint.op_Implicit(0u)
    let FromOne() = nuint.op_Implicit(1u)
    let FromUInt16 (n:UInt16) = nuint.op_Implicit(n)
    let FromUInt32 (n:UInt32) = nuint.op_Implicit(n)
    let FromString (n:string) = nuint.Parse(n)


module Conversion =
    let inline implicit< ^a,^b when ^a : (static member op_Implicit : ^b -> ^a)> arg =
        (^a : (static member op_Implicit : ^b -> ^a) arg)

    let inline nint (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> nint) x)
    let inline nfloat (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> nfloat) x)
    let inline nuint (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit : ^a -> nuint) x)