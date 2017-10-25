using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<T> TakeLast<T>(this ICollection<T> enumerable, int count)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (count > enumerable.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                    "You cannot take more elements than the enumerable contains.");
            }

            return enumerable.Skip(enumerable.Count - count).Take(count);
        }
    }
}
