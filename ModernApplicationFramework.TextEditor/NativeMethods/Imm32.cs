using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ModernApplicationFramework.TextEditor.NativeMethods
{
    internal static class Imm32
    {
        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr ImmEscapeW(IntPtr hkl, IntPtr himc, int esc, IntPtr lpBuf);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hImc);

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern int ImmGetCompositionStringW(IntPtr hImc, int dwIndex, StringBuilder lpBuf, int dwBufLen);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImmNotifyIME(IntPtr immContext, int dwAction, int dwIndex, int dwValue);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hImc);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImmSetCompositionFontW(IntPtr hImc, IntPtr lplf);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ImmSetCompositionWindow(IntPtr hImc, IntPtr ptr);

    }
}
