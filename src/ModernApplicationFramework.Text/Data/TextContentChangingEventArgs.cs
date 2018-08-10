using System;

namespace ModernApplicationFramework.Text.Data
{
    public class TextContentChangingEventArgs : EventArgs
    {
        private readonly Action<TextContentChangingEventArgs> _cancelAction;

        public ITextSnapshot Before { get; }

        public ITextVersion BeforeVersion => Before.Version;

        public bool Canceled { get; private set; }

        public object EditTag { get; }

        public TextContentChangingEventArgs(ITextSnapshot beforeSnapshot, object editTag,
            Action<TextContentChangingEventArgs> cancelAction)
        {
            Canceled = false;
            Before = beforeSnapshot ?? throw new ArgumentNullException(nameof(beforeSnapshot));
            EditTag = editTag;
            _cancelAction = cancelAction;
        }

        public void Cancel()
        {
            if (Canceled)
                return;
            Canceled = true;
            _cancelAction?.Invoke(this);
        }
    }
}