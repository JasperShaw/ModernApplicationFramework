using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace ModernApplicationFramework.Modules.Editor.NativeMethods
{
    internal static class NativeMethods
    {
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct RECONVERTSTRING
        {
            public int dwSize;
            public int dwVersion;
            public int dwStrLen;
            public int dwStrOffset;
            public int dwCompStrLen;
            public int dwCompStrOffset;
            public int dwTargetStrLen;
            public int dwTargetStrOffset;
        }

        public struct Monitorinfo
        {
            public int CbSize;
            public int DwFlags;
            public RECT RcMonitor;
            public RECT RcWork;
        }

        public struct RECT
        {
            public int Bottom;
            public int Left;
            public int Right;
            public int Top;
        }

        public enum ImageType
        {
            ImageBitmap,
            ImageIcon,
            ImageCursor,
            ImageEnhmetafile
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class Logfont
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

        public struct Compositionform
        {
            public int DwStyle;
            public POINT PtCurrentPos;
            public RECT RcArea;
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