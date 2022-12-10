using System.Text.RegularExpressions;

internal class Program {
    private readonly struct Point {
        public static readonly Point Origin = new(0, 0);
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj) {
            return obj is Point point &&
                   X == point.X && Y == point.Y;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public Point Move(Point move) {
            return new(X + move.X, Y + move.Y);
        }
    }

    private static class Direction {
        public static Point Down = new(0, -1);
        public static Point Left = new(-1, 0);
        public static Point Right = new(+1, 0);
        public static Point Up = new(0, +1);

        public static Point Parse(string abbrev) {
            return abbrev switch {
                "D" or "d" => Down,
                "L" or "l" => Left,
                "R" or "r" => Right,
                "U" or "u" => Up,
                _ => throw new ArgumentException("Invalid Character"),
            };
        }
    }

    private static void Main() {
        Regex motionPattern = new(@"^([DLRU])\s+(\d+)$");
        List<(Point, int)> instructions = new();

        foreach (string line in File.ReadLines(@"./input.txt")) {
            if (motionPattern.IsMatch(line)) {
                Point dir = Direction.Parse(motionPattern.Match(line).Groups[1].Value);
                int amount = int.Parse(motionPattern.Match(line).Groups[2].Value);
                instructions.Add((dir, amount));
            }
        }

        List<Point> rope = Enumerable.Repeat(Point.Origin, 10).ToList();
        HashSet<Point> secondKnotPath = new(new[] { Point.Origin });
        HashSet<Point> lastKnotPath = new(new[] { Point.Origin });

        foreach ((Point dir, int amount) in instructions) {
            foreach (int x in Enumerable.Range(0, amount)) {
                rope[0] = rope[0].Move(dir);

                foreach (int i in Enumerable.Range(1, rope.Count - 1)) {
                    Point head = rope[i - 1];
                    Point tail = rope[i];
                    int deltaX = Math.Abs(head.X - tail.X);
                    int deltaY = Math.Abs(head.Y - tail.Y);
                    int distance = Math.Max(deltaX, deltaY);

                    if (distance >= 2) {
                        deltaX = Math.Sign(head.X - tail.X);
                        deltaY = Math.Sign(head.Y - tail.Y);
                        rope[i] = rope[i].Move(new(deltaX, deltaY));
                    }
                }

                secondKnotPath.Add(rope[1]);
                lastKnotPath.Add(rope.Last());
            }

        }

        Console.WriteLine("--- Part One ---");
        Console.WriteLine("Simulate your complete hypothetical series of motions. How many positions does the tail of");
        Console.WriteLine("the rope visit at least once?");
        Console.WriteLine($"Answer: {secondKnotPath.Count}\n");

        Console.WriteLine("--- Part Two ---");
        Console.WriteLine("Simulate your complete series of motions on a larger rope with ten knots. How many");
        Console.WriteLine("positions does the tail of the rope visit at least once?");
        Console.WriteLine($">>> {lastKnotPath.Count}\n");
    }
}
