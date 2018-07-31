namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorPrimitivesFactoryService
    {
        IViewPrimitives GetViewPrimitives(ITextView textView);

        IBufferPrimitives GetBufferPrimitives(ITextBuffer textBuffer);
    }
}