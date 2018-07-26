using System;

namespace ModernApplicationFramework.TextEditor
{
    public class ContentTypeChangedEventArgs : TextSnapshotChangedEventArgs
    {
        public ContentTypeChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot, IContentType beforeContentType, IContentType afterContentType, object editTag)
            : base(beforeSnapshot, afterSnapshot, editTag)
        {
            BeforeContentType = beforeContentType ?? throw new ArgumentNullException(nameof(beforeContentType));
            AfterContentType = afterContentType ?? throw new ArgumentNullException(nameof(afterContentType));
        }

        public IContentType BeforeContentType { get; }

        public IContentType AfterContentType { get; }
    }
}