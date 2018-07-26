using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ModernApplicationFramework.TextEditor.NativeMethods
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out NativeMethods.ByHandleFileInformation lpFileInformation);
    }
}
