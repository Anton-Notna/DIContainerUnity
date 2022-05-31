using System.Collections.Generic;

namespace DI
{
    public class ReusableArray<T>
    {
        private List<T[]> _arrays;

        public ReusableArray(int preparedMaxLength)
        {
            _arrays = new List<T[]>(preparedMaxLength);
            Upscale(preparedMaxLength);
        }

        public T[] Get(int length)
        {
            if (_arrays.Count <= length)
                Upscale(length);

            return _arrays[length];
        }


        private void Upscale(int length)
        {
            for (int i = _arrays.Count; i <= length; i++)
            {
                T[] result = new T[_arrays.Count];
                _arrays.Add(result);
            }
        }
    }
}