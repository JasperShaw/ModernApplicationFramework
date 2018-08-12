using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.TextAndAdornmentSequencer
{
    internal sealed class TextSpanElement : ISequenceElement
    {
        public bool ShouldRenderText => true;

        public IMappingSpan Span { get; }

        public TextSpanElement(IMappingSpan span)
        {
            Span = span;
        }

        public override string ToString()
        {
            return "Text Element span " + Span;
        }
    }
}