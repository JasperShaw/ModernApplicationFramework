using ModernApplicationFramework.Basics.MostRecentlyUsedManager;

namespace ModernApplicationFramework.Extended.Clipboard
{
    public abstract class MruClipboard<T> : MruManager<T> where T : MruItem
    {
        protected virtual ClipboardPushOption SupportedOptions => ClipboardPushOption.Any;

        protected MruClipboard(int maxCount) : base(maxCount)
        {
            CopyCutWatcher.ClipboardPushed += CopyCutWatcher_ClipboardPushed;
        }

        protected abstract void OnClipboardPushed();

        private void CopyCutWatcher_ClipboardPushed(object sender, ClipboardPushEventArgs e)
        {
            if (SupportedOptions.HasFlag(e.Sender))
                OnClipboardPushed();
        }
    }
}
