using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
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
