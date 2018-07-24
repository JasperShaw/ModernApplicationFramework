using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextBufferFactoryService
    {
        IContentType TextContentType { get; }

        IContentType PlaintextContentType { get; }

        IContentType InertContentType { get; }

        ITextBuffer CreateTextBuffer();

        ITextBuffer CreateTextBuffer(IContentType contentType);

        ITextBuffer CreateTextBuffer(string text, IContentType contentType);

        ITextBuffer CreateTextBuffer(SnapshotSpan span, IContentType contentType);

        event EventHandler<TextBufferCreatedEventArgs> TextBufferCreated;
    }
}