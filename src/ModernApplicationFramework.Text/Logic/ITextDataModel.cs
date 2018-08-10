using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic
{
    public interface ITextDataModel
    {
        IContentType ContentType { get; }

        event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;

        ITextBuffer DocumentBuffer { get; }

        ITextBuffer DataBuffer { get; }
    }
}