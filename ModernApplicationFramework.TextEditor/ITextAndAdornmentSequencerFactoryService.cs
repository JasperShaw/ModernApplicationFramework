using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextAndAdornmentSequencerFactoryService
    {
        ITextAndAdornmentSequencer Create(ITextView view);
    }

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