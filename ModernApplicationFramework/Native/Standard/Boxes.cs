namespace ModernApplicationFramework.Native.Standard
{
    internal static class Boxes
    {
        public static readonly object BooleanTrue = true;
        public static readonly object BooleanFalse = false;
        public static readonly object Int32Zero = 0;
        public static readonly object Int32One = 1;
        public static readonly object UInt32Zero = 0U;
        public static readonly object UInt32One = 1U;
        public static readonly object UInt64Zero = 0UL;
        public static readonly object DoubleZero = 0.0;

        public static object Box(bool value)
        {
            return !value ? BooleanFalse : BooleanTrue;
        }

        public static object Box(bool? nullableValue)
        {
            return !nullableValue.HasValue ? null : Box(nullableValue.Value);
        }

        public static object Box(int value)
        {
            switch (value)
            {
                case 0:
                    return Int32Zero;
                case 1:
                    return Int32One;
            }
            return value;
        }

        public static object Box(uint value)
        {
            switch ((int)value)
            {
                case 0:
                    return UInt32Zero;
                case 1:
                    return UInt32One;
            }
            return value;
        }

        public static object Box(ulong value)
        {
            return (long)value != 0L ? value : UInt64Zero;
        }

        public static object Box(double value)
        {
            return value != 0.0 ? value : DoubleZero;
        }
    }
}