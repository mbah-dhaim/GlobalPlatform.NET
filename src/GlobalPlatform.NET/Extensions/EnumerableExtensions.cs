using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int blockSize)
        {
            int returned = 0;

            do
            {
                yield return enumerable.Skip(returned).Take(blockSize);

                returned += blockSize;
            }
            while (returned < enumerable.Count());
        }
    }
}
