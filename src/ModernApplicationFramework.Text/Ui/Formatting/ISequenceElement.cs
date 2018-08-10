using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ISequenceElement
    {
        IMappingSpan Span { get; }

        bool ShouldRenderText { get; }
    }
}