﻿using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.NativeMethods
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string moduleName);
    }
}
