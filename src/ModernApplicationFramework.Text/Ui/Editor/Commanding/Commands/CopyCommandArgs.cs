﻿using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor.Commanding.Commands
{
    public sealed class CopyCommandArgs : EditorCommandArgs
    {
        public CopyCommandArgs(ITextView textView, ITextBuffer subjectBuffer)
            : base(textView, subjectBuffer)
        {
        }
    }
}