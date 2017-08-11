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
