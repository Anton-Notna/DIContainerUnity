using System.Collections.Generic;

namespace DI
{
    public static class SingletonSourceExtensions
    {
        public static SingletonSource<T> AsSource<T>(this T value) => new SingletonSource<T>(value);

        public static SingletonSource<TBase> AsSource<T, TBase>(this T value) where T : TBase => new SingletonSource<TBase>(value);

        public static IEnumerable<ISource> AsSources<TBase, T1, T2>(this TBase value) where TBase : T1 where T1 : T2
        {
            return new List<ISource>()
            {
                value.AsSource<T1>(),
                value.AsSource<T2>(),
            };
        }

        public static IEnumerable<ISource> AsSources<TBase, T1, T2, T3>(this TBase value) where TBase : T1 where T1 : T2 where T2 : T3
        {
            return new List<ISource>()
            {
                value.AsSource<T1>(),
                value.AsSource<T2>(),
                value.AsSource<T3>(),
            };
        }

        public static IEnumerable<ISource> AsSources<TBase, T1, T2, T3, T4>(this TBase value) where TBase : T1 where T1 : T2 where T2 : T3 where T3 : T4
        {
            return new List<ISource>()
            {
                value.AsSource<T1>(),
                value.AsSource<T2>(),
                value.AsSource<T3>(),
                value.AsSource<T4>(),
            };
        }

        public static IEnumerable<ISource> AsSources<TBase, T1, T2, T3, T4, T5>(this TBase value) where TBase : T1 where T1 : T2 where T2 : T3 where T3 : T4 where T4 : T5
        {
            return new List<ISource>()
            {
                value.AsSource<T1>(),
                value.AsSource<T2>(),
                value.AsSource<T3>(),
                value.AsSource<T4>(),
                value.AsSource<T5>(),
            };
        }

        public static IEnumerable<ISource> AsSources<TBase, T1, T2, T3, T4, T5, T6>(this TBase value) where TBase : T1 where T1 : T2 where T2 : T3 where T3 : T4 where T4 : T5 where T5 : T6
        {
            return new List<ISource>()
            {
                value.AsSource<T1>(),
                value.AsSource<T2>(),
                value.AsSource<T3>(),
                value.AsSource<T4>(),
                value.AsSource<T5>(),
                value.AsSource<T6>(),
            };
        }

    }
}