using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface IBufferPrimitivesFactoryService
    {
        PrimitiveTextBuffer CreateTextBuffer(ITextBuffer textBuffer);

        TextPoint CreateTextPoint(PrimitiveTextBuffer textBuffer, int position);

        TextRange CreateTextRange(PrimitiveTextBuffer textBuffer, TextPoint startPoint, TextPoint endPoint);
    }
}