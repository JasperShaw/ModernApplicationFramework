using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.NativeMethods
{
    internal static class NativeMethods
    {
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        

        public struct POINT
        {
            public int x;
            public int y;
        }

        internal delegate int FontEnumProc(ref EnumLogFont lpelf, ref NewTextMetric lpntm, uint fontType, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct EnumLogFont
        {
            public Logfont LogFont;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string FullName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Style;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct NewTextMetric
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
            public uint ntmFlags;
            public uint ntmSizeEM;
            public uint ntmCellHeight;
            public uint ntmAvgWidth;
        }
    }
}
