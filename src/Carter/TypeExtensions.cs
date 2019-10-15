namespace Carter
{
    using System;
    using System.Linq;

    internal static class TypeExtensions
    {
        internal static void MustDeriveFrom<T>(this Type[] types)
        {
            var invalidTypes = types.Where(m => !typeof(T).IsAssignableFrom(m)).ToList();
            if (invalidTypes.Any())
            {
                throw new ArgumentException($"Types must derive from {typeof(T).Name}, failing types: {string.Join(",", invalidTypes)}");
            }
        }
    }
}