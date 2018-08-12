using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ILineTransformSource
    {
        LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement);
    }
}