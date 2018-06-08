namespace ModernApplicationFramework.Modules.Toolbox.NativeMethods
{
    internal static class NativeMethods
    {
        public static int SignedLoword(int n)
        {
            return (short)(n & ushort.MaxValue);
        }

        public static int SignedHiword(int n)
        {
            return (short)(n >> 16 & ushort.MaxValue);
        }
    }
}
