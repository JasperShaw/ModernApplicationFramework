using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(ClassificationTag))]
    [TextViewRole("INTERACTIVE")]
    internal sealed class UrlClassifierProvider : IViewTaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry;
        [Import]
        internal IViewTagAggregatorFactoryService TagAggregatorFactory;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer != textView.TextBuffer)
                return null;
            var tagAggregator = TagAggregatorFactory.CreateTagAggregator<IUrlTag>(textView, (TagAggregatorOptions) 2);
            return new UrlClassifier(textView, tagAggregator, ClassificationTypeRegistry.GetClassificationType("url")) as ITagger<T>;
        }
    }
}