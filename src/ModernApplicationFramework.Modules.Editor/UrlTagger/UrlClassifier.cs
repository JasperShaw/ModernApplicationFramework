using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    internal sealed class UrlClassifier : ITagger<ClassificationTag>, IDisposable
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private readonly ITextView _textView;
        private ITagAggregator<IUrlTag> _urlTagAggregator;
        private readonly IClassificationType _urlClassification;
        private bool _enabled;
        private bool _disposed;

        public UrlClassifier(ITextView textView, ITagAggregator<IUrlTag> urlTagAggregator, IClassificationType urlClassification)
        {
            _textView = textView;
            _urlTagAggregator = urlTagAggregator;
            _urlClassification = urlClassification;
            _enabled = textView.Options.GetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId);
            textView.Options.OptionChanged += OnTextView_OptionChanged;
            urlTagAggregator.BatchedTagsChanged += UrlTagsChanged;
            textView.Closed += OnTextView_Closed;
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UrlClassifier));
            if (spans.Count != 0 && _enabled && _urlTagAggregator != null)
            {
                foreach (var tag in _urlTagAggregator.GetTags(spans))
                {
                    var tagSpan = tag.Span.GetSpans(_textView.TextSnapshot);
                    if (tagSpan.Count == 1 && tagSpan[0].Length == tag.Span.GetSpans(tag.Span.AnchorBuffer)[0].Length)
                        yield return new TagSpan<ClassificationTag>(tagSpan[0], new ClassificationTag(_urlClassification));
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            if (_urlTagAggregator != null)
            {
                _urlTagAggregator.Dispose();
                _urlTagAggregator = null;
            }

            _textView.Closed -= OnTextView_Closed;
            _textView.Options.OptionChanged -= OnTextView_OptionChanged;
        }

        private void OnTextView_OptionChanged(object sender, EditorOptionChangedEventArgs args)
        {
            if (_disposed || args.OptionId != DefaultTextViewOptions.DisplayUrlsAsHyperlinksId.Name)
                return;
            _enabled = _textView.Options.GetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId);
            var textSnapshot = _textView.TextSnapshot;
            RaiseTagsChanged(new SnapshotSpan(textSnapshot, 0, textSnapshot.Length));
        }

        private void UrlTagsChanged(object sender, BatchedTagsChangedEventArgs args)
        {
            if (_disposed)
                return;
            var snapshot = _textView.TextSnapshot;
            foreach (var changedSpan in new NormalizedSnapshotSpanCollection(
                args.Spans.SelectMany(mappingSpan => mappingSpan.GetSpans(snapshot))))
                RaiseTagsChanged(changedSpan);
        }

        private void RaiseTagsChanged(SnapshotSpan changedSpan)
        {
            EventHandler<SnapshotSpanEventArgs> tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(changedSpan));
        }

        private void OnTextView_Closed(object sender, EventArgs args)
        {
            Dispose();
        }
    }
}