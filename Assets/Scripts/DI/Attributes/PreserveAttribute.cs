using System;

namespace DI
{
    /// <summary>
    /// The code stripper will consider any attribute with the exact name "PreserveAttribute" as a reason not to strip the thing it is applied on.
    /// </summary>
    public class PreserveAttribute : Attribute { }
}