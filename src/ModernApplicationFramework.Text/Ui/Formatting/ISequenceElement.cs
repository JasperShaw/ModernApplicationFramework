using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ISequenceElement
    {
        bool ShouldRenderText { get; }
        IMappingSpan Span { get; }
    }
}