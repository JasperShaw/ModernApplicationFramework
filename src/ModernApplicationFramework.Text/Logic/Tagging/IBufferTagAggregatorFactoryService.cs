using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IBufferTagAggregatorFactoryService
    {
        ITagAggregator<T> CreateTagAggregator<T>(ITextBuffer textBuffer) where T : ITag;

        ITagAggregator<T> CreateTagAggregator<T>(ITextBuffer textBuffer, TagAggregatorOptions options) where T : ITag;
    }
}