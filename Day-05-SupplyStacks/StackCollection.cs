namespace Utils {
    public class StackCollection<T> {
        private readonly SortedDictionary<string, Stack<T>> Stacks;

        public IEnumerable<string> Keys {
            get { return Stacks.Keys; }
        }

        public StackCollection() {
            Stacks = new();
        }

        private void Touch(string key) {
            if (!Stacks.ContainsKey(key)) {
                Stacks.Add(key, new());
            }
        }

        public int Count(string key) {
            if (Stacks.ContainsKey(key)) {
                return Stacks[key].Count;
            }
            return -1;
        }

        public void Push(string key, T item) {
            Touch(key);
            Stacks[key].Push(item);
        }

        public T? Pop(string key) {
            if (Count(key) > 0) {
                return Stacks[key].Pop();
            }

            return default;
        }

        public T? Peek(string key) {
            if (Count(key) > 0) {
                return Stacks[key].Peek();
            }

            return default;
        }

        public void MultiPush(string key, IEnumerable<T> items) {
            foreach (T item in items) {
                Push(key, item);
            }
        }

        public List<T>? MultiPop(string key, int number) {
            if (Count(key) >= number) {
                return Enumerable.Range(0, number).Select(_ => Pop(key)!).ToList();
            }

            return default;
        }
    }
}
