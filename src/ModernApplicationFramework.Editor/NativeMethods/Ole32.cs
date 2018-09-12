using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.NativeMethods
{
    internal static class Ole32
    {
        [DllImport("ole32.dll")]
        public static extern int OleGetClipboard([MarshalAs(UnmanagedType.IUnknown)] out object ppDataObj);
    }
}
