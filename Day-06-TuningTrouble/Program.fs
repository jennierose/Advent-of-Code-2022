open System.IO

let findMarker (windowSize: int) (text: string) : int option =
    Seq.cast text
    |> Seq.windowed windowSize
    |> Seq.indexed
    |> Seq.map (fun (idx: int, window: char []) -> (idx, Set.count <| Set.ofArray window))
    |> Seq.tryFind (snd >> ((=) windowSize))
    |> Option.map (fst >> ((+) windowSize))

[<EntryPoint>]
let main (args: string []) =
    let input: string = File.ReadAllText "./input.txt"

    printfn "--- Part One ---"
    printfn "How many characters need to be processed before the first start-of-packet marker is detected?"
    printfn "Answer: %A\n" (findMarker 4 input)

    printfn "--- Part Two ---"
    printfn "How many characters need to be processed before the first start-of-message marker is detected?"
    printfn "Answer: %A\n" (findMarker 14 input)

    0
