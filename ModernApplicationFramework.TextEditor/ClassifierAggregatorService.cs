using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor
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