using System.Collections.Generic;

namespace HyperStubs.ExtensionMethods
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            var i = 0;
            foreach (T itm in enumerable)
            {
                if (itm.Equals(item))
                    return i;

                i++;
            }

            return -1;
        }
    }
}