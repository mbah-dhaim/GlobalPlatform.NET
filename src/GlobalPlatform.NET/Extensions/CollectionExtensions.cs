using System;
using System.Collections.Generic;

namespace GlobalPlatform.NET.Extensions
{
    internal static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void ForEach<TSource>(this ICollection<TSource> collection, Action<TSource, int, bool> action)
        {
            int index = 0;

            foreach (var item in collection)
            {
                action(item, index, collection.Count == index + 1);

                index++;
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this ICollection<TSource> collection,
            Func<TSource, int, bool, TResult> selector)
        {
            int index = 0;

            foreach (var item in collection)
            {
                yield return selector(item, index, collection.Count == index + 1);

                index++;
            }
        }
    }
}
