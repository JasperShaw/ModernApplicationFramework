using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class PreviewTextMarkerTagger : ITagger<ITextMarkerTag>, IDisposable
    {
        private readonly ITextView _sourceView;
        private readonly ITextView _view;
        private ITagAggregator<ITextMarkerTag> _tagger;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public PreviewTextMarkerTagger(ITextView sourceView, ITextView view, ITagAggregator<ITextMarkerTag> tagger)
        {
            _sourceView = sourceView;
            _view = view;
            _tagger = tagger;
            _tagger.TagsChanged += OnTagsChanged;
        }

        public void Dispose()
        {
            if (_tagger == null)
                return;
            _tagger.TagsChanged -= OnTagsChanged;
            _tagger.Dispose();
            _tagger = null;
        }

        public IEnumerable<ITagSpan<ITextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_tagger == null || _view.IsClosed || _sourceView.IsClosed) yield break;
            foreach (var tag in _tagger.GetTags(spans))
            {
                var t = tag;
                foreach (var span in t.Span.GetSpans(spans[0].Snapshot))
                    yield return new TagSpan<ITextMarkerTag>(span, t.Tag);
            }
        }

        private void OnTagsChanged(object sender, TagsChangedEventArgs e)
        {
            var tagsChanged = TagsChanged;
            if (tagsChanged == null)
                return;
            foreach (var span in e.Span.GetSpans(_view.TextSnapshot))
                tagsChanged(this, new SnapshotSpanEventArgs(span));
        }
    }
}