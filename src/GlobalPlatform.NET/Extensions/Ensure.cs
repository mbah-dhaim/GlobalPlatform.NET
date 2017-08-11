using System;
using System.Collections.Generic;

namespace GlobalPlatform.NET.Extensions
{
    internal static class Ensure
    {
        public static void IsNotNull(object instance, string name)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void IsNotEmpty(string instance, string name)
        {
            if (instance.Length == 0)
            {
                throw new ArgumentException($"{name} cannot be empty.", name);
            }
        }

        public static void IsNotEmpty<T>(ICollection<T> instance, string name)
        {
            if (instance.Count == 0)
            {
                throw new ArgumentException($"{name} is empty.", name);
            }
        }

        public static void IsNotNullOrEmpty<T>(ICollection<T> instance, string name)
        {
            IsNotNull(instance, name);
            IsNotEmpty(instance, name);
        }

        public static void HasCount<T>(ICollection<T> instance, string name, int count)
        {
            IsNotNull(instance, name);
            IsNotEmpty(instance, name);

            if (instance.Count != count)
            {
                throw new ArgumentException($"{name} does not have the expected count of {count}.", name);
            }
        }

        public static void HasAtLeast<T>(ICollection<T> instance, string name, int min)
        {
            IsNotNull(instance, name);
            IsNotEmpty(instance, name);

            if (instance.Count < min)
            {
                throw new ArgumentException($"{name} must contain at least {min} elements.", name);
            }
        }

        public static void HasNoMoreThan<T>(ICollection<T> instance, string name, int max)
        {
            IsNotNull(instance, name);
            IsNotEmpty(instance, name);

            if (instance.Count > max)
            {
                throw new ArgumentException($"{name} must contain no more than {max} elements.", name);
            }
        }

        public static void HasCount<T>(ICollection<T> instance, string name, int min, int max)
        {
            IsNotNull(instance, name);
            IsNotEmpty(instance, name);
            HasAtLeast(instance, name, min);
            HasNoMoreThan(instance, name, max);
        }

        public static void IsAID(ICollection<byte> instance, string name)
            => HasCount(instance, name, 5, 16);
    }
}
