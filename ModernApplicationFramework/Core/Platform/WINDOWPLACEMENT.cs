using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform
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