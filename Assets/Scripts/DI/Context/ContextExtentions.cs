using System;
using System.Collections.Generic;

namespace DI
{
    public static class ContextExtentions
    {
        public static void AddRange(this IContext context, params ISource[] sources) => context.AddRange(sources);

        public static void TryDispose(this IReadOnlyContext context)
        {
            foreach (KeyValuePair<Type, ISource> pair in context.GetSources())
            {
                if (pair.Value == null)
                    continue;

                if (pair.Value is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}