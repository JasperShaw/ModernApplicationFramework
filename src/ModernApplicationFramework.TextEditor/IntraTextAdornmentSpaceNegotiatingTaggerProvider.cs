using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [ContentType("any")]
    [TagType(typeof(SpaceNegotiatingAdornmentTag))]
    [Export(typeof(IViewTaggerProvider))]
    internal sealed class IntraTextAdornmentSpaceNegotiatingTaggerProvider : IViewTaggerProvider
    {
        [Name("Intra Text Adornment")]
        [Order(After = "Text")]
        [Export]
        internal AdornmentLayerDefinition AdornmentLayer;

        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;
            return textView.Properties.GetOrCreateSingletonProperty(() => new IntraTextAdornmentManager(textView, TagAggregatorFactoryService.CreateTagAggregator<IntraTextAdornmentTag>(textView))).SpaceNegotiationTagger as ITagger<T>;
        }
    }
}