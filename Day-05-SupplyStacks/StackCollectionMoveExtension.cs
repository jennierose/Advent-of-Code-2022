using System.Text.RegularExpressions;
using Utils;

namespace StackCollectionExtensions {
    public readonly struct MoveInstruction {
        private static readonly Regex MovePattern = new(@"^move (\d+) from (\d+) to (\d+)$");

        public readonly int Number;
        public readonly int Source;
        public readonly int Target;

        public MoveInstruction(int number, int source, int target) {
            Number = number;
            Source = source;
            Target = target;
        }

        public static bool CheckLine(string line) {
            return MovePattern.IsMatch(line);
        }

        public static MoveInstruction Parse(string line) {
            IEnumerable<Group> groups = MovePattern.Match(line).Groups;
            int[] args = groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
            return new MoveInstruction(args[0], args[1], args[2]);
        }
    }

    public static class StackCollectionMoveExtension {
        private static bool CheckMove<T>(this StackCollection<int, T> stacks, MoveInstruction move) {
            return stacks.Count(move.Source) >= move.Number;
        }

        public static void MoveItemsOneAtATime<T>(this StackCollection<int, T> stacks, MoveInstruction move) {
            if (stacks.CheckMove(move)) {
                foreach (T item in stacks.MultiPop(move.Source, move.Number) ?? new()) {
                    stacks.Push(move.Target, item);
                }
            }
        }

        public static void MoveItemsAllAtOnce<T>(this StackCollection<int, T> stacks, MoveInstruction move) {
            if (stacks.CheckMove(move)) {
                IEnumerable<T> items = stacks.MultiPop(move.Source, move.Number) ?? new();
                stacks.MultiPush(move.Target, items.Reverse());
            }
        }
    }
}
