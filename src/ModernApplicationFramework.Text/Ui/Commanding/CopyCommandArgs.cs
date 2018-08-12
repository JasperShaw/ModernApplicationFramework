using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.Commanding;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public sealed class CopyCommandArgs : EditorCommandArgs
    {
        public CopyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}