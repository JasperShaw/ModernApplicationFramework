namespace ModernApplicationFramework.TextEditor.Text
{
    internal class TextSnapshot : BaseSnapshot
    {
        public TextSnapshot(ITextBuffer textBuffer, ITextVersion version, StringRebuilder content)
            : base(version, content)
        {
            TextBufferHelper = textBuffer;
        }

        protected override ITextBuffer TextBufferHelper { get; }
    }
}
