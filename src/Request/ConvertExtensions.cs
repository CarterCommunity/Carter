namespace Botwin.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ConvertExtensions
    {
        public static T ConvertTo<T>(this object value)
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                if (value == null) return default;

                type = underlyingType;
            }

            return (T)Convert.ChangeType(value, type);
        }

        public static IEnumerable<T> ConvertMultipleTo<T>(this IEnumerable<string> values)
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                type = underlyingType;
            }

            foreach (var value in values)
            {
                if (value == null)
                    yield return default;
                else
                    yield return (T)Convert.ChangeType(value, type);
            }
        }
        
        public static bool IsArray(this Type source)
        {
            return source.BaseType == typeof(Array);
        }

        public static bool IsCollection(this Type source)
        {
            var collectionType = typeof(ICollection<>);

            return source.IsGenericType && source
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == collectionType);
        }

        public static bool IsEnumerable(this Type source)
        {
            var enumerableType = typeof(IEnumerable<>);

            return source.IsGenericType && source.GetGenericTypeDefinition() == enumerableType;
        }
    }
}
