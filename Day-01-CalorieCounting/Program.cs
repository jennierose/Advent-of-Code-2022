internal class Program {
    private static void Main() {
        int calorieSubtotal = 0;
        List<int> calorieTotals = new();

        foreach (string line in File.ReadLines(@"./input.txt")) {
            try {
                calorieSubtotal += int.Parse(line);
            } catch (FormatException) {
                // Reset the subtotal when the program reaches the end of a block
                calorieTotals.Add(calorieSubtotal);
                calorieSubtotal = 0;
            }
        }

        calorieTotals.Sort();
        int mostCalories = calorieTotals.Last();
        int topThree;

        if (calorieTotals.Count >= 3) {
            topThree = calorieTotals.GetRange(calorieTotals.Count - 3, 3).Sum();
        } else {
            topThree = calorieTotals.Sum();
        }

        Console.WriteLine("--- Part One ---");
        Console.WriteLine("Find the Elf carrying the most Calories. How many total Calories is that Elf carrying?");
        Console.WriteLine("Answer: {0}\n", mostCalories);

        Console.WriteLine("--- Part Two ---");
        Console.WriteLine("Find the top three Elves carrying the most Calories. How many Calories are those Elves carrying in");
        Console.WriteLine("total?");
        Console.WriteLine("Answer: {0}\n", topThree);
    }
}
