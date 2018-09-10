using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands
{
    public sealed class SelectAllCommandArgs : EditorCommandArgs
    {
        public SelectAllCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}