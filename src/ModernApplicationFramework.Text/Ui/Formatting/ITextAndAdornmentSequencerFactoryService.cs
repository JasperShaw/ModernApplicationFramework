using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextAndAdornmentSequencerFactoryService
    {
        ITextAndAdornmentSequencer Create(ITextView view);
    }
}