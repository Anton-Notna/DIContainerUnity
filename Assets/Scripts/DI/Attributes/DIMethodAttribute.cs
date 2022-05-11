using System;

namespace DI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DIMethodAttribute : PreserveAttribute { }
}