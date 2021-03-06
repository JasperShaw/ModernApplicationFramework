﻿using System;
using System.IO;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextBufferFactoryService
    {
        IContentType TextContentType { get; }

        IContentType PlaintextContentType { get; }

        IContentType InertContentType { get; }

        ITextBuffer CreateTextBuffer();

        ITextBuffer CreateTextBuffer(IContentType contentType);

        ITextBuffer CreateTextBuffer(string text, IContentType contentType);

        ITextBuffer CreateTextBuffer(SnapshotSpan span, IContentType contentType);

        ITextBuffer CreateTextBuffer(TextReader reader, IContentType contentType, long length = -1, string traceId = "");

        event EventHandler<TextBufferCreatedEventArgs> TextBufferCreated;
    }
}