using System;

namespace ModernApplicationFramework.Extended.Clipboard
{
    public class ClipboardPushEventArgs : EventArgs
    {
        public ClipboardPushOption Sender { get; }

        public ClipboardPushEventArgs(ClipboardPushOption sender)
        {
            Sender = sender;
        }
    }
}