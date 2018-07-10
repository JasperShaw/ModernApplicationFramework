using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Modules.Toolbox.NativeMethods
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetMessagePos();

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
}
