using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITagSpan<out T> where T : ITag
    {
        SnapshotSpan Span { get; }
        T Tag { get; }
    }
}