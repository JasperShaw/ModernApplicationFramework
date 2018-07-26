namespace ModernApplicationFramework.TextEditor
{
    public interface IBufferGraphFactoryService
    {
        IBufferGraph CreateBufferGraph(ITextBuffer textBuffer);
    }
}