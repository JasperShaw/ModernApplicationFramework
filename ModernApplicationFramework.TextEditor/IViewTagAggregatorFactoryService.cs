namespace ModernApplicationFramework.TextEditor
{
    public interface IViewTagAggregatorFactoryService
    {
        ITagAggregator<T> CreateTagAggregator<T>(ITextView textView) where T : ITag;

        ITagAggregator<T> CreateTagAggregator<T>(ITextView textView, TagAggregatorOptions options) where T : ITag;
    }
}