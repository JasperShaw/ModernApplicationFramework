using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Commanding
{
    internal interface IActiveTextViewState
    {
        ITextView ActiveTextView { get; }

        ICommandTarget ActiveCommandTarget { get; }
    }
}