using System;
using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal static class ListUtilities
    {
        public static bool BinarySearch<E>(IList<E> list, Func<E, int> compare, out int index)
        {
            var num1 = 0;
            var num2 = list.Count;
            while (num1 < num2)
            {
                index = (num1 + num2) / 2;
                var num3 = compare(list[index]);
                if (num3 < 0)
                {
                    num1 = index + 1;
                }
                else
                {
                    if (num3 == 0)
                        return true;
                    num2 = index;
                }
            }
            index = num1;
            return false;
        }

        public static T? FirstOrNullable<T>(this IEnumerable<T> source) where T : struct
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Cast<T?>().FirstOrDefault<T?>();
        }
    }
}
