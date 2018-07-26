using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextContentChangingEventArgs : EventArgs
    {
        private readonly Action<TextContentChangingEventArgs> _cancelAction;

        public bool Canceled { get; private set; }

        public ITextSnapshot Before { get; }

        public object EditTag { get; }

        public TextContentChangingEventArgs(ITextSnapshot beforeSnapshot, object editTag, Action<TextContentChangingEventArgs> cancelAction)
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

        public ITextVersion BeforeVersion => Before.Version;
    }
}