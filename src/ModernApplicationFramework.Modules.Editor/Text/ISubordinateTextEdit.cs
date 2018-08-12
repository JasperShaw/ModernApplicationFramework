using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal interface ISubordinateTextEdit
    {
        bool Canceled { get; }

        ITextBuffer TextBuffer { get; }

        void CancelApplication();

        bool CheckForCancellation(Action cancelAction);

        void FinalApply();
        void PreApply();

        void RecordMasterChangeOffset(int masterChangeOffset);
    }
}