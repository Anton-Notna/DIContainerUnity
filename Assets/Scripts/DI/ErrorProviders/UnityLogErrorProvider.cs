using System;
using UnityEngine;

namespace DI
{
    public class UnityLogErrorProvider : IErrorProvider
    {
        public void Throw(Exception exception) => 
            Debug.LogError(exception.ToString());
    }
}