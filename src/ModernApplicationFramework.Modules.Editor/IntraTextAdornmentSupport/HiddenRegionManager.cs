using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Modules.Editor.IntraTextAdornmentSupport
{
    internal sealed class HiddenRegionManager : IDisposable
    {
        private readonly IElisionBuffer _elisionBuffer;
        private readonly ITagAggregator<IElisionTag> _hiddenRegionTagAggregator;

        internal HiddenRegionManager(IElisionBuffer elisionBuffer,
            ITagAggregator<IElisionTag> hiddenRegionTagAggregator)
        {
            _elisionBuffer = elisionBuffer;
            _hiddenRegionTagAggregator = hiddenRegionTagAggregator;
            hiddenRegionTagAggregator.TagsChanged += HandleTagsChanged;
            var currentSnapshot = elisionBuffer.SourceBuffer.CurrentSnapshot;
            UpdateAfterChange(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length));
        }

        public void Dispose()
        {
            _hiddenRegionTagAggregator.TagsChanged -= HandleTagsChanged;
            _hiddenRegionTagAggregator.Dispose();
        }

        private static int SpanComparator(SnapshotSpan left, SnapshotSpan right)
        {
            var num = left.Start - right.Start;
            if (num != 0)
                return num;
            return left.End - right.End;
        }

        private void HandleTagsChanged(object sender, TagsChangedEventArgs args)
        {
            foreach (var span in args.Span.GetSpans(_elisionBuffer.SourceBuffer))
                UpdateAfterChange(span);
        }

        private void UpdateAfterChange(SnapshotSpan changedSpan)
        {
            var snap = _elisionBuffer.SourceBuffer.CurrentSnapshot;
            changedSpan = changedSpan.TranslateTo(snap, SpanTrackingMode.EdgeInclusive);
            if (changedSpan.Length == 0)
                return;
            var list = _hiddenRegionTagAggregator.GetTags(changedSpan)
                .SelectMany(mappingTag => mappingTag.Span.GetSpans(snap) as IEnumerable<SnapshotSpan>).ToList();
            var right = NormalizedSnapshotSpanCollection.Intersection(new NormalizedSnapshotSpanCollection(list),
                new NormalizedSnapshotSpanCollection(changedSpan));
            var snapshotSpanCollection1 =
                NormalizedSnapshotSpanCollection.Difference(new NormalizedSnapshotSpanCollection(changedSpan), right);
            if (list.Count > 1)
            {
                list.Sort(SpanComparator);
                var snapshotSpanList = new List<SnapshotSpan>();
                int val2 = list[0].End;
                for (var index = 1; index < list.Count; ++index)
                {
                    var snapshotSpan = list[index];
                    if (snapshotSpan.Start == val2)
                        snapshotSpanList.Add(new SnapshotSpan(snapshotSpan.Start, 0));
                    val2 = Math.Max(snapshotSpan.End, val2);
                }

                snapshotSpanCollection1 =
                    NormalizedSnapshotSpanCollection.Union(new NormalizedSnapshotSpanCollection(snapshotSpanList),
                        snapshotSpanCollection1);
            }

            var snapshotSpanCollection2 = NormalizedSnapshotSpanCollection.Union(snapshotSpanCollection1,
                new NormalizedSnapshotSpanCollection(new[]
                {
                    new SnapshotSpan(snap, 0, 0),
                    new SnapshotSpan(snap, snap.Length, 0)
                }));
            _elisionBuffer.ModifySpans(right, snapshotSpanCollection2);
        }
    }
}