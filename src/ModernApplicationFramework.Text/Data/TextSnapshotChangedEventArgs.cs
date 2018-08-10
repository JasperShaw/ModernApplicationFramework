using System;

namespace ModernApplicationFramework.Text.Data
{
    public abstract class TextSnapshotChangedEventArgs : EventArgs
    {
        protected TextSnapshotChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot, object editTag)
        {
            Before = beforeSnapshot ?? throw new ArgumentNullException(nameof(beforeSnapshot));
            After = afterSnapshot ?? throw new ArgumentNullException(nameof(afterSnapshot));
            EditTag = editTag;
        }

        public ITextSnapshot Before { get; }

        public ITextSnapshot After { get; }

        public ITextVersion BeforeVersion => Before.Version;

        public ITextVersion AfterVersion => After.Version;

        public object EditTag { get; }
    }
}