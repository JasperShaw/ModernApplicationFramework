using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class ReadOnlySpanCollection : ReadOnlyCollection<ReadOnlySpan>
    {
        private readonly List<IReadOnlyRegion> _regionsWithActions;

        internal IEnumerable<ReadOnlySpan> QueryAllEffectiveReadOnlySpans(ITextVersion version)
        {
            foreach (var readOnlySpan in this)
                yield return readOnlySpan;
            foreach (var regionsWithAction in _regionsWithActions)
            {
                if (regionsWithAction.QueryCallback(false))
                    yield return new ReadOnlySpan(version, regionsWithAction);
            }
        }

        internal ReadOnlySpanCollection(TextVersion version, IEnumerable<IReadOnlyRegion> regions)
            : base(NormalizeSpans(version, regions))
        {
            _regionsWithActions = regions.Where(region => region.QueryCallback != null).ToList();
        }

        internal bool IsReadOnly(int position, ITextSnapshot textSnapshot, bool notify)
        {
            foreach (var regionsWithAction in _regionsWithActions)
            {
                if (!IsEditAllowed(regionsWithAction, position, textSnapshot) && regionsWithAction.QueryCallback(notify))
                    return true;
            }
            for (var index = 0; index < Count; ++index)
            {
                if (!this[index].IsInsertAllowed(position, textSnapshot))
                    return true;
            }
            return false;
        }

        private static bool IsEditAllowed(IReadOnlyRegion region, int position, ITextSnapshot textSnapshot)
        {
            return new ReadOnlySpan(textSnapshot.Version, region).IsInsertAllowed(position, textSnapshot);
        }

        private static bool IsEditAllowed(IReadOnlyRegion region, Span span, ITextSnapshot textSnapshot)
        {
            return new ReadOnlySpan(textSnapshot.Version, region).IsReplaceAllowed(span, textSnapshot);
        }

        internal bool IsReadOnly(Span span, ITextSnapshot textSnapshot, bool notify)
        {
            foreach (var regionsWithAction in _regionsWithActions)
            {
                if (!IsEditAllowed(regionsWithAction, span, textSnapshot) && regionsWithAction.QueryCallback(notify))
                    return true;
            }
            for (var index = 0; index < Count; ++index)
            {
                if (!this[index].IsReplaceAllowed(span, textSnapshot))
                    return true;
            }
            return false;
        }

        private static IList<ReadOnlySpan> NormalizeSpans(TextVersion version, IEnumerable<IReadOnlyRegion> regions)
        {
            var readOnlyRegionList = new List<IReadOnlyRegion>(regions.Where(region => region.QueryCallback == null));
            if (readOnlyRegionList.Count == 0)
                return new FrugalList<ReadOnlySpan>();
            if (readOnlyRegionList.Count == 1)
                return new FrugalList<ReadOnlySpan>()
                {
                    new ReadOnlySpan(version, readOnlyRegionList[0])
                };
            readOnlyRegionList.Sort((s1, s2) => s1.Span.GetSpan(version).Start.CompareTo(s2.Span.GetSpan(version).Start));
            var readOnlySpanList = new List<ReadOnlySpan>(readOnlyRegionList.Count);
            var start1 = readOnlyRegionList[0].Span.GetSpan(version).Start;
            var num = readOnlyRegionList[0].Span.GetSpan(version).End;
            var startEdgeInsertionMode = readOnlyRegionList[0].EdgeInsertionMode;
            var endEdgeInsertionMode = readOnlyRegionList[0].EdgeInsertionMode;
            var trackingMode = readOnlyRegionList[0].Span.TrackingMode;
            for (var index = 1; index < readOnlyRegionList.Count; ++index)
            {
                var start2 = readOnlyRegionList[index].Span.GetSpan(version).Start;
                var end = readOnlyRegionList[index].Span.GetSpan(version).End;
                if (num < start2)
                {
                    readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start1, num - start1), trackingMode, startEdgeInsertionMode, endEdgeInsertionMode));
                    start1 = start2;
                    num = end;
                    startEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                    endEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                    trackingMode = readOnlyRegionList[index].Span.TrackingMode;
                }
                else
                {
                    if (start2 == start1)
                    {
                        if (readOnlyRegionList[index].EdgeInsertionMode == EdgeInsertionMode.Deny)
                            startEdgeInsertionMode = EdgeInsertionMode.Deny;
                        if (trackingMode != readOnlyRegionList[index].Span.TrackingMode)
                        {
                            if (num == end)
                                trackingMode = SpanTrackingMode.EdgeInclusive;
                            else if (num < end)
                            {
                                readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start1, num - start1), SpanTrackingMode.EdgeInclusive, startEdgeInsertionMode, EdgeInsertionMode.Deny));
                                start1 = num;
                                num = end;
                                startEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                                endEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                                trackingMode = readOnlyRegionList[index].Span.TrackingMode;
                            }
                            else
                            {
                                readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start2, end - start2), SpanTrackingMode.EdgeInclusive, startEdgeInsertionMode, EdgeInsertionMode.Deny));
                                start1 = end;
                            }
                        }
                    }
                    if (num < end)
                    {
                        if (num == start2 && endEdgeInsertionMode == EdgeInsertionMode.Allow && readOnlyRegionList[index].EdgeInsertionMode == EdgeInsertionMode.Allow || trackingMode != readOnlyRegionList[index].Span.TrackingMode)
                        {
                            readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start1, num - start1), trackingMode, startEdgeInsertionMode, endEdgeInsertionMode));
                            start1 = num;
                            num = end;
                            startEdgeInsertionMode = trackingMode == readOnlyRegionList[index].Span.TrackingMode ? EdgeInsertionMode.Allow : EdgeInsertionMode.Deny;
                            endEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                            trackingMode = readOnlyRegionList[index].Span.TrackingMode;
                        }
                        else
                        {
                            num = end;
                            endEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                        }
                    }
                    else if (num == end)
                    {
                        if (readOnlyRegionList[index].EdgeInsertionMode == EdgeInsertionMode.Deny)
                            endEdgeInsertionMode = EdgeInsertionMode.Deny;
                        if (trackingMode != readOnlyRegionList[index].Span.TrackingMode)
                        {
                            readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start1, num - start1), trackingMode, startEdgeInsertionMode, endEdgeInsertionMode));
                            start1 = end;
                            num = end;
                            startEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                            endEdgeInsertionMode = readOnlyRegionList[index].EdgeInsertionMode;
                            trackingMode = readOnlyRegionList[index].Span.TrackingMode;
                        }
                    }
                }
            }
            readOnlySpanList.Add(new ReadOnlySpan(version, new Span(start1, num - start1), trackingMode, startEdgeInsertionMode, endEdgeInsertionMode));
            return readOnlySpanList;
        }
    }
}