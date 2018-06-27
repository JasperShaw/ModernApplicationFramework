using System;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;

namespace ModernApplicationFramework.Extended.Clipboard
{
    public abstract class StackClipboard<T> : MruManager<T> where T : MruItem
    {
        protected StackClipboard(int maxCount) : base(maxCount)
        {
            CopyCutWatcher.ClipboardPushed += CopyCutWatcher_ClipboardPushed;
        }

        protected abstract void OnClipboardPushed();

        private void CopyCutWatcher_ClipboardPushed(object sender, EventArgs e)
        {
            OnClipboardPushed();
        }
    }
  
    public static class CopyCutWatcher
    {
        public static event EventHandler ClipboardPushed;

        internal static void PushClipboard()
        {
            ClipboardPushed?.Invoke(null, EventArgs.Empty);
        }
    }
}
