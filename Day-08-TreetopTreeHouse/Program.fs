open System.IO
open FSharpPlus

let iterRow (ls: 't list list) (j: int) : seq<'t> =
    if j < 0 || j >= ls.Length then
        failwith (sprintf "Row index [%d] is out of range" j)
    else
        Seq.ofList ls[j]

let iterColumn (ls: 't list list) (i: int) : seq<'t> =
    if i < 0 || List.exists (fun (row: 't list) -> i >= row.Length) ls then
        failwith (sprintf "Column index [%d] is out of range" i)
    else
        Seq.map (fun (row: 't list) -> row[i]) ls

let scanTallestTrees (trees: seq<'t>) : int list =
    let idxTrees: seq<int * 't> = Seq.indexed trees
    let revTrees: seq<int * 't> = Seq.rev idxTrees
    let pickTallerTree (tallestSoFar: int * 't) (currentTree: int * 't) =
        maxBy snd [ tallestSoFar; currentTree ]

    // Scans the sequence of trees both forward and backward,
    // and replaces each item with the height of the tallest tree seen so far.
    // Each of the "tallest so far" trees will be visible from outside the grid.
    Seq.append
        (Seq.scan pickTallerTree (Seq.head idxTrees) (Seq.tail idxTrees))
        (Seq.scan pickTallerTree (Seq.head revTrees) (Seq.tail revTrees))
    |> Seq.map fst
    |> Seq.toList

let scenicTreeScores (trees: seq<'t>) : int list =
    // Takes a tree, and the list of trees it might be able to "see" looking in any one direction,
    // and counts the neighbors until it finds a tree that is at least as tall as the main tree.
    // The result is the number of trees visible in that direction.
    let countVisible (tree: 't) (neighbors: 't list) : int =
        List.takeWhile ((>) tree) neighbors
        |> List.length
        |> (+) 1
        |> min neighbors.Length

    // Scans the sequence of trees from left to right,
    // and replaces each tree with a list of trees to its left,
    // then pipes the results into `countVisible` to count how many trees each tree can see to its left.
    let lookLeft: seq<int> =
        Seq.scan (flip List.cons) [] trees
        |> Seq.zip trees
        |> Seq.map ((<||) countVisible)

    // Scans the sequence of trees from right to left,
    // and replaces each tree with a list of trees on its right,
    // then pipes the results into `countVisible` to count how many trees each tree can see to its right.
    let lookRight: seq<int> =
        Seq.scanBack List.cons trees []
        |> Seq.tail
        |> Seq.zip trees
        |> Seq.map ((<||) countVisible)

    // Combines the results of `lookLeft` and `lookRight` and multiplies the corresponding elements.
    Seq.zip lookLeft lookRight
    |> Seq.map ((<||) (*))
    |> Seq.toList

[<EntryPoint>]
let main (args: string []) =
    let input: seq<string> = File.ReadLines "./input.txt"
    assert Seq.forall (Seq.forall System.Char.IsDigit) input

    let lineToList: string -> int list =
        (Seq.map (System.Char.GetNumericValue >> int))
        >> Seq.toList

    let treeGrid: int list list =
        input
        |> Seq.map lineToList
        |> Seq.toList

    let height: int = treeGrid.Length
    let width: int = treeGrid[0].Length
    assert List.forall ((=) width) (List.map List.length treeGrid)

    // Iterates through each row and column of the grid,
    // and yields the index of each tree that's visible from the left or right,
    // followed by the indices of trees that are visible from the bottom or top.
    // The number of unique indices is how many trees are visible from outside the grid.
    let visibleTrees: int =
        (Seq.distinct >> Seq.length)
        <| seq {
            for j: int in { 0 .. height - 1 } do
                yield!
                    iterRow treeGrid j
                    |> scanTallestTrees
                    |> List.map (fun (i: int) -> (j, i))

            for i: int in { 0 .. width - 1 } do
                yield!
                    iterColumn treeGrid i
                    |> scanTallestTrees
                    |> List.map (fun (j: int) -> (j, i))
        }

    // Computes a partial scenic score for each tree in the grid.
    // The row scores only account for what each tree can see on its left and right.
    let rowScores: int list list =
        { 0 .. height - 1 }
        |> Seq.map (iterRow treeGrid)
        |> Seq.map scenicTreeScores
        |> Seq.toList

    // Iterates through each column of the grid, and computes a partial scenic score for each tree.
    // Next, it zips the lists of vertical scores with the *columns* of `rowScores`,
    // and multiplies the horizontal and vertical scores to get the total score for each tree.
    // The sequence yields the maximum scenic score in each column,
    // and the maximum of those is the top scenic score over all.
    let topScenicScore =
        Seq.max
        <| seq {
            for i: int in { 0 .. width - 1 } do
                iterColumn treeGrid i
                |> scenicTreeScores
                |> Seq.zip (iterColumn rowScores i)
                |> Seq.map ((<||) (*))
                |> Seq.max
        }

    printfn "--- Part One ---"
    printfn "Consider your map; how many trees are visible from outside the grid?"
    printfn "Answer %d\n" visibleTrees

    printfn "--- Part Two ---"
    printfn "Consider each tree on your map. What is the highest scenic score possible for any tree?"
    printfn "Answer %d\n" topScenicScore

    0
