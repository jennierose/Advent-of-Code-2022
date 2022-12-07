namespace Utils {
    public class StackCollection<K, T> where K : notnull {
        private readonly SortedDictionary<K, Stack<T>> Stacks;

        public IEnumerable<K> Keys {
            get { return Stacks.Keys; }
        }

        public StackCollection() {
            Stacks = new();
        }

        private void Touch(K key) {
            if (!Stacks.ContainsKey(key)) {
                Stacks.Add(key, new());
            }
        }

        public int Count(K key) {
            if (Stacks.ContainsKey(key)) {
                return Stacks[key].Count;
            }
            return -1;
        }

        public void Push(K key, T item) {
            Touch(key);
            Stacks[key].Push(item);
        }

        public T? Pop(K key) {
            if (Count(key) > 0) {
                return Stacks[key].Pop();
            }

            return default;
        }

        public T? Peek(K key) {
            if (Count(key) > 0) {
                return Stacks[key].Peek();
            }

            return default;
        }

        public void MultiPush(K key, IEnumerable<T> items) {
            foreach (T item in items) {
                Push(key, item);
            }
        }

        public List<T>? MultiPop(K key, int number) {
            if (Count(key) >= number) {
                return Enumerable.Range(0, number).Select(_ => Pop(key)!).ToList();
            }

            return default;
        }
    }
}
