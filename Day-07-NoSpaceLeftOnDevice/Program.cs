using System.Text.RegularExpressions;
using Utils;

internal class Program {
    static readonly Regex ChdirPattern = new(@"^\$\s+cd\s+(/|[\w.]+)$");
    static readonly Regex FilePattern = new(@"^(\d+)\s+([\w.]+)$");
    static readonly Regex DirPattern = new(@"^dir\s+([\w.]+)$");
    static readonly int TotalDiskSpace = 70000000;
    static readonly int NeededFreeSpace = 30000000;

    private static void Main() {
        DirectoryTree rootDirectory = DirectoryTree.CreateRoodDirectory();
        DirectoryTree currentDirectory = rootDirectory;

        foreach (string line in File.ReadLines(@"./input.txt")) {
            // Discover a file in the current directory => Add it to the current DirectoryTree node
            if (FilePattern.IsMatch(line)) {
                int fileSize = int.Parse(FilePattern.Match(line).Groups[1].Value);
                string fileName = FilePattern.Match(line).Groups[2].Value;
                currentDirectory.AddFile(fileName, fileSize);
            }
            // Discover a subdirectory in the current directory => Add a new child to the current node
            if (DirPattern.IsMatch(line)) {
                string dirName = DirPattern.Match(line).Groups[1].Value;
                currentDirectory.AddDirectory(dirName);
            }
            // Change directory, can only descend into subdirectories that have already been added
            if (ChdirPattern.IsMatch(line)) {
                string dirName = ChdirPattern.Match(line).Groups[1].Value;
                currentDirectory = currentDirectory.GetDirectory(dirName);
            }
        }

        Dictionary<Guid, int> directorySizes = new();

        foreach (DirectoryTree node in rootDirectory.DepthFirstTraversal()) {
            directorySizes[node.UUID] =
                node.LocalSize +
                node.Subdirectories.Select(child => directorySizes[child.UUID]).Sum();
        }

        int maxTotalSize = 100000;
        int freeSpace = TotalDiskSpace - directorySizes[rootDirectory.UUID];
        int minSizeToDelete = NeededFreeSpace - freeSpace;

        int firstResult = directorySizes.Values.Where(n => n <= maxTotalSize).Sum();
        int secondResult = directorySizes.Values.Where(n => n >= minSizeToDelete).Min();

        Console.WriteLine("--- Part One ---");
        Console.WriteLine("Find all of the directories with a total size of at most 100000. What is the sum of the");
        Console.WriteLine("total sizes of those directories?");
        Console.WriteLine("Answer: {0}\n", firstResult);

        Console.WriteLine("--- Part Two ---");
        Console.WriteLine("Find the smallest directory that, if deleted, would free up enough space on the filesystem");
        Console.WriteLine("to run the update. What is the total size of that directory?");
        Console.WriteLine("Answer: {0}\n", secondResult);
    }
}
