using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextDataModel
    {
        IContentType ContentType { get; }

        event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;

        ITextBuffer DocumentBuffer { get; }

        ITextBuffer DataBuffer { get; }
    }
}