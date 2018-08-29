﻿using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Docking.NativeMethods
{
    internal static class Shlwapi
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string value1, string value2);
    }
}
