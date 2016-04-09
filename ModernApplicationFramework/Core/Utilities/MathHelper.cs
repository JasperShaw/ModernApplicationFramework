using System;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class MathHelper
    {
        public static double MinMax(double value, double min, double max)
        {
            if (min > max)
                throw new ArgumentException("min>max");

            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }
    }
}