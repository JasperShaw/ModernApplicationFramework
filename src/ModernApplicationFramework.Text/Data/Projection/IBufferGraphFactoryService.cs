namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IBufferGraphFactoryService
    {
        IBufferGraph CreateBufferGraph(ITextBuffer textBuffer);
    }
}