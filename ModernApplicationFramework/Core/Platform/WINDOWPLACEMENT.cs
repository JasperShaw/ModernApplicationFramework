using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform
{
    [StructLayout(LayoutKind.Sequential)]
    internal class Windowplacement
    {
        public int length = Marshal.SizeOf(typeof(Windowplacement));
        public int flags;
        public int showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public RECT rcNormalPosition;
    }
}
