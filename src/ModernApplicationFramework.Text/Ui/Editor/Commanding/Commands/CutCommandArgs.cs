using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands
{
    public sealed class CutCommandArgs : EditorCommandArgs
    {
        public CutCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}