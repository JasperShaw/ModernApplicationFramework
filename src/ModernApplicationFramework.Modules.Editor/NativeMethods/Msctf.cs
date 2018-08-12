using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Modules.Editor.NativeMethods
{
    internal static class Msctf
    {
        [DllImport("msctf.dll")]
        internal static extern int TF_CreateThreadMgr(out NativeMethods.ITfThreadMgr threadMgr);
    }
}