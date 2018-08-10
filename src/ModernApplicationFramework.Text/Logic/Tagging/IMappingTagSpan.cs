using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface IMappingTagSpan<out T> where T : ITag
    {
        T Tag { get; }

        IMappingSpan Span { get; }
    }
}