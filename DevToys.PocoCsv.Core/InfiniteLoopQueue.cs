namespace DevToys.PocoCsv.Core
{
    /// <summary>
    /// A Queue that only holds last x items of T
    /// The infinite loop queue does not reorder on Add it only maintains a last add position and loop around the size of the queue.
    /// GetQueue recontstructs the correct queue order based on last position.
    ///
    /// It is similar to a LIFO but differs with a fixed size.
    /// </summary>
    internal sealed class InfiniteLoopQueue<T>
    {
        private readonly T[] _Collection;
        private int _Position;
        private bool _Round = false;

        /// <summary>
        /// Construct the Queue with it's size.
        /// </summary>
        public InfiniteLoopQueue(int size)
        {
            _Collection = new T[size];
        }

        /// <summary>
        /// Add item to the Queue
        /// </summary>
        public void Add(T item)
        {
            _Collection[_Position] = item;
            _Position++;
            if (_Position == _Collection.Length)
            {
                _Position = 0;
                _Round = true;
            }
        }

        /// <summary>
        /// Materialize the Queue
        /// </summary>
        public T[] GetQueue()
        {
            T[] _result = new T[_Collection.Length];
            if (!_Round)
            {
                _result = new T[_Position];
            }

            int _xx = 0;
            for (int ii = _Position; ii < _result.Length; ii++)
            {
                _result[_xx] = _Collection[ii];
                _xx++;
            }
            for (int ii = 0; ii < _Position; ii++)
            {
                _result[_xx] = _Collection[ii];
                _xx++;
            }

            return _result;
        }
    }
}