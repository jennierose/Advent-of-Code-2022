using System.Text.RegularExpressions;
using Utils;

namespace StackCollectionExtensions {
    public readonly struct MoveInstruction {
        private static readonly Regex MovePattern = new(@"^move (\d+) from (\w+) to (\w+)$");

        public readonly int Number;
        public readonly string Source;
        public readonly string Target;

        public MoveInstruction(int number, string source, string target) {
            Number = number;
            Source = source;
            Target = target;
        }

        public static bool CheckLine(string line) {
            return MovePattern.IsMatch(line);
        }

        public static MoveInstruction Parse(string line) {
            IEnumerable<Group> groups = MovePattern.Match(line).Groups;
            string[] args = groups.Skip(1).Select(g => g.Value).ToArray();
            return new MoveInstruction(int.Parse(args[0]), args[1], args[2]);
        }
    }

    public static class StackCollectionMoves {
        private static bool CheckMove<T>(this StackCollection<T> stacks, MoveInstruction move) {
            return stacks.Count(move.Source) >= move.Number;
        }

        public static void MoveItemsOneAtATime<T>(this StackCollection<T> stacks, MoveInstruction move) {
            if (stacks.CheckMove(move)) {
                foreach (T item in stacks.MultiPop(move.Source, move.Number) ?? new()) {
                    stacks.Push(move.Target, item);
                }
            }
        }

        public static void MoveItemsAllAtOnce<T>(this StackCollection<T> stacks, MoveInstruction move) {
            if (stacks.CheckMove(move)) {
                IEnumerable<T> items = stacks.MultiPop(move.Source, move.Number) ?? new();
                stacks.MultiPush(move.Target, items.Reverse());
            }
        }
    }
}
