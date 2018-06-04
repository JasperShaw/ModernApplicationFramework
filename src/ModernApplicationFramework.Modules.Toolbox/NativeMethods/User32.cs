using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Modules.Toolbox.NativeMethods
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();
    }
}
