namespace DevToys.PocoCsv.Core
{
    public class FixedQueue<T>
    {
        private T[] _Collection;
        private int _Position;
        private bool _Round = false;

        public FixedQueue(int size)
        {
            _Collection = new T[size];
        }

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

        public T[] GetCollection()
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