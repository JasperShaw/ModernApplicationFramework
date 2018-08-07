﻿using System.Runtime.InteropServices;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ColorableItemInfo
    {
        public Color Foreground;
        public Color Background;
        public FontFlags FontFlags;
    }
}