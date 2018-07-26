using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextDataModelContentTypeChangedEventArgs : EventArgs
    {
        public IContentType BeforeContentType { get; }

        public IContentType AfterContentType { get; }

        public TextDataModelContentTypeChangedEventArgs(IContentType beforeContentType, IContentType afterContentType)
        {
            BeforeContentType = beforeContentType;
            AfterContentType = afterContentType;
        }
    }
}