using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data
{
    public sealed class NormalizedSnapshotSpanCollection : IList<SnapshotSpan>, IList
    {
        public static readonly NormalizedSnapshotSpanCollection Empty = new NormalizedSnapshotSpanCollection();
        private readonly ITextSnapshot _snapshot;
        private readonly Span _span;
        private readonly NormalizedSpanCollection _spans;

        public int Count
        {
            get
            {
                if (_spans != null)
                    return _spans.Count;
                return _snapshot != null ? 1 : 0;
            }
        }

        bool IList.IsFixedSize => true;

        bool IList.IsReadOnly => true;

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot
        {
            get
            {
                if (!(_spans != null))
                    return this;
                return ((ICollection) _spans).SyncRoot;
            }
        }

        bool ICollection<SnapshotSpan>.IsReadOnly => true;

        public NormalizedSnapshotSpanCollection()
        {
        }

        public NormalizedSnapshotSpanCollection(SnapshotSpan span)
        {
            _snapshot = span.Snapshot ?? throw new ArgumentException();
            _span = span;
        }

        public NormalizedSnapshotSpanCollection(ITextSnapshot snapshot, NormalizedSpanCollection spans)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            if (spans.Count > 0 && spans[spans.Count - 1].End > snapshot.Length)
                throw new ArgumentException();
            if (spans.Count == 1)
            {
                _snapshot = snapshot;
                _span = spans[0];
            }
            else
            {
                if (spans.Count <= 1)
                    return;
                _snapshot = snapshot;
                _spans = spans;
            }
        }

        public NormalizedSnapshotSpanCollection(ITextSnapshot snapshot, IEnumerable<Span> spans)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            using (var enumerator = spans.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return;
                _snapshot = snapshot;
                var current = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    _span = current;
                    if (current.End > snapshot.Length)
                        throw new ArgumentException();
                }
                else
                {
                    _spans = new NormalizedSpanCollection(spans);
                    if (_spans[_spans.Count - 1].End > snapshot.Length)
                        throw new ArgumentException();
                }
            }
        }

        public NormalizedSnapshotSpanCollection(ITextSnapshot snapshot, IList<Span> spans)
        {
            if (spans == null)
                throw new ArgumentNullException(nameof(spans));
            if (spans.Count == 0)
                return;
            _snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            if (spans.Count == 1)
            {
                _span = spans[0];
                if (_span.End > snapshot.Length)
                    throw new ArgumentException();
            }
            else
            {
                _spans = new NormalizedSpanCollection(spans);
                if (_spans[_spans.Count - 1].End > snapshot.Length)
                    throw new ArgumentException();
            }
        }

        public NormalizedSnapshotSpanCollection(IEnumerable<SnapshotSpan> snapshotSpans)
        {
            if (snapshotSpans == null)
                throw new ArgumentNullException(nameof(snapshotSpans));
            using (var enumerator = snapshotSpans.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return;
                var current1 = enumerator.Current;
                _snapshot = current1.Snapshot;
                if (!enumerator.MoveNext())
                {
                    _span = current1.Span;
                }
                else
                {
                    var flag = true;
                    var spanList = new List<Span>();
                    var span1 = current1.Span;
                    spanList.Add(span1);
                    var end = span1.End;
                    do
                    {
                        var current2 = enumerator.Current;
                        if (current2.Snapshot != _snapshot)
                        {
                            if (current2.Snapshot == null)
                                throw new ArgumentException();
                            throw new ArgumentException();
                        }

                        var span2 = current2.Span;
                        spanList.Add(span2);
                        if (span2.Start <= end)
                            flag = false;
                        end = span2.End;
                    } while (enumerator.MoveNext());

                    _spans = flag
                        ? NormalizedSpanCollection.CreateFromNormalizedSpans(spanList)
                        : new NormalizedSpanCollection(spanList);
                }
            }
        }

        public NormalizedSnapshotSpanCollection(IList<SnapshotSpan> snapshotSpans)
        {
            if (snapshotSpans == null)
                throw new ArgumentNullException(nameof(snapshotSpans));
            if (snapshotSpans.Count == 0)
                return;
            _snapshot = snapshotSpans[0].Snapshot;
            if (_snapshot == null)
                throw new ArgumentException();
            if (snapshotSpans.Count == 1)
            {
                _span = snapshotSpans[0].Span;
            }
            else
            {
                var flag = true;
                var spanList = new List<Span>(snapshotSpans.Count);
                var span1 = snapshotSpans[0].Span;
                spanList.Add(span1);
                var end = span1.End;
                for (var index = 1; index < snapshotSpans.Count; ++index)
                {
                    if (snapshotSpans[index].Snapshot != _snapshot)
                    {
                        if (snapshotSpans[index].Snapshot == null)
                            throw new ArgumentException();
                        throw new ArgumentException();
                    }

                    var span2 = snapshotSpans[index].Span;
                    spanList.Add(span2);
                    if (span2.Start <= end)
                        flag = false;
                    end = span2.End;
                }

                _spans = flag
                    ? NormalizedSpanCollection.CreateFromNormalizedSpans(spanList)
                    : new NormalizedSpanCollection(spanList);
            }
        }

        public NormalizedSnapshotSpanCollection(ITextSnapshot snapshot, Span span)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));
            if (span.End > snapshot.Length)
                throw new ArgumentException();
            _snapshot = snapshot;
            _span = span;
        }

        public SnapshotSpan this[int index]
        {
            get
            {
                if (_spans != null)
                    return new SnapshotSpan(_snapshot, _spans[index]);
                if (_snapshot != null && index == 0)
                    return new SnapshotSpan(_snapshot, _span);
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            set => throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get
            {
                if (_spans != null)
                    return new SnapshotSpan(_snapshot, _spans[index]);
                if (_snapshot != null && index == 0)
                    return new SnapshotSpan(_snapshot, _span);
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            set => throw new NotSupportedException();
        }

        public static NormalizedSnapshotSpanCollection Difference(NormalizedSnapshotSpanCollection left,
            NormalizedSnapshotSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0 || right.Count == 0)
                return left;
            if (left._snapshot != right._snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection1 = left._spans;
            if ((object) normalizedSpanCollection1 == null)
                normalizedSpanCollection1 = new NormalizedSpanCollection(left._span);
            var left1 = normalizedSpanCollection1;
            var normalizedSpanCollection2 = right._spans;
            if ((object) normalizedSpanCollection2 == null)
                normalizedSpanCollection2 = new NormalizedSpanCollection(right._span);
            var right1 = normalizedSpanCollection2;
            return new NormalizedSnapshotSpanCollection(left[0].Snapshot,
                NormalizedSpanCollection.Difference(left1, right1));
        }

        public static NormalizedSnapshotSpanCollection Intersection(NormalizedSnapshotSpanCollection left,
            NormalizedSnapshotSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return left;
            if (right.Count == 0)
                return right;
            if (left._snapshot != right._snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection1 = left._spans;
            if ((object) normalizedSpanCollection1 == null)
                normalizedSpanCollection1 = new NormalizedSpanCollection(left._span);
            var left1 = normalizedSpanCollection1;
            var normalizedSpanCollection2 = right._spans;
            if ((object) normalizedSpanCollection2 == null)
                normalizedSpanCollection2 = new NormalizedSpanCollection(right._span);
            var right1 = normalizedSpanCollection2;
            return new NormalizedSnapshotSpanCollection(left[0].Snapshot,
                NormalizedSpanCollection.Intersection(left1, right1));
        }

        public static bool operator ==(NormalizedSnapshotSpanCollection left, NormalizedSnapshotSpanCollection right)
        {
            if ((object) left == right)
                return true;
            if ((object) left == null || (object) right == null || left.Count != right.Count)
                return false;
            for (var index = 0; index < left.Count; ++index)
                if (left[index] != right[index])
                    return false;
            return true;
        }

        public static implicit operator NormalizedSpanCollection(NormalizedSnapshotSpanCollection spans)
        {
            if (spans == null)
                return null;
            if (spans._spans != null)
                return spans._spans;
            if (spans._snapshot != null)
                return new NormalizedSpanCollection(spans._span);
            return NormalizedSpanCollection.Empty;
        }

        public static bool operator !=(NormalizedSnapshotSpanCollection left, NormalizedSnapshotSpanCollection right)
        {
            return !(left == right);
        }

        public static NormalizedSnapshotSpanCollection Overlap(NormalizedSnapshotSpanCollection left,
            NormalizedSnapshotSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return left;
            if (right.Count == 0)
                return right;
            if (left._snapshot != right._snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection1 = left._spans;
            if ((object) normalizedSpanCollection1 == null)
                normalizedSpanCollection1 = new NormalizedSpanCollection(left._span);
            var left1 = normalizedSpanCollection1;
            var normalizedSpanCollection2 = right._spans;
            if ((object) normalizedSpanCollection2 == null)
                normalizedSpanCollection2 = new NormalizedSpanCollection(right._span);
            var right1 = normalizedSpanCollection2;
            return new NormalizedSnapshotSpanCollection(left[0].Snapshot,
                NormalizedSpanCollection.Overlap(left1, right1));
        }

        public static NormalizedSnapshotSpanCollection Union(NormalizedSnapshotSpanCollection left,
            NormalizedSnapshotSpanCollection right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            if (left.Count == 0)
                return right;
            if (right.Count == 0)
                return left;
            if (left._snapshot != right._snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection1 = left._spans;
            if ((object) normalizedSpanCollection1 == null)
                normalizedSpanCollection1 = new NormalizedSpanCollection(left._span);
            var left1 = normalizedSpanCollection1;
            var normalizedSpanCollection2 = right._spans;
            if ((object) normalizedSpanCollection2 == null)
                normalizedSpanCollection2 = new NormalizedSpanCollection(right._span);
            var right1 = normalizedSpanCollection2;
            return new NormalizedSnapshotSpanCollection(left[0].Snapshot,
                NormalizedSpanCollection.Union(left1, right1));
        }

        public NormalizedSnapshotSpanCollection CloneAndTrackTo(ITextSnapshot targetSnapshot, SpanTrackingMode mode)
        {
            if (targetSnapshot == null)
                throw new ArgumentNullException(nameof(targetSnapshot));
            switch (mode)
            {
                case SpanTrackingMode.EdgeExclusive:
                case SpanTrackingMode.EdgeInclusive:
                case SpanTrackingMode.EdgePositive:
                case SpanTrackingMode.EdgeNegative:
                case SpanTrackingMode.Custom:
                    if (_snapshot == null)
                        return Empty;
                    if (targetSnapshot.TextBuffer != _snapshot.TextBuffer)
                        throw new ArgumentException(
                            "this.Snapshot and targetSnapshot must be from the same ITextBuffer");
                    if (_snapshot == targetSnapshot)
                        return this;
                    if (_spans == null)
                    {
                        var span = targetSnapshot.Version.VersionNumber > _snapshot.Version.VersionNumber
                            ? Tracking.TrackSpanForwardInTime(mode, _span, _snapshot.Version, targetSnapshot.Version)
                            : Tracking.TrackSpanBackwardInTime(mode, _span, _snapshot.Version, targetSnapshot.Version);
                        return new NormalizedSnapshotSpanCollection(targetSnapshot, span);
                    }

                    var spanArray = new Span[_spans.Count];
                    for (var index = 0; index < _spans.Count; ++index)
                        spanArray[index] = targetSnapshot.Version.VersionNumber > _snapshot.Version.VersionNumber
                            ? Tracking.TrackSpanForwardInTime(mode, _spans[index], _snapshot.Version,
                                targetSnapshot.Version)
                            : Tracking.TrackSpanBackwardInTime(mode, _spans[index], _snapshot.Version,
                                targetSnapshot.Version);
                    return new NormalizedSnapshotSpanCollection(targetSnapshot, spanArray);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public bool Contains(SnapshotSpan item)
        {
            if (_spans != null)
            {
                if (item.Snapshot == _snapshot)
                    return _spans.Contains(item);
                return false;
            }

            if (_snapshot == null || item.Snapshot != _snapshot)
                return false;
            return item.Span == _span;
        }

        public bool Contains(object value)
        {
            if (!(value is SnapshotSpan))
                return false;
            var snapshotSpan = (SnapshotSpan) value;
            if (_spans != null)
            {
                if (_snapshot == snapshotSpan.Snapshot)
                    return _spans.Contains(snapshotSpan.Span);
                return false;
            }

            if (_snapshot == null || _snapshot != snapshotSpan.Snapshot)
                return false;
            return _span == snapshotSpan.Span;
        }

        public void CopyTo(SnapshotSpan[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length || Count > array.Length - arrayIndex)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (_spans != null)
            {
                foreach (var t in _spans)
                    array[arrayIndex++] = new SnapshotSpan(_snapshot, t);
            }
            else
            {
                if (_snapshot == null)
                    return;
                array[arrayIndex] = new SnapshotSpan(_snapshot, _span);
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0 || index > array.Length || Count > array.Length - index)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Rank != 1)
                throw new ArgumentException();
            if (_spans != null)
            {
                foreach (var t in _spans)
                    array.SetValue(new SnapshotSpan(_snapshot, t), index++);
            }
            else
            {
                if (_snapshot == null)
                    return;
                array.SetValue(new SnapshotSpan(_snapshot, _span), index);
            }
        }

        public override bool Equals(object obj)
        {
            return this == obj as NormalizedSnapshotSpanCollection;
        }

        public IEnumerator<SnapshotSpan> GetEnumerator()
        {
            if (_spans != null)
                foreach (var span in _spans)
                    yield return new SnapshotSpan(_snapshot, span);
            else if (_snapshot != null)
                yield return new SnapshotSpan(_snapshot, _span);
        }

        public override int GetHashCode()
        {
            if (!(_spans != null))
                return _span.GetHashCode();
            return _spans.GetHashCode();
        }

        public int IndexOf(SnapshotSpan item)
        {
            if (_snapshot == item.Snapshot)
            {
                if (_spans != null)
                    return _spans.IndexOf(item.Span);
                if (_snapshot != null && _span == item.Span)
                    return 0;
            }

            return -1;
        }

        public int IndexOf(object value)
        {
            if (value is SnapshotSpan snapshotSpan)
                if (_snapshot == snapshotSpan.Snapshot)
                {
                    if (_spans != null)
                        return _spans.IndexOf(snapshotSpan.Span);
                    if (_snapshot != null && _span == snapshotSpan.Span)
                        return 0;
                }

            return -1;
        }

        public bool IntersectsWith(NormalizedSnapshotSpanCollection set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));
            if (set.Count == 0 || Count == 0)
                return false;
            if (set._snapshot != _snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection = _spans;
            if ((object) normalizedSpanCollection == null)
                normalizedSpanCollection = new NormalizedSpanCollection(_span);
            return normalizedSpanCollection.IntersectsWith(set);
        }

        public bool IntersectsWith(SnapshotSpan span)
        {
            if (_snapshot == null)
                return false;
            if (span.Snapshot != _snapshot)
                throw new ArgumentException();
            if (_spans != null)
                return _spans.IntersectsWith(span);
            return _span.IntersectsWith(span.Span);
        }

        public bool OverlapsWith(NormalizedSnapshotSpanCollection set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));
            if (set.Count == 0 || Count == 0)
                return false;
            if (set._snapshot != _snapshot)
                throw new ArgumentException();
            var normalizedSpanCollection = _spans;
            if ((object) normalizedSpanCollection == null)
                normalizedSpanCollection = new NormalizedSpanCollection(_span);
            return normalizedSpanCollection.OverlapsWith(set);
        }

        public bool OverlapsWith(SnapshotSpan span)
        {
            if (_snapshot == null)
                return false;
            if (span.Snapshot != _snapshot)
                throw new ArgumentException();
            if (_spans != null)
                return _spans.OverlapsWith(span.Span);
            return _span.OverlapsWith(span.Span);
        }

        public override string ToString()
        {
            if (!(_spans != null))
                return _span.ToString();
            return _spans.ToString();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<SnapshotSpan>.Add(SnapshotSpan item)
        {
            throw new NotSupportedException();
        }

        void ICollection<SnapshotSpan>.Clear()
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_spans != null)
                foreach (var span in _spans)
                    yield return new SnapshotSpan(_snapshot, span);
            else if (_snapshot != null)
                yield return new SnapshotSpan(_snapshot, _span);
        }

        void IList<SnapshotSpan>.Insert(int index, SnapshotSpan item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<SnapshotSpan>.Remove(SnapshotSpan item)
        {
            throw new NotSupportedException();
        }

        void IList<SnapshotSpan>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}