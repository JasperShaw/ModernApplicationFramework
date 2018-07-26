namespace ModernApplicationFramework.TextEditor
{
    internal interface IInternalTextBufferFactory
    {
        ITextBuffer CreateTextBuffer(string text, IContentType contentType);

        ITextBuffer CreateTextBuffer(string text, IContentType contentType, bool spurnGroup);

        IContentType TextContentType { get; }

        IContentType InertContentType { get; }

        IContentType ProjectionContentType { get; }
    }
}