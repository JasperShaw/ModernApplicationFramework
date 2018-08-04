namespace ModernApplicationFramework.TextEditor.Commanding.Commands
{
    public sealed class BackspaceKeyCommandArgs : EditorCommandArgs
    {
        public BackspaceKeyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}