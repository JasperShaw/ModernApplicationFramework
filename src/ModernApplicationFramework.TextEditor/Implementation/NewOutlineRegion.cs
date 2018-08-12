﻿using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NewOutlineRegion
    {
        public uint dwState;
        public TextSpan tsHiddenText;
    }
}