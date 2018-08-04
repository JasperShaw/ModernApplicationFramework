using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor.Commanding.Commands
{
    public sealed class LeftKeyCommandArgs : EditorCommandArgs
    {
        public LeftKeyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}