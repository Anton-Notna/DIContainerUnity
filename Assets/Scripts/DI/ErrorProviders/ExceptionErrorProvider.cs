using System;

namespace DI
{
    public class ExceptionErrorProvider : IErrorProvider
    {
        public void Throw(Exception exception) =>
            throw exception;
    }
}