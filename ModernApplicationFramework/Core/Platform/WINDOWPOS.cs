using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Core.Platform
{
    [StructLayout(LayoutKind.Sequential)]
    internal class Windowpos
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public uint flags;
    }
}