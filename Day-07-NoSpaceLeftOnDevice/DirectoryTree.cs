using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;

namespace Utils {
    public class DirectoryTree {
        [Flags]
        public enum NodeType {
            None = 0x00,
            File = 0x0f,
            Directory = 0xf0,
            Any = 0xff
        }

        public readonly Guid UUID;
        public readonly string Name;
        private readonly Dictionary<string, DirectoryTree> Children;
        private readonly Dictionary<string, int> FileData;
        private readonly DirectoryTree? Parent;

        private const string CurrentDirectory = ".";
        private const string ParentDirectory = "..";
        private const string RootDirectory = "/";
        private static readonly ReadOnlyCollection<string> SpecialDirectories =
            new(new[] { CurrentDirectory, ParentDirectory, RootDirectory });

        public int LocalSize => FileData.Values.Sum();
        public IEnumerable<DirectoryTree> Subdirectories => Children.Values.AsEnumerable();

        private DirectoryTree(string dirName, DirectoryTree? parent = null) {
            Name = dirName;
            Children = new();
            FileData = new();
            Parent = parent;
            UUID = Guid.NewGuid();
        }

        public static DirectoryTree CreateRoodDirectory() {
            return new("/");
        }

        public bool FileExists(string fileName, NodeType nodeType = NodeType.Any) {
            if ((nodeType & NodeType.File) != NodeType.None) {
                if (FileData.ContainsKey(fileName)) {
                    return true;
                }
            }

            if ((nodeType & NodeType.Directory) != NodeType.None) {
                if (SpecialDirectories.Contains(fileName) || Children.ContainsKey(fileName)) {
                    return true;
                }
            }

            return false;
        }

        private void ThrowErrorIfFileExists(string fileName, NodeType nodeType = NodeType.Any) {
            if (FileExists(fileName, nodeType)) {
                throw new ArgumentException($"Cannot create '{fileName}': File exists");
            }
        }

        private void ThrowErrorIfFileDoesNotExist(string fileName, NodeType nodeType = NodeType.Any) {
            if (!FileExists(fileName, nodeType)) {
                throw new ArgumentException($"Cannot access '{fileName}': No such file or directory");
            }
        }

        private static void ThrowErrorIfFileNameIsInvalid(string fileName) {
            if (!new Regex(@"^[\w.]+$").IsMatch(fileName)) {
                throw new ArgumentException($"Cannot create '{fileName}': Invalid file name");
            }
        }

        public void AddFile(string fileName, int fileSize) {
            ThrowErrorIfFileExists(fileName);
            ThrowErrorIfFileNameIsInvalid(fileName);
            FileData.Add(fileName, fileSize);
        }

        public void AddDirectory(string dirName) {
            ThrowErrorIfFileExists(dirName);
            ThrowErrorIfFileNameIsInvalid(dirName);
            DirectoryTree child = new(dirName, this);
            Children.Add(dirName, child);
        }

        public DirectoryTree GetRootDirectory() {
            DirectoryTree node = this;
            HashSet<Guid> history = new();

            while (node.Parent != null) {
                history.Add(node.UUID);
                node = node.Parent;

                if (history.Contains(node.UUID)) {
                    throw new InvalidConstraintException("Cannot find root of DirectoryTree (cycle detected)");
                }
            }

            return node;
        }

        public DirectoryTree GetDirectory(string dirName) {
            ThrowErrorIfFileDoesNotExist(dirName, NodeType.Directory);

            if (dirName == CurrentDirectory) {
                return this;
            } else if (dirName == ParentDirectory) {
                return Parent ?? this;
            } else if (dirName == RootDirectory) {
                return GetRootDirectory();
            } else {
                return Children[dirName];
            }
        }

        public IEnumerable<DirectoryTree> DepthFirstTraversal() {
            IEnumerable<DirectoryTree> start = new[] { this };
            return Subdirectories.SelectMany(node => node.DepthFirstTraversal()).Concat(start);
        }
    }
}
