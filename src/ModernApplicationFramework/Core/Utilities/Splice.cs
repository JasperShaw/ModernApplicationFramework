using System;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class SpliceExtension
    {
        /// <summary>
        /// Creates an array containing a part of the array (similar to string.Substring).
        /// </summary>
        public static T[] Splice<T>(this T[] array, int startIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            return Splice(array, startIndex, array.Length - startIndex);
        }

        /// <summary>
        /// Creates an array containing a part of the array (similar to string.Substring).
        /// </summary>
        public static T[] Splice<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "Value must be between 0 and " + array.Length);
            if (length < 0 || length > array.Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(length), length, "Value must be between 0 and " + (array.Length - startIndex));
            var result = new T[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }
    }
}
