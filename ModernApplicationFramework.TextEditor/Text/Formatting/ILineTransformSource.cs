using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.TextEditor.Text.Formatting
{
    public interface ILineTransformSource
    {
        LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement);
    }
}