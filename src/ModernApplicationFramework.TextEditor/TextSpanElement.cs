using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class TextSpanElement : ISequenceElement
    {
        public TextSpanElement(IMappingSpan span)
        {
            Span = span;
        }

        public IMappingSpan Span { get; }

        public bool ShouldRenderText => true;

        public override string ToString()
        {
            return "Text Element span " + Span.ToString();
        }
    }
}