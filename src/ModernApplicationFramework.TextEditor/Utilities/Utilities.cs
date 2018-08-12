using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Utilities
{
    internal static class Utilities
    {
        internal static unsafe string GetCmdText(IntPtr pCmdTextInt)
        {
            if (pCmdTextInt == IntPtr.Zero)
                return string.Empty;
            var olecmdtextPtr = (Olecmdtext*)(void*)pCmdTextInt;
            return olecmdtextPtr->cwActual == 0U
                ? string.Empty
                : new string((char*) &olecmdtextPtr->rgwz, 0, (int) olecmdtextPtr->cwActual);
        }

        internal static void SetCmdText(IntPtr cmdTextPointer, string text)
        {
            var structure = (Olecmdtext)Marshal.PtrToStructure(cmdTextPointer, typeof(Olecmdtext));
            var charArray = text.ToCharArray();
            var destination = (IntPtr)((long)cmdTextPointer + (long)Marshal.OffsetOf(typeof(Olecmdtext), nameof(Olecmdtext.rgwz)));
            var ptr = (IntPtr)((long)cmdTextPointer + (long)Marshal.OffsetOf(typeof(Olecmdtext), nameof(Olecmdtext.cwActual)));
            var length = (int)Math.Min(structure.cwBuf - 1U, charArray.Length);
            Marshal.Copy(charArray, 0, destination, length);
            Marshal.WriteInt16((IntPtr)((long)destination + length * 2L), 0);
            var val = length + 1;
            Marshal.WriteInt32(ptr, val);
        }
    }
}