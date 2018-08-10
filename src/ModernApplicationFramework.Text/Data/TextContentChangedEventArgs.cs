namespace ModernApplicationFramework.Text.Data
{
    public class TextContentChangedEventArgs : TextSnapshotChangedEventArgs
    {
        public INormalizedTextChangeCollection Changes => Before.Version.Changes;

        public EditOptions Options { get; }

        public TextContentChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot,
            EditOptions options, object editTag)
            : base(beforeSnapshot, afterSnapshot, editTag)
        {
            Options = options;
        }
    }
}