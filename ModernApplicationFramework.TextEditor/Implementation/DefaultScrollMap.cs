using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal sealed class DefaultScrollMap : IScrollMap
    {
        private bool _useWordWrap;
        public event EventHandler MappingChanged;
        private double _currentHeight = double.MinValue;
        private double _currentWidth = double.MinValue;
        private ITextSnapshot _currentSnapshot;

        public ITextView TextView { get; }

        public bool AreElisionsExpanded { get; }

        private ITextSnapshot MappingSnapshot => !AreElisionsExpanded ? TextView.VisualSnapshot : TextView.TextSnapshot;

        public double Start => 0.5;

        public double End
        {
            get
            {
                if (!_useWordWrap)
                    return MappingSnapshot.LineCount - 0.5;
                return MappingSnapshot.Length + 0.5;
            }
        }

        public double ThumbSize
        {
            get
            {
                var num = TextView.ViewportHeight / TextView.LineHeight;
                if (_useWordWrap)
                    return num * (TextView.ViewportWidth / 20.0);
                return num;
            }
        }

        private bool WordWrapOnInOptions =>
            (TextView.Options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId) & WordWrapStyles.WordWrap) ==
            WordWrapStyles.WordWrap;

        internal DefaultScrollMap(ITextView textView, bool areElisionsExpanded)
        {
            TextView = textView;
            AreElisionsExpanded = areElisionsExpanded;
            _useWordWrap = WordWrapOnInOptions;
            TextView.Options.OptionChanged += OnOptionChanged;
            TextView.LayoutChanged += OnViewportChanged;
        }

        public double GetFractionAtBufferPosition(SnapshotPoint bufferPosition)
        {
            bufferPosition = TranslatePosition(bufferPosition);
            return !_useWordWrap
                ? bufferPosition.GetContainingLine().LineNumber / (double) bufferPosition.Snapshot.LineCount
                : bufferPosition.Position / (double) (bufferPosition.Snapshot.Length + 1);
        }

        public SnapshotPoint GetBufferPositionAtFraction(double fraction)
        {
            if (double.IsNaN(fraction) || fraction < 0.0 || fraction > 1.0)
                throw new ArgumentOutOfRangeException(nameof(fraction));
            var mappingSnapshot = MappingSnapshot;
            SnapshotPoint position;
            if (_useWordWrap)
            {
                var val1 = (int)(fraction * (mappingSnapshot.Length + 1));
                position = new SnapshotPoint(mappingSnapshot, Math.Min(val1, mappingSnapshot.Length));
            }
            else
            {
                var val1 = (int)(fraction * mappingSnapshot.LineCount);
                position = mappingSnapshot.GetLineFromLineNumber(Math.Min(val1, mappingSnapshot.LineCount - 1)).Start;
            }
            if (position.Snapshot != TextView.TextSnapshot)
                return TextView.BufferGraph.MapDownToBuffer(position, PointTrackingMode.Positive, TextView.TextBuffer, PositionAffinity.Successor).Value.TranslateTo(TextView.TextSnapshot, PointTrackingMode.Positive);
            return position;
        }

        
        public double GetCoordinateAtBufferPosition(SnapshotPoint bufferPosition)
        {
            bufferPosition = TranslatePosition(bufferPosition);
            return (!_useWordWrap ? bufferPosition.GetContainingLine().LineNumber : bufferPosition.Position) + 0.5;
        }

        
        public SnapshotPoint GetBufferPositionAtCoordinate(double coordinate)
        {
            if (double.IsNaN(coordinate))
                throw new ArgumentOutOfRangeException(nameof(coordinate));
            var val1 = Math.Max((int)coordinate, 0);
            var mappingSnapshot = MappingSnapshot;
            SnapshotPoint position1;
            if (_useWordWrap)
            {
                var position2 = Math.Min(val1, mappingSnapshot.Length);
                position1 = new SnapshotPoint(mappingSnapshot, position2);
            }
            else
            {
                var lineNumber = Math.Min(val1, mappingSnapshot.LineCount - 1);
                position1 = mappingSnapshot.GetLineFromLineNumber(lineNumber).Start;
            }
            if (position1.Snapshot != TextView.TextSnapshot)
                return TextView.BufferGraph.MapDownToBuffer(position1, PointTrackingMode.Positive, TextView.TextBuffer, PositionAffinity.Successor).Value.TranslateTo(TextView.TextSnapshot, PointTrackingMode.Positive);
            return position1;
        }

        private SnapshotPoint TranslatePosition(SnapshotPoint bufferPosition)
        {
            if (bufferPosition.Snapshot != TextView.TextSnapshot)
                throw new ArgumentException();
            if (AreElisionsExpanded)
                return bufferPosition;
            return TextView.TextViewModel.GetNearestPointInVisualBuffer(bufferPosition);
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultTextViewOptions.WordWrapStyleId.Name)
                return;
            var wordWrapOnInOptions = WordWrapOnInOptions;
            if (wordWrapOnInOptions == _useWordWrap)
                return;
            _useWordWrap = wordWrapOnInOptions;
            MappingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            var currentWidth = _currentWidth;
            var currentHeight1 = _currentHeight;
            _currentHeight = TextView.ViewportHeight;
            _currentWidth = TextView.ViewportWidth;
            var currentHeight2 = _currentHeight;
            var flag = currentHeight1 != currentHeight2 || _useWordWrap && currentWidth != _currentWidth;
            var currentSnapshot = _currentSnapshot;
            _currentSnapshot = MappingSnapshot;
            var mappingChanged = MappingChanged;
            if (mappingChanged == null)
                return;
            if (currentSnapshot != _currentSnapshot && !flag)
            {
                if (currentSnapshot == null)
                    flag = true;
                else
                {
                    for (var textVersion = currentSnapshot.Version;
                        textVersion != _currentSnapshot.Version;
                        textVersion = textVersion.Next)
                    {
                        if (textVersion.Changes.Count != 0 && (_useWordWrap || textVersion.Changes.IncludesLineChanges))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            if (!flag)
                return;
            mappingChanged(this, EventArgs.Empty);
        }
    }
}