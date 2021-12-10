namespace PDMF.Algorithms.Shared
{
    public class Pair<T, U> {
        public Pair() {
        }

        public Pair(T first, U second) {
            First = first;
            Second = second;
        }

        public T First { get; private set; }
        public U Second { get; private set; }
    };
}