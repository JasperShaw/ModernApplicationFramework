using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Olecmdtext
    {
        public uint cmdtextf;
        public uint cwActual;
        public uint cwBuf;
        public IntPtr rgwz;
    }
}