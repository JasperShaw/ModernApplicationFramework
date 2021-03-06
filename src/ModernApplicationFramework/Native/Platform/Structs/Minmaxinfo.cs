﻿using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Minmaxinfo
    {
        public Point ptReserved;
        public Point ptMaxSize;
        public Point ptMaxPosition;
        public Point ptMinTrackSize;
        public Point ptMaxTrackSize;
    }
}