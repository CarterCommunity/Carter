namespace Botwin.Request
{
    using System;
    using System.Collections.Generic;

    internal static class ConvertExtensions
    {
        public static T ConvertTo<T>(this object value)
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                if (value == null) return default(T);

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
                    yield return default(T);
                else
                    yield return (T)Convert.ChangeType(value, type);
            }
        }
    }
}
