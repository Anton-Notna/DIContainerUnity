using System;

namespace DI
{
    public interface IErrorProvider
    {
        public void Throw(Exception exception);
    }
}