using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public class ContentTypeChangedEventArgs : TextSnapshotChangedEventArgs
    {
        public IContentType AfterContentType { get; }

        public IContentType BeforeContentType { get; }

        public ContentTypeChangedEventArgs(ITextSnapshot beforeSnapshot, ITextSnapshot afterSnapshot,
            IContentType beforeContentType, IContentType afterContentType, object editTag)
            : base(beforeSnapshot, afterSnapshot, editTag)
        {
            BeforeContentType = beforeContentType ?? throw new ArgumentNullException(nameof(beforeContentType));
            AfterContentType = afterContentType ?? throw new ArgumentNullException(nameof(afterContentType));
        }
    }
}