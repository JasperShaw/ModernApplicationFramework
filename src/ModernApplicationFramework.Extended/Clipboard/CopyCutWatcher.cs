using System;

namespace ModernApplicationFramework.Extended.Clipboard
{
    public static class CopyCutWatcher
    {
        public static event EventHandler<ClipboardPushEventArgs> ClipboardPushed;

        internal static void PushClipboard(ClipboardPushOption option)
        {
            ClipboardPushed?.Invoke(null, new ClipboardPushEventArgs(option));
        }
    }
}