#nowarn "25"

open System.IO
open System.Text.RegularExpressions

type RockPaperScissors =
    | Rock = 1
    | Paper = 2
    | Scissors = 3

type LoseDrawWin =
    | Lose = 0
    | Draw = 3
    | Win = 6

#nowarn "25"

let rockPaperScissorsScore (abc: string) (xyz: string) : int =
    let myMove: int =
        match xyz with
        | "X" -> int RockPaperScissors.Rock
        | "Y" -> int RockPaperScissors.Paper
        | "Z" -> int RockPaperScissors.Scissors

    let scoreKey: LoseDrawWin [] = LoseDrawWin.GetValues()

    let myScore: int =
        match abc with
        | "A" -> int scoreKey[myMove % 3]
        | "B" -> int scoreKey[myMove - 1]
        | "C" -> int scoreKey[(myMove + 1) % 3]

    myMove + myScore

let loseDrawWinScore (abc: string) (xyz: string) : int =
    let myScore: int =
        match xyz with
        | "X" -> int LoseDrawWin.Lose
        | "Y" -> int LoseDrawWin.Draw
        | "Z" -> int LoseDrawWin.Win

    let moveKey: RockPaperScissors [] = RockPaperScissors.GetValues()

    let myMove: int =
        match abc with
        | "A" -> int moveKey[(myScore / 3 + 2) % 3]
        | "B" -> int moveKey[myScore / 3]
        | "C" -> int moveKey[(myScore / 3 + 1) % 3]

    myMove + myScore

[<EntryPoint>]
let main (args: string []) =
    let checkFormat: Regex = Regex @"^[ABC]\s+[XYZ]$"

    let input: seq<string * string> =
        Seq.cast (File.ReadAllLines(@"./input"))
        |> Seq.filter checkFormat.IsMatch
        |> Seq.map (fun (line: string) -> line.Split null)
        |> Seq.map (fun [| abc: string; xyz: string |] -> (abc, xyz))

    printfn "--- Part One ---"
    printfn "What would your total score be if everything goes exactly according to your strategy guide?"

    input
    |> Seq.map (fun (moves: string * string) -> moves ||> rockPaperScissorsScore)
    |> Seq.sum
    |> printfn "Answer: %d\n"

    printfn "--- Part Two ---"
    printfn "Following the Elf's instructions for the second column, what would your total score be if everything goes"
    printfn "exactly according to your strategy guide?"

    input
    |> Seq.map (fun (moves: string * string) -> moves ||> loseDrawWinScore)
    |> Seq.sum
    |> printfn "Answer: %d\n"

    0
