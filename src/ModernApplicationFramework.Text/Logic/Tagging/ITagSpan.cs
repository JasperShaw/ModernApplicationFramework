using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public interface ITagSpan<out T> where T : ITag
    {
        T Tag { get; }

        SnapshotSpan Span { get; }
    }
}