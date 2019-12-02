using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Shared.Library.Extensions
{
    public static class SystemCollectionsGenericExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                return !source.Any();
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            if (source != null)
            {
                return source.Count == 0;
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> source)
        {
            if (source != null)
            {
                return source.Count == 0;
            }
            return true;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return from element in source
                   where element != null
                   select element;
        }
    }
}
