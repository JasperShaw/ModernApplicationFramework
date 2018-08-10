using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic
{
    public class TextDataModelContentTypeChangedEventArgs : EventArgs
    {
        public IContentType AfterContentType { get; }
        public IContentType BeforeContentType { get; }

        public TextDataModelContentTypeChangedEventArgs(IContentType beforeContentType, IContentType afterContentType)
        {
            BeforeContentType = beforeContentType;
            AfterContentType = afterContentType;
        }
    }
}