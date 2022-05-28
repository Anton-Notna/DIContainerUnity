using System.Collections.Generic;

namespace DI
{
    public class ReusableArray<T>
    {
        private Dictionary<int, T[]> _arrays = new Dictionary<int, T[]>();

        public ReusableArray(int preparedMaxLength)
        {
            for (int i = 0; i <= preparedMaxLength; i++)
                Create(i);
        }

        public T[] Get(int length)
        {
            if (_arrays.TryGetValue(length, out T[] result))
                return result;
            else
                return Create(length);

        }

        private T[] Create(int length)
        {
            T[] result = new T[length];
            _arrays.Add(length, result);

            return result;
        }
    }
}