using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Commanding;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding
{
    public abstract class EditorCommandArgs : CommandArgs
    {
        public ITextBuffer SubjectBuffer { get; }

        public ITextView TextView { get; }

        protected EditorCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
        {
            var textView1 = textView;
            TextView = textView1 ?? throw new ArgumentNullException(nameof(textView));
            var textBuffer = subjectBuffer;
            SubjectBuffer = textBuffer ?? throw new ArgumentNullException(nameof(subjectBuffer));
        }
    }
}