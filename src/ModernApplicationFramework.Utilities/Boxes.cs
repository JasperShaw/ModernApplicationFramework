namespace ModernApplicationFramework.Utilities
{
    public static class Boxes
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

        public static object Box(double value)
        {
            if (value != 0.0)
                return value;
            return DoubleZero;
        }
    }
}