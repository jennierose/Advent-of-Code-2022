using System.Text.RegularExpressions;
using StackCollectionExtensions;
using StackCollection = Utils.StackCollection<int, string>;

class Program {
    private static readonly Regex StackItemPattern = new(@"(\s{3,4}|\[\w\]\s?)");

    internal static StackCollection ParseInput(string columnLabels, IEnumerable<string> inputLines) {
        StackCollection stacks = new();
        IEnumerable<int> keys = columnLabels
            .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse);

        foreach (string line in inputLines) {
            var items = StackItemPattern
                .Matches(line)
                .Select(match => match.Value.Trim().Trim('[', ']'))
                .Zip(keys)
                .Where(pair => pair.First.Length > 0);

            foreach ((string item, int key) in items) {
                stacks.Push(key, item);
            }
        }

        return stacks;
    }

    public static void Main() {
        string columnLabels = "";
        LinkedList<string> inputLines = new();
        List<MoveInstruction> moveInstructions = new();

        using (TextReader streamReader = new StreamReader(@"./input")) {
            string? line;
            Regex columnLabelsPattern = new(@"^\s*(\d+\s+)+\d*$");
            Regex stackItemLinePattern = new($"^{StackItemPattern}*$");

            while ((line = streamReader.ReadLine()) != null) {
                if (stackItemLinePattern.IsMatch(line)) {
                    inputLines.AddFirst(line);
                } else if (columnLabelsPattern.IsMatch(line)) {
                    columnLabels = line.Trim();
                    break;
                }
            }

            while ((line = streamReader.ReadLine()) != null) {
                if (MoveInstruction.CheckLine(line)) {
                    moveInstructions.Add(MoveInstruction.Parse(line));
                }
            }
        }

        StackCollection oneAtATimeStacks = ParseInput(columnLabels, inputLines);
        StackCollection allAtOnceStacks = ParseInput(columnLabels, inputLines);

        foreach (MoveInstruction move in moveInstructions) {
            oneAtATimeStacks.MoveItemsOneAtATime(move);
            allAtOnceStacks.MoveItemsAllAtOnce(move);
        }

        string firstResult = string.Join("", oneAtATimeStacks.Keys.Select(oneAtATimeStacks.Peek));
        string secondResult = string.Join("", allAtOnceStacks.Keys.Select(allAtOnceStacks.Peek));

        Console.WriteLine("--- Part One ---");
        Console.WriteLine("After the rearrangement procedure completes, what crate ends up on top of each stack?");
        Console.WriteLine("Answer: {0}\n", firstResult);

        Console.WriteLine("--- Part Two ---");
        Console.WriteLine("Before the rearrangement process finishes, update your simulation so that the Elves know where they");
        Console.WriteLine("should stand to be ready to unload the final supplies. After the rearrangement procedure completes,");
        Console.WriteLine("what crate ends up on top of each stack?");
        Console.WriteLine("Answer: {0}\n", secondResult);
    }
}
