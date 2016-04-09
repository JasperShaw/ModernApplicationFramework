namespace ModernApplicationFramework.Core.Platform
{
    internal static class Boxes
    {
        public static readonly object BoolTrue = true;
        public static readonly object BoolFalse = false;

        public static object Box(bool value)
        {
            return !value ? BoolFalse : BoolTrue;
        }

        public static object Box(bool? nullableValue)
        {
            return !nullableValue.HasValue ? null : Box(nullableValue.Value);
        }
    }
}