using System;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class MultiValueHelper
    {
        public static void CheckValue<T>(object[] values, int index)
        {
            if (!(values[index] is T) && (values[index] != null || typeof(T).IsValueType))
                throw new ArgumentException($"{index} is not from this Type: {typeof(T).IsValueType}");
        }

        public static void CheckType<T>(Type[] types, int index)
        {
            if (!types[index].IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException($"Target {index} is not from Type: {typeof(T).FullName}");
        }
    }
}
