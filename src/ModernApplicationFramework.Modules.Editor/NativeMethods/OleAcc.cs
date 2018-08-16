using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Modules.Editor.NativeMethods
{
    internal static class OleAcc
    {
        [DllImport("oleacc.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);
    }
}
