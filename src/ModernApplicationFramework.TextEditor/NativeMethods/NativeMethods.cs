using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace ModernApplicationFramework.TextEditor.NativeMethods
{
    public static class NativeMethods
    {
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public enum ImageType
        {
            ImageBitmap,
            ImageIcon,
            ImageCursor,
            ImageEnhmetafile,
        }

        [Flags]
        public enum ImageFormatRequest
        {
            LrDefaultcolor = 0,
            LrMonochrome = 1,
            LrCopyreturnorg = 4,
            LrCopydeleteorg = 8,
            LrLoadfromfile = 16,
            LrDefaultsize = 64,
            LrLoadmap3Dcolors = 4096,
            LrCreatedibsection = 8192,
            LrCopyfromresource = 16384,
            LrShared = 32768
        }

        public struct COMPOSITIONFORM
        {
            public int dwStyle;
            public POINT ptCurrentPos;
            public RECT rcArea;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        public struct POINT
        {
            public int x;
            public int y;
        }

        public struct MONITORINFO
        {
            public int cbSize;
            public NativeMethods.RECT rcMonitor;
            public NativeMethods.RECT rcWork;
            public int dwFlags;
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("aa80e801-2021-11d2-93e0-0060b067b86e")]
        [ComImport]
        internal interface ITfThreadMgr
        {
            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            void Activate(out int clientId);

            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            void Deactivate();

            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            void CreateDocumentMgr(out object docMgr);

            void EnumDocumentMgrs(out object enumDocMgrs);

            void GetFocus(out IntPtr docMgr);

            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            void SetFocus(IntPtr docMgr);

            void AssociateFocus(IntPtr hwnd, object newDocMgr, out object prevDocMgr);

            void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);

            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetFunctionProvider(ref Guid classId, out object funcProvider);

            void EnumFunctionProviders(out object enumProviders);

            [SecurityCritical]
            [SuppressUnmanagedCodeSecurity]
            void GetGlobalCompartment(out object compartmentMgr);
        }
    }
}
