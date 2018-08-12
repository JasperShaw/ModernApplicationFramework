using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class TextDataModel : ITextDataModel
    {
        public IContentType ContentType => DocumentBuffer.ContentType;

        public event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;
        public ITextBuffer DocumentBuffer { get; }
        public ITextBuffer DataBuffer { get; }

        public TextDataModel(ITextBuffer documentBuffer, ITextBuffer dataBuffer)
        {
            DocumentBuffer = documentBuffer;
            DataBuffer = dataBuffer;
            DocumentBuffer.ContentTypeChanged += OnContentTypeChanged;

        }

        private void OnContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            var contentTypeChanged = ContentTypeChanged;
            contentTypeChanged?.Invoke(this, new TextDataModelContentTypeChangedEventArgs(e.BeforeContentType, e.AfterContentType));
        }
    }
}
