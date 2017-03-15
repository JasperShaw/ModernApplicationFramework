using System;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Core.Standard
{
    public static class Validate
    {
        public static void IsNotNull(object o, string paramName)
        {
            if (o == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNull(object o, string paramName)
        {
            if (o != null)
                throw new InvalidOperationException(paramName);
        }

        public static void IsNotEmpty(string s, string paramName)
        {
            if (s == string.Empty)
                throw new ArgumentException(paramName);
        }

        public static void IsNotEmpty(Guid g, string paramName)
        {
            if (g == Guid.Empty)
                throw new ArgumentException(paramName);
        }

        public static void IsNotWhiteSpace(string s, string paramName)
        {
            if (s != null && string.IsNullOrWhiteSpace(s))
                throw new ArgumentException(paramName);
        }

        public static void IsNotNullAndNotEmpty(string s, string paramName)
        {
            IsNotNull(s, paramName);
            IsNotEmpty(s, paramName);
        }

        public static void IsNotNullAndNotWhiteSpace(string s, string paramName)
        {
            IsNotNull(s, paramName);
            IsNotWhiteSpace(s, paramName);
        }

        public static void IsEqual(int value, int expectedValue, string paramName)
        {
            if (value != expectedValue)
                throw new ArgumentException(value.ToString(), expectedValue.ToString());
        }

        public static void IsEqual(uint value, uint expectedValue, string paramName)
        {
            if ((int)value != (int)expectedValue)
                throw new ArgumentException(value.ToString(), expectedValue.ToString());
        }

        public static void IsNotEqual(int value, int unexpectedValue, string paramName)
        {
            if (value == unexpectedValue)
                throw new ArgumentException(value.ToString(), paramName);
        }

        public static void IsNotEqual(uint value, uint unexpectedValue, string paramName)
        {
            if ((int)value == (int)unexpectedValue)
                throw new ArgumentException(value.ToString(), paramName);
        }

        public static void IsWithinRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, value.ToString());
            }
        }

        public static void IsWithinRange(long value, long min, long max, string paramName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, value.ToString());
            }
        }

        public static void IsWithinRange(uint value, uint min, uint max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(value.ToString());
        }

        public static void IsWithinRange(ulong value, ulong min, ulong max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(value.ToString());
        }

        public static void IsNormalized(string path, string paramName)
        {
            if (!PathUtilities.IsNormalized(path))
                throw new ArgumentException(path, paramName);
        }
    }
}
