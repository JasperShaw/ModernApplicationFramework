using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Core.Platform.Structs;
using RECT = ModernApplicationFramework.Core.Platform.Structs.RECT;

namespace ModernApplicationFramework.Core.NativeMethods
{
    internal static class Gdi32
    {
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateSolidBrush(int colorref);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);


        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRectRgnIndirect(ref RECT lprc);


        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int CombineRgn(IntPtr hrngDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateDIBSection(IntPtr hdc, ref BitmapInfo pbmi, uint iUsage,
            out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

    }
}