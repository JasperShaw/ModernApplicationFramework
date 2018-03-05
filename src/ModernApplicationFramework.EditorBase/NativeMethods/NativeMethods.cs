using System;
using System.IO;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.EditorBase.NativeMethods
{
    internal static class NativeMethods
    {
        public static ShellFileType GetExeType(string file)
        {
            const uint shgfiExetype = 0x000002000;

            var type = ShellFileType.FileNotFound;
            if (!File.Exists(file))
                return type;
            ShFileinfo shinfo = new ShFileinfo();
            IntPtr ptr = Shell32.SHGetFileInfo(file, 128, ref shinfo, (uint)Marshal.SizeOf(shinfo), shgfiExetype);
            int wparam = ptr.ToInt32();
            int loWord = wparam & 0xffff;
            int hiWord = wparam >> 16;

            type = ShellFileType.Unknown;

            if (wparam != 0)
            {
                if (hiWord == 0x0000 && loWord == 0x5a4d)
                {
                    type = ShellFileType.Dos;
                }
                else if (hiWord == 0x0000 && loWord == 0x4550)
                {
                    type = ShellFileType.Console;
                }
                else if (hiWord != 0x0000 && (loWord == 0x454E || loWord == 0x4550 || loWord == 0x454C))
                {
                    type = ShellFileType.Windows;
                }
            }
            return type;
        }
    }
}
