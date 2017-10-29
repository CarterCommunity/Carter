namespace Botwin.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public static class QueryStringExtensions
    {
        /// <summary>
        /// Retrieve strongly typed query paramater value for given key
        /// </summary>
        /// <typeparam name="T">Query param type</typeparam>
        /// <param name="query"><see cref="IQueryCollection"/></param>
        /// <param name="key">Query param key</param>
        /// <param name="defaultValue">Default quary paramter value</param>
        /// <returns>Query paramater value</returns>
        public static T As<T>(this IQueryCollection query, string key, T defaultValue = default(T))
        {
            var value = query[key].FirstOrDefault();

            if (value == null)
            {
                return defaultValue;
            }

            return value.ConvertTo<T>();
        }

        /// <summary>
        /// Retrieve strongly typed query paramater values for given key
        /// </summary>
        /// <typeparam name="T">Query param type</typeparam>
        /// <param name="query"><see cref="IQueryCollection"/></param>
        /// <param name="key">Query param key</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<T> AsMultiple<T>(this IQueryCollection query, string key)
        {
            var values = query[key];

            var splitValues = values.SelectMany(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            return splitValues.ConvertMultipleTo<T>();
        }
    }
}
