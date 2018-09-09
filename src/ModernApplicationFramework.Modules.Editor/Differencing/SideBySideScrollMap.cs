using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Logic.Differencing;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class SideBySideScrollMap : IScrollMap
    {
        internal readonly IDifferenceViewer Viewer;
        internal readonly IScrollMap RightMap;
        private List<int> _sideBySideEndAtDiff;
        private int _sideBySideEndTotalLength;

        internal ISnapshotDifference Current { get; set; }

        internal bool _useSideBySideMapping { get; private set; }

        public SideBySideScrollMap(IDifferenceViewer viewer, IScrollMap rightMap)
        {
            Viewer = viewer;
            RightMap = rightMap;
            viewer.DifferenceBuffer.SnapshotDifferenceChanged += OnSnapshotDifferenceChanged;
            rightMap.MappingChanged += OnRightMappingChanged;
            RecalculateMap();
        }

        public bool UseSideBySideMapping
        {
            get
            {
                if (_useSideBySideMapping)
                    return Current != null;
                return false;
            }
            set
            {
                if (_useSideBySideMapping == value)
                    return;
                _useSideBySideMapping = value;
                if (Current == null)
                    return;
                OnMappingChanged(this, EventArgs.Empty);
            }
        }

        private void OnSnapshotDifferenceChanged(object sender, SnapshotDifferenceChangeEventArgs e)
        {
            RecalculateMap();
            if (!UseSideBySideMapping)
                return;
            OnMappingChanged(this, EventArgs.Empty);
        }

        private void OnRightMappingChanged(object sender, EventArgs e)
        {
            if (UseSideBySideMapping)
                return;
            OnMappingChanged(this, EventArgs.Empty);
        }

        private void RecalculateMap()
        {
            Current = Viewer.DifferenceBuffer.CurrentSnapshotDifference;
            if (Current == null)
                return;
            _sideBySideEndAtDiff = new List<int>(Current.LineDifferences.Differences.Count);
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            foreach (var difference in Current.LineDifferences.Differences)
            {
                var span = difference.Left;
                var end1 = span.End;
                span = difference.Right;
                var end2 = span.End;
                var val1 = end1 - num1;
                var val2 = end2 - num2;
                num3 += Math.Max(val1, val2);
                _sideBySideEndAtDiff.Add(num3);
                num1 = end1;
                num2 = end2;
            }
            _sideBySideEndTotalLength = num3 + Current.RightBufferSnapshot.LineCount - num2;
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            var mappingChanged = MappingChanged;
            mappingChanged?.Invoke(sender, e);
        }

        public double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition)
        {
            if (!UseSideBySideMapping)
                return RightMap.GetCoordinateAtBufferPosition(bufferPosition);
            return 0.5 + GetSideBySideLineAtBufferPosition(bufferPosition);
        }

        public SnapshotPoint GetBufferPositionAtCoordinate(double coordinate)
        {
            if (!UseSideBySideMapping)
                return RightMap.GetBufferPositionAtCoordinate(coordinate);
            return GetPositionOfSideBySideLine((int)coordinate, true);
        }

        public double Start => 0.5;

        public double End
        {
            get
            {
                if (!UseSideBySideMapping)
                    return RightMap.End;
                return _sideBySideEndTotalLength - 0.5;
            }
        }

        public double ThumbSize
        {
            get
            {
                if (!UseSideBySideMapping)
                    return RightMap.ThumbSize;
                return TextView.ViewportHeight / TextView.LineHeight;
            }
        }

        public ITextView TextView => Viewer.RightView;

        public double GetFractionAtBufferPosition(SnapshotPoint bufferPosition)
        {
            if (!UseSideBySideMapping)
                return RightMap.GetFractionAtBufferPosition(bufferPosition);
            return GetSideBySideLineAtBufferPosition(bufferPosition) / (double)_sideBySideEndTotalLength;
        }

        public SnapshotPoint GetBufferPositionAtFraction(double fraction)
        {
            if (!UseSideBySideMapping)
                return RightMap.GetBufferPositionAtFraction(fraction);
            return GetPositionOfSideBySideLine((int)(fraction * _sideBySideEndTotalLength), true);
        }

        public event EventHandler MappingChanged;

        public bool AreElisionsExpanded => false;

        internal SnapshotPoint GetPositionOfSideBySideLine(int lineInSideBySide, bool rightBufferOnly)
        {
            lineInSideBySide = Math.Min(Math.Max(0, lineInSideBySide), _sideBySideEndTotalLength - 1);
            ListUtilities.BinarySearch(_sideBySideEndAtDiff, s => s - lineInSideBySide - 1, out var index);
            var textSnapshot = Viewer.RightView.TextSnapshot;
            ITextSnapshotLine lineFromLineNumber;
            if (index >= Current.LineDifferences.Differences.Count)
            {
                lineFromLineNumber = Current.RightBufferSnapshot.GetLineFromLineNumber(Current.RightBufferSnapshot.LineCount - (_sideBySideEndTotalLength - lineInSideBySide));
            }
            else
            {
                var num1 = index > 0 ? _sideBySideEndAtDiff[index - 1] : 0;
                var num2 = lineInSideBySide - num1;
                var difference = Current.LineDifferences.Differences[index];
                var span1 = difference.Left;
                var span2 = difference.Right;
                if (difference.Before != null)
                {
                    var span3 = difference.Before.Left;
                    var start1 = span3.Start;
                    span3 = difference.Left;
                    var end1 = span3.End;
                    span1 = Span.FromBounds(start1, end1);
                    span3 = difference.Before.Right;
                    var start2 = span3.Start;
                    span3 = difference.Right;
                    var end2 = span3.End;
                    span2 = Span.FromBounds(start2, end2);
                }
                if (num2 < span2.Length)
                    lineFromLineNumber = Current.RightBufferSnapshot.GetLineFromLineNumber(span2.Start + num2);
                else if (rightBufferOnly)
                {
                    lineFromLineNumber = Current.RightBufferSnapshot.GetLineFromLineNumber(Math.Max(0, span2.End - 1));
                }
                else
                {
                    lineFromLineNumber = Current.LeftBufferSnapshot.GetLineFromLineNumber(span1.Start + num2);
                    textSnapshot = Viewer.LeftView.TextSnapshot;
                }
            }
            return lineFromLineNumber.Start.TranslateTo(textSnapshot, PointTrackingMode.Positive);
        }

        public int GetSideBySideLineAtBufferPosition(SnapshotPoint bufferPosition)
        {
            bufferPosition = Current.TranslateToSnapshot(bufferPosition);
            var lineNumber = bufferPosition.GetContainingLine().LineNumber;
            var matchOrDifference = Current.FindMatchOrDifference(bufferPosition, out _, out _);
            if (matchOrDifference == 0)
                return lineNumber;
            var difference2 = Current.LineDifferences.Differences[matchOrDifference - 1];
            if (bufferPosition.Snapshot == Current.LeftBufferSnapshot)
                return _sideBySideEndAtDiff[matchOrDifference - 1] + (lineNumber - difference2.Left.End);
            return _sideBySideEndAtDiff[matchOrDifference - 1] + (lineNumber - difference2.Right.End);
        }
    }
}