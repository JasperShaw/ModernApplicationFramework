using System;

namespace ModernApplicationFramework.Utilities
{
    public static class Validate
    {
        public static void IsNotNull(object o, string paramName)
        {
            if (o == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNotEmpty(string s, string paramName)
        {
            if (s == string.Empty)
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

        public static void IsWithinRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(paramName, value.ToString());
            }
        }
    }
}
