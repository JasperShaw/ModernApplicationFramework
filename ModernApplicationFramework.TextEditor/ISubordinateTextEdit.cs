using System;

namespace ModernApplicationFramework.TextEditor
{
    internal interface ISubordinateTextEdit
    {
        void PreApply();

        bool CheckForCancellation(Action cancelAction);

        void FinalApply();

        ITextBuffer TextBuffer { get; }

        void CancelApplication();

        bool Canceled { get; }

        void RecordMasterChangeOffset(int masterChangeOffset);
    }
}