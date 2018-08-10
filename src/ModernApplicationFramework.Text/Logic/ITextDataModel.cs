using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic
{
    public interface ITextDataModel
    {
        event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;
        IContentType ContentType { get; }

        ITextBuffer DataBuffer { get; }

        ITextBuffer DocumentBuffer { get; }
    }
}