using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface IViewTagAggregatorFactoryService
    {
        ITagAggregator<T> CreateTagAggregator<T>(ITextView textView) where T : ITag;

        ITagAggregator<T> CreateTagAggregator<T>(ITextView textView, TagAggregatorOptions options) where T : ITag;
    }
}