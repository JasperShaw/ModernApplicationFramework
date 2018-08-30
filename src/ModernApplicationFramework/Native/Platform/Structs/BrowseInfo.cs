using System;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Native.Platform.Structs
{
    public delegate int BrowseCallbackProc(IntPtr hwnd, FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam);

    public struct BrowseInfo
    {
        public IntPtr HwndOwner;
        public IntPtr PidlRoot;
        public string PszDisplayName;
        public string LpszTitle;
        public BrowseInfoFlags UlFlags;
        public BrowseCallbackProc Lpfn;
        public IntPtr LParam;
        public int IImage;
    }
}
