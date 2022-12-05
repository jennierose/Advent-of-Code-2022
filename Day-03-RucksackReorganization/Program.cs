static int ItemPriority(char item) {
    if ('a' <= item && item <= 'z') {
        return item - 'a' + 1;
    } else if ('A' <= item && item <= 'Z') {
        return item - 'A' + 27;
    } else {
        return 0;
    }
}

List<(string, string)> inputs = new();

foreach (string line in File.ReadLines(@"./input")) {
    if (line.Length > 0 && line.Length % 2 == 0) {
        string firstCompartment = line[..(line.Length / 2)];
        string secondCompartment = line[(line.Length / 2)..];
        inputs.Add((firstCompartment, secondCompartment));
    }
}

int teamBadgePriorityTotal = 0;
int crossCompartmentPriorityTotal = 0;
HashSet<char> teamItems = new();
for (int index = 0; index < inputs.Count; index++) {
    (string firstCompartment, string secondCompartment) = inputs[index];

    HashSet<char> compartmentItems = new(firstCompartment);
    compartmentItems.IntersectWith(secondCompartment);
    crossCompartmentPriorityTotal += compartmentItems.Select(ItemPriority).Sum();

    switch (index % 3) {
        case 0:
            teamItems.UnionWith(firstCompartment + secondCompartment);
            break;
        case 1:
            teamItems.IntersectWith(firstCompartment + secondCompartment);
            break;
        case 2:
            teamItems.IntersectWith(firstCompartment + secondCompartment);
            teamBadgePriorityTotal += teamItems.Select(ItemPriority).Sum();
            teamItems.Clear();
            break;
    }
}

Console.WriteLine("--- Part One ---");
Console.WriteLine("Find the item type that appears in both compartments of each rucksack. What is the sum of the");
Console.WriteLine("priorities of those item types?");
Console.WriteLine("Answer: {0}\n", crossCompartmentPriorityTotal);

Console.WriteLine("--- Part Two ---");
Console.WriteLine("Find the item type that corresponds to the badges of each three-Elf group. What is the sum of the");
Console.WriteLine("priorities of those item types?");
Console.WriteLine("Answer: {0}\n", teamBadgePriorityTotal);
