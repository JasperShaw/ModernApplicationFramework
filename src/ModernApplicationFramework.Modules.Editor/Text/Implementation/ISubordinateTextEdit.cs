using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
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