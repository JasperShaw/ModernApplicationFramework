using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.NativeMethods
{
    internal static class Msctf
    {
        [DllImport("msctf.dll")]
        internal static extern int TF_CreateThreadMgr(out NativeMethods.ITfThreadMgr threadMgr);
    }
}
