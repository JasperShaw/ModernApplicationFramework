using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class VacuousTextDataModel : ITextDataModel
    {
        public IContentType ContentType => DocumentBuffer.ContentType;

        public event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;
        public ITextBuffer DocumentBuffer { get; }
        public ITextBuffer DataBuffer => DocumentBuffer;

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
