﻿using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TrackingTagSpan<T> where T : ITag
    {
        public T Tag { get; }

        public ITrackingSpan Span { get; }

        public TrackingTagSpan(ITrackingSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span ?? throw new ArgumentNullException(nameof(span));
            Tag = tag;
        }
    }
}
