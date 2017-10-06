namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public static class QueryStringExtensions
    {
        public static T As<T>(this IQueryCollection query, string key)
        {
            var value = query[key].FirstOrDefault();

            if (value == null)
            {
                throw new Exception("Multiple query string parameters cannot be parsed automatically, use AsMultiple");
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static IEnumerable<T> AsMultiple<T>(this IQueryCollection query, string key)
        {
            var values = query[key];

            foreach (var val in values)
            {
                yield return (T)Convert.ChangeType(val, typeof(T));
            }
        }
    }
}
