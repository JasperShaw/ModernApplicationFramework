using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal class Windowplacement
    {
        public int flags;
        public int length = Marshal.SizeOf(typeof(Windowplacement));
        public Point ptMaxPosition;
        public Point ptMinPosition;
        public RECT rcNormalPosition;
        public int showCmd;
    }
}