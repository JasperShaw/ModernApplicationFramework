using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(IElisionTag))]
    internal sealed class IntraTextAdornmentElisionTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;
            return textView.Properties.GetOrCreateSingletonProperty(() => new IntraTextAdornmentManager(textView, TagAggregatorFactoryService.CreateTagAggregator<IntraTextAdornmentTag>(textView))).HiddenRegionTagger as ITagger<T>;
        }
    }
}