namespace ModernApplicationFramework.TextEditor.Text.Formatting
{
    public interface ILineTransformSource
    {
        LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement);
    }
}