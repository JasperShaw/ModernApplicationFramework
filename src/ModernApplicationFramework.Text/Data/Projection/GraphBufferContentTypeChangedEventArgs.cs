﻿using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public class GraphBufferContentTypeChangedEventArgs : EventArgs
    {
        public IContentType AfterContentType { get; }

        public IContentType BeforeContentType { get; }

        public ITextBuffer TextBuffer { get; }

        public GraphBufferContentTypeChangedEventArgs(ITextBuffer textBuffer, IContentType beforeContentType,
            IContentType afterContentType)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
            BeforeContentType = beforeContentType ?? throw new ArgumentNullException(nameof(beforeContentType));
            AfterContentType = afterContentType ?? throw new ArgumentNullException(nameof(afterContentType));
        }
    }
}