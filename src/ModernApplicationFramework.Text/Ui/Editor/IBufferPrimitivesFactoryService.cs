using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IBufferPrimitivesFactoryService
    {
        PrimitiveTextBuffer CreateTextBuffer(ITextBuffer textBuffer);

        TextPoint CreateTextPoint(PrimitiveTextBuffer textBuffer, int position);

        TextRange CreateTextRange(PrimitiveTextBuffer textBuffer, TextPoint startPoint, TextPoint endPoint);
    }
}