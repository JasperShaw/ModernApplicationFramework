using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.Text.Classification
{
    [Export(typeof(IClassifierAggregatorService))]
    [Export(typeof(IViewClassifierAggregatorService))]
    internal sealed class ClassifierAggregatorService : IClassifierAggregatorService, IViewClassifierAggregatorService
    {
        [Import]
        internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactory { get; set; }

        [Import]
        internal IViewTagAggregatorFactoryService ViewTagAggregatorFactory { get; set; }

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return new ClassifierAggregator(textBuffer, BufferTagAggregatorFactory, ClassificationTypeRegistry);
        }

        public IClassifier GetClassifier(ITextView textView)
        {
            return new ClassifierAggregator(textView, ViewTagAggregatorFactory, ClassificationTypeRegistry);
        }
    }
}