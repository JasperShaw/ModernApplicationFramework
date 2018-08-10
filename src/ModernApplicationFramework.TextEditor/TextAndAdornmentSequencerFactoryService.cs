using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextAndAdornmentSequencerFactoryService))]
    internal sealed class TextAndAdornmentSequencerFactoryService : ITextAndAdornmentSequencerFactoryService
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ITextAndAdornmentSequencer Create(ITextView view)
        {
            return TextAndAdornmentSequencer.GetSequencer(view, TagAggregatorFactoryService);
        }
    }
}