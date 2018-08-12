using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class VsTextMarkerTag : IVsVisibleTextMarkerTag
    {
        //public VsTextMarkerTag(VsTextMarkerAdapter textMarkerAdapter, VirtualSnapshotSpan virtualSpan)
        //{
        //    TextMarker = textMarkerAdapter;
        //    MarkerSpan = EffectiveSpan(virtualSpan);
        //}

        public TextMarkerAdapter TextMarker { get; }

        public ITextBuffer TagTextBuffer => MarkerSpan.Snapshot.TextBuffer;

        public VirtualSnapshotSpan MarkerSpan { get; private set; }

        //public void UpdateMarkerSpan(VirtualSnapshotSpan newMarkerSpan)
        //{
        //    MarkerSpan = EffectiveSpan(newMarkerSpan);
        //}

        public void UpdateMarkerSpan()
        {
            MarkerSpan = EffectiveSpan(MarkerSpan);
        }

        public void SetSnapshot(ITextSnapshot newSnapshot, bool keepMarkers)
        {
            if (MarkerSpan.Snapshot.Version.VersionNumber >= newSnapshot.Version.VersionNumber || TextMarker.IsDisposed)
                return;
            if (MarkerSpan.Snapshot.Version.Changes.Count == 0)
            {
                MarkerSpan = MarkerSpan.TranslateTo(newSnapshot);
            }
            else
            {
                if (((int)TextMarker.Behavior & 256) != 0)
                    keepMarkers = false;
                var markerSpan = MarkerSpan;
                var trackingMode = AdjustTrackingMode(markerSpan, TextMarker.SpanTrackingMode);
                var newEffectiveVirtualSpan = EffectiveSpan(MarkerSpan.TranslateTo(newSnapshot, trackingMode));
                if (MarkerRemovedByChange(keepMarkers, markerSpan, newEffectiveVirtualSpan))
                    return;
                MarkerSpan = newEffectiveVirtualSpan;
                if (TextMarker.NotifyClientOnChanges && markerSpan.Snapshot.Version.Changes[0].OldPosition <= markerSpan.End.Position)
                    TextMarker.OnAfterMarkerChange();
                if (!TextMarker.NotifyClientAdvancedOnChanges)
                    return;
                var span = Span.FromBounds(markerSpan.Start.Position.GetContainingLine().Start, markerSpan.End.Position.GetContainingLine().End);
                foreach (var change in markerSpan.Snapshot.Version.Changes)
                {
                    if (span.IntersectsWith(new Span(change.OldPosition, change.OldLength)))
                    {
                        TextMarker.OnMarkerTextChanged();
                        break;
                    }
                }
            }
        }

        private static SpanTrackingMode AdjustTrackingMode(VirtualSnapshotSpan oldSpan, SpanTrackingMode trackingMode)
        {
            if (oldSpan.Length == 0)
            {
                if (trackingMode != SpanTrackingMode.EdgeInclusive && trackingMode != SpanTrackingMode.EdgePositive)
                {
                    foreach (var change in oldSpan.Snapshot.Version.Changes)
                    {
                        if (oldSpan.Start.Position <= change.OldEnd)
                        {
                            if (change.OldPosition < oldSpan.Start.Position)
                            {
                                trackingMode = SpanTrackingMode.EdgePositive;
                            }
                            break;
                        }
                    }
                }
            }
            else if (trackingMode != SpanTrackingMode.EdgeInclusive)
            {
                foreach (var change in oldSpan.Snapshot.Version.Changes)
                {
                    if (oldSpan.End.Position <= change.OldEnd)
                    {
                        if (oldSpan.End.Position == change.OldEnd && (oldSpan.Start.Position == change.OldPosition && change.NewLength > 0 && change.LineCountDelta == 0))
                        {
                            trackingMode = SpanTrackingMode.EdgeInclusive;
                            break;
                        }
                        if (trackingMode == SpanTrackingMode.EdgeNegative || trackingMode == SpanTrackingMode.EdgeExclusive)
                        {
                            var oldPosition1 = change.OldPosition;
                            var virtualSnapshotPoint = oldSpan.Start;
                            int position1 = virtualSnapshotPoint.Position;
                            if (oldPosition1 > position1)
                            {
                                var oldPosition2 = change.OldPosition;
                                virtualSnapshotPoint = oldSpan.End;
                                int position2 = virtualSnapshotPoint.Position;
                                if (oldPosition2 < position2)
                                {
                                    trackingMode = trackingMode == SpanTrackingMode.EdgeNegative ? SpanTrackingMode.EdgeInclusive : SpanTrackingMode.EdgePositive;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return trackingMode;
        }

        public bool HasGlyph => (TextMarker.VisualStyle & 1U) > 0U;

        //public bool HasGlyphHoverCursor => (TextMarker.VisualStyle & 512U) > 0U;

        //public bool HasDraggableGlyph
        //{
        //    get
        //    {
        //        if (HasGlyph)
        //            return (TextMarker.VisualStyle & 1024U) > 0U;
        //        return false;
        //    }
        //}

        private bool MarkerRemovedByChange(bool keepMarkers, VirtualSnapshotSpan oldVirtualSpan, VirtualSnapshotSpan newEffectiveVirtualSpan)
        {
            var flag1 = false;
            var flag2 = false;
            foreach (var change in MarkerSpan.Snapshot.Version.Changes)
            {
                if (change.OldLength > 0 && change.OldPosition <= MarkerSpan.Start.Position && change.OldEnd >= MarkerSpan.End.Position)
                {
                    flag1 = true;
                    var oldPosition = change.OldPosition;
                    var virtualSnapshotPoint = MarkerSpan.Start;
                    int position1 = virtualSnapshotPoint.Position;
                    int num;
                    if (oldPosition == position1)
                    {
                        var oldEnd = change.OldEnd;
                        virtualSnapshotPoint = MarkerSpan.End;
                        int position2 = virtualSnapshotPoint.Position;
                        num = oldEnd == position2 ? 1 : 0;
                    }
                    else
                        num = 0;
                    flag2 = num != 0;
                    break;
                }
            }
            if (flag1 && !keepMarkers)
            {
                if (oldVirtualSpan.Length == 0)
                {
                    if (TextMarker.IsVisible)
                    {
                        TextMarker.OnMarkerSpanDeleted();
                        return true;
                    }
                }
                else if (!flag2 || newEffectiveVirtualSpan.Length == 0 && ((int)TextMarker.Behavior & 16) == 0)
                {
                    TextMarker.OnMarkerSpanDeleted();
                    return true;
                }
            }
            return false;
        }

        private VirtualSnapshotSpan EffectiveSpan(VirtualSnapshotSpan span)
        {
            if (((int)TextMarker.Behavior & 8) != 0)
            {
                var snapshot1 = span.Snapshot;
                var virtualSnapshotPoint = span.Start;
                int position1 = virtualSnapshotPoint.Position;
                int start = snapshot1.GetLineFromPosition(position1).Start;
                var snapshot2 = span.Snapshot;
                virtualSnapshotPoint = span.End;
                int position2 = virtualSnapshotPoint.Position;
                int end = snapshot2.GetLineFromPosition(position2).End;
                return new VirtualSnapshotSpan(new SnapshotSpan(span.Snapshot, Span.FromBounds(start, end)));
            }
            if (((int)TextMarker.Behavior & 1) != 0)
                return new VirtualSnapshotSpan(span.Snapshot.GetLineFromPosition(span.End.Position).Extent);
            return span;
        }

        int IVsVisibleTextMarkerTag.Type => TextMarker.Type;

        IVsTextMarkerType IVsVisibleTextMarkerTag.MarkerType => TextMarker.MarkerType.VsMarkerType;

        IVsTextLineMarker IVsVisibleTextMarkerTag.LineMarker => TextMarker;

        IVsTextStreamMarker IVsVisibleTextMarkerTag.StreamMarker => TextMarker;
    }

    public interface IVsVisibleTextMarkerTag : ITag
    {
        int Type { get; }

        IVsTextMarkerType MarkerType { get; }

        IVsTextLineMarker LineMarker { get; }

        IVsTextStreamMarker StreamMarker { get; }
    }

    public interface IVsTextStreamMarker
    {
    }

    public interface IVsTextLineMarker
    {
    }
}