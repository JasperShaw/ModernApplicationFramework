using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IEditorPrimitivesFactoryService
    {
        IViewPrimitives GetViewPrimitives(ITextView textView);

        IBufferPrimitives GetBufferPrimitives(ITextBuffer textBuffer);
    }
}