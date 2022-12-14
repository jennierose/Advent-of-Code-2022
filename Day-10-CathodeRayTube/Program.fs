open System.IO
open System.Text.RegularExpressions

type Instruction =
    | AddX of int
    | NoOp

    member this.Exec(register: int) : seq<int> =
        match this with
        | AddX (n: int) ->
            seq {
                yield register
                yield register + n
            }
        | NoOp -> Seq.singleton register

    static member Parse(line: string) : option<Instruction> =
        let line': string = line.Trim().ToLower()
        let pattern: Regex = Regex @"^(?:(noop)|(addx)\s+([+-]?\d+))$"

        if pattern.IsMatch(line') then
            let groups: GroupCollection = pattern.Match(line').Groups
            let op: string = List.maxBy String.length [ groups[1].Value; groups[2].Value ]

            match op with
            | "addx" -> let n: int = int groups[3].Value in Some(AddX n)
            | "noop" -> Some NoOp
            | _ -> None
        else
            None

[<EntryPoint>]
let main (args: string []) : int =
    let instructions: Instruction list =
        File.ReadLines @"./input.txt"
        |> Seq.map Instruction.Parse
        |> Seq.filter Option.isSome
        |> Seq.map Option.get
        |> Seq.toList

    let recordCycle ((r: int, state: seq<int>)) (i: Instruction) =
        let update: seq<int> = i.Exec r in (Seq.last update, Seq.append state update)

    let cycles: seq<int> =
        List.fold recordCycle (1, Seq.singleton 1) instructions
        |> snd

    let signalStrength: int =
        Seq.take (20 + 5 * 40) cycles
        |> Seq.indexed
        |> Seq.filter (fun (i: int, _) -> (i + 1) % 40 = 20)
        |> Seq.map (fun (i: int, v: int) -> (i + 1) * v)
        |> Seq.sum

    printfn "--- Part One ---"
    printfn "Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles. What is the sum"
    printfn "of these six signal strengths?"
    printfn "Answer: %d\n" signalStrength

    printfn "--- Part Two ---"
    printfn "Render the image given by your program. What eight capital letters appear on your CRT?"
    printfn "Answer:\n"

    Seq.take (6 * 40) cycles
    |> Seq.chunkBySize 40
    |> Seq.map Seq.indexed
    |> Seq.map (
        Seq.map (fun (i: int, r: int) -> if abs (i - r) <= 1 then "#" else ".")
        >> String.concat ""
    )
    |> String.concat "\n"
    |> printfn "%s"

    0
