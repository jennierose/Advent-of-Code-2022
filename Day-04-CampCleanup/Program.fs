#nowarn "25"

open System.IO
open System.Text.RegularExpressions

type Section(left: int, right: int) =
    member this.Start = min left right
    member this.End = max left right

    static member private LineRegex: Regex = Regex @"^(\d+)-(\d+),(\d+)-(\d+)$"

    static member checkLine = Section.LineRegex.IsMatch

    static member parseLine(line: string) : (Section * Section) =
        Seq.cast (Section.LineRegex.Match line).Groups
        // Skip the first group, which contains the entire string
        |> Seq.skip 1
        // Parse the remaining four (\d+) groups as integers
        |> Seq.map (fun (g: Group) -> int g.Value)
        // Split the four integers into two arrays of size two
        |> Seq.chunkBySize 2
        // Convert each of the two arrays into Section objects
        |> Seq.toArray
        |> Array.map (fun [| left: int; right: int |] -> Section(left, right))
        // Convert the array of two Sections into a tuple
        |> fun [| fst: Section; snd: Section |] -> (fst, snd)

    member this.overlaps(other: Section) : bool =
        max this.Start other.Start
        <= min this.End other.End

    member this.totallyOverlaps(other: Section) : bool =
        let maxSection: Section =
            Section(min this.Start other.Start, max this.End other.End)

        List.contains maxSection [ this; other ]

    member this.ToTuple: int * int = (this.Start, this.End)

    override this.GetHashCode() : int = hash this.ToTuple

    override this.Equals(x: obj) : bool =
        match x with
        | :? Section as (other: Section) -> this.ToTuple = other.ToTuple
        | _ -> false

[<EntryPoint>]
let main (args: string []) =
    let input: seq<Section * Section> =
        Seq.cast (File.ReadAllLines @"./input")
        |> Seq.filter Section.checkLine
        |> Seq.map Section.parseLine

    printfn "--- Part One ---"
    printfn "In how many assignment pairs does one range fully contain the other?"

    input
    |> Seq.filter (fun (fst: Section, snd: Section) -> fst.totallyOverlaps snd)
    |> Seq.length
    |> printfn "Answer: %d\n"

    printfn "--- Part Two ---"
    printfn "In how many assignment pairs do the ranges overlap?"

    input
    |> Seq.filter (fun (fst: Section, snd: Section) -> fst.overlaps snd)
    |> Seq.length
    |> printfn "Answer: %d\n"

    0
