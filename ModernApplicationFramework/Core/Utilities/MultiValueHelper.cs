using System;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class MultiValueHelper
    {
        public static void CheckValue<T>(object[] values, int index)
        {
            if (!(values[index] is T) && (values[index] != null || typeof(T).IsValueType))
                throw new ArgumentException(string.Format("{0} is not from this Type: {1}", index, typeof(T).IsValueType));
        }

        public static void CheckType<T>(Type[] types, int index)
        {
            if (!types[index].IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException(string.Format("Target {0} is not from Type: {1}", index, typeof(T).FullName));
        }
    }
}
