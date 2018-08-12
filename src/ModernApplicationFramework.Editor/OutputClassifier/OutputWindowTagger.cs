using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Editor.OutputClassifier
{
    public class OutputWindowTagger : ITagger<IClassificationTag>
    {
        private readonly ITextBuffer _buffer;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        public OutputWindowTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            _buffer.Properties.TryGetProperty<OutputWindowStyleManager>(nameof(OutputWindowStyleManager), out var property);
            return property == null ? new List<ITagSpan<IClassificationTag>>() : property.GetColorizableSpans(spans);
        }
    }
}
