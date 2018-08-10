using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class SimpleTagger<T> : ITagger<T> where T : ITag
    {
        private readonly List<TrackingTagSpan<T>> _trackingTagSpans = new List<TrackingTagSpan<T>>();
        private readonly object _mutex = new object();
        private readonly ITextBuffer _buffer;
        private int _batchNesting;
        private ITrackingSpan _batchSpan;

        public SimpleTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        private void StartBatch()
        {
            Interlocked.Increment(ref _batchNesting);
        }

        private void EndBatch()
        {
            if (Interlocked.Decrement(ref _batchNesting) != 0)
                return;
            var trackingSpan = Interlocked.Exchange(ref _batchSpan, null);
            if (trackingSpan == null)
                return;
            var tagsChanged = TagsChanged;
            tagsChanged?.Invoke(this, new SnapshotSpanEventArgs(trackingSpan.GetSpan(_buffer.CurrentSnapshot)));
        }

        private void UpdateBatchSpan(ITrackingSpan snapshotSpan)
        {
            var trackingSpan = snapshotSpan;
            if (_batchSpan != null)
            {
                var currentSnapshot = _buffer.CurrentSnapshot;
                var span1 = _batchSpan.GetSpan(currentSnapshot);
                var span2 = snapshotSpan.GetSpan(currentSnapshot);
                var start = span1.Start < span2.Start ? span1.Start : span2.Start;
                var end = span1.End > span2.End ? span1.End : span2.End;
                trackingSpan = currentSnapshot.CreateTrackingSpan(new SnapshotSpan(start, end), _batchSpan.TrackingMode);
            }
            _batchSpan = trackingSpan;
        }

        public TrackingTagSpan<T> CreateTagSpan(ITrackingSpan span, T tag)
        {
            if (span == null)
                throw new ArgumentNullException(nameof(span));
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            var trackingTagSpan = new TrackingTagSpan<T>(span, tag);
            StartBatch();
            try
            {
                lock (_mutex)
                {
                    _trackingTagSpans.Add(trackingTagSpan);
                    UpdateBatchSpan(trackingTagSpan.Span);
                }
            }
            finally
            {
                EndBatch();
            }
            return trackingTagSpan;
        }

        public bool RemoveTagSpan(TrackingTagSpan<T> tagSpan)
        {
            if (tagSpan == null)
                throw new ArgumentNullException(nameof(tagSpan));
            bool flag;
            StartBatch();
            try
            {
                lock (_mutex)
                {
                    flag = _trackingTagSpans.Remove(tagSpan);
                    if (flag)
                        UpdateBatchSpan(tagSpan.Span);
                }
            }
            finally
            {
                EndBatch();
            }
            return flag;
        }

        public int RemoveTagSpans(Predicate<TrackingTagSpan<T>> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            StartBatch();
            try
            {
                lock (_mutex)
                    return _trackingTagSpans.RemoveAll(tagSpan =>
                    {
                        if (!match(tagSpan))
                            return false;
                        UpdateBatchSpan(tagSpan.Span);
                        return true;
                    });
            }
            finally
            {
                EndBatch();
            }
        }

        public IEnumerable<TrackingTagSpan<T>> GetTaggedSpans(SnapshotSpan span)
        {
            IList<TrackingTagSpan<T>> source;
            lock (_mutex)
                source = new List<TrackingTagSpan<T>>(_trackingTagSpans);
            return source.Where(tagSpan => span.IntersectsWith(tagSpan.Span.GetSpan(span.Snapshot)));
        }

        public IDisposable Update()
        {
            return new Batch(this);
        }

        public IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                return Enumerable.Empty<ITagSpan<T>>();
            var tagSpans = (TrackingTagSpan<T>[])null;
            lock (_mutex)
            {
                if (_trackingTagSpans.Count > 0)
                    tagSpans = _trackingTagSpans.ToArray();
            }
            if (tagSpans == null)
                return Enumerable.Empty<ITagSpan<T>>();
            return Create(tagSpans, spans);
        }

        internal static IEnumerable<ITagSpan<T>> Create(TrackingTagSpan<T>[] tagSpans,
            NormalizedSnapshotSpanCollection querySpans)
        {
            return (from span in tagSpans
                let tagSnapshotSpan = span.Span.GetSpan(querySpans[0].Snapshot)
                where querySpans.Any(tagSnapshotSpan.IntersectsWith)
                select new TagSpan<T>(tagSnapshotSpan, span.Tag));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private class Batch : IDisposable
        {
            private SimpleTagger<T> _tagger;

            internal Batch(SimpleTagger<T> tagger)
            {
                _tagger = tagger ?? throw new ArgumentNullException(nameof(tagger));
                _tagger.StartBatch();
            }

            public void Dispose()
            {
                _tagger.EndBatch();
                _tagger = null;
                GC.SuppressFinalize(this);
            }
        }
    }
}