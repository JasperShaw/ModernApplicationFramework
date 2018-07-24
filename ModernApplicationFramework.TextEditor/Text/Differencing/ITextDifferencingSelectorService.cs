namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    public interface ITextDifferencingSelectorService
    {
        ITextDifferencingService GetTextDifferencingService(IContentType contentType);

        ITextDifferencingService DefaultTextDifferencingService { get; }
    }
}