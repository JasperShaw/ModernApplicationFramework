using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IMappingTagSpan<out T> where T : ITag
    {
        IMappingSpan Span { get; }
        T Tag { get; }
    }
}