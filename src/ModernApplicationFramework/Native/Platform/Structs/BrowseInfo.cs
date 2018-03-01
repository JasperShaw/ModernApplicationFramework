using System;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    public delegate int BrowseCallbackProc(IntPtr hwnd, FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam);

    public struct BrowseInfo
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public string pszDisplayName;
        public string lpszTitle;
        public BrowseInfoFlags ulFlags;
        public BrowseCallbackProc lpfn;
        public IntPtr lParam;
        public int iImage;
    }
}
