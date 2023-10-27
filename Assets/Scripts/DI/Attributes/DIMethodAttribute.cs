using System;

namespace DI
{
    /// <summary>
    /// In MonoBehaviour calls after Awake and before Start.
    /// </summary>

    [AttributeUsage(AttributeTargets.Method)]
    public class DIMethodAttribute : PreserveAttribute { }
}