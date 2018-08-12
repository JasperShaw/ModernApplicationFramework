using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor
{
    internal class VacuousTextDataModel : ITextDataModel
    {
        public event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;
        public IContentType ContentType => DocumentBuffer.ContentType;
        public ITextBuffer DataBuffer => DocumentBuffer;
        public ITextBuffer DocumentBuffer { get; }

        public VacuousTextDataModel(ITextBuffer documentBuffer)
        {
            DocumentBuffer = documentBuffer ?? throw new ArgumentNullException(nameof(documentBuffer));
            documentBuffer.ContentTypeChanged += OnDocumentBufferContentTypeChanged;
        }

        private void OnDocumentBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            var contentTypeChanged = ContentTypeChanged;
            contentTypeChanged?.Invoke(this,
                new TextDataModelContentTypeChangedEventArgs(e.BeforeContentType, e.AfterContentType));
        }
    }
}