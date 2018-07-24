namespace ModernApplicationFramework.TextEditor
{
    public interface ITextBuffer
    {
        IContentType ContentType { get; }

        ITextSnapshot CurrentSnapshot { get; }
    }
}
