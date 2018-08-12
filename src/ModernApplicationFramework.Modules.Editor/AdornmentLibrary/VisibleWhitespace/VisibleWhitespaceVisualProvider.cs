using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.AdornmentLibrary.VisibleWhitespace
{
    internal class VisibleWhitespaceVisualProvider
    {
        internal Brush ForegroundBrush;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IEditorOptions _editorOptions;
        private readonly ITextView _view;
        private readonly IAdornmentLayer _visibleWhitespaceLayer;
        private double _left = double.MaxValue;
        private double _right = double.MinValue;
        private bool _visible;

        public bool Visible
        {
            get => _visible;
            set
            {
                if (value == _visible)
                    return;
                if (!value)
                {
                    _view.LayoutChanged -= OnLayoutChanged;
                    _visibleWhitespaceLayer.RemoveAllAdornments();
                    _visible = false;
                }
                else
                {
                    _view.LayoutChanged += OnLayoutChanged;
                    if (_view.TextViewLines != null)
                        CreateVisuals(_view.TextViewLines, _view.ViewportLeft, _view.ViewportRight);
                    _visible = true;
                }
            }
        }

        public VisibleWhitespaceVisualProvider(ITextView textView, IEditorOptions editorOptions,
            IEditorFormatMap editorFormatMap)
        {
            _view = textView ?? throw new ArgumentNullException(nameof(textView));
            _view.Closed += OnTextViewClosed;
            _visibleWhitespaceLayer = textView.GetAdornmentLayer("VisibleWhitespace");
            _editorOptions = editorOptions ?? throw new ArgumentNullException(nameof(editorOptions));
            _editorOptions.OptionChanged += OnOptionChanged;
            _editorFormatMap = editorFormatMap;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            UpdateForegroundBrush();
            Visible = _editorOptions.IsVisibleWhitespaceEnabled();
        }

        public void UpdateForegroundBrush()
        {
            var properties = _editorFormatMap.GetProperties("Visible Whitespace");
            ForegroundBrush = !properties.Contains("ForegroundColor")
                ? (!properties.Contains("Foreground")
                    ? SystemColors.ControlLightLightBrush.Clone()
                    : (Brush) properties["Foreground"])
                : new SolidColorBrush((Color) properties["ForegroundColor"]);
            if (ForegroundBrush.CanFreeze)
                ForegroundBrush.Freeze();
            _visibleWhitespaceLayer.RemoveAllAdornments();
            if (!_visible)
                return;
            CreateVisuals(_view.TextViewLines, _view.ViewportLeft, _view.ViewportRight);
        }

        internal static void ScanLine(ITextView view, ITextViewLine line, List<TextBounds> spaceBounds,
            List<TextBounds> tabBounds, List<TextBounds> ideographicSpaceBounds, double left, double right)
        {
            if (line.TextRight <= left)
                return;
            var positionFromXcoordinate = GetBufferPositionFromXCoordinate(line, left);
            var snapshotPoint = GetBufferPositionFromXCoordinate(line, right);
            if (snapshotPoint != line.End)
                snapshotPoint = line.GetTextElementSpan(snapshotPoint).End;
            var start1 = MappingPointSnapshot
                .MapUpToSnapshotNoTrack(view.VisualSnapshot, positionFromXcoordinate, PositionAffinity.Predecessor)
                .Value;
            var end = MappingPointSnapshot
                .MapUpToSnapshotNoTrack(view.VisualSnapshot, snapshotPoint, PositionAffinity.Successor).Value;
            foreach (var span in MappingSpanSnapshot
                .Create(view.VisualSnapshot, new SnapshotSpan(start1, end), SpanTrackingMode.EdgeInclusive,
                    view.BufferGraph).GetSpans(view.TextSnapshot))
                for (int start2 = span.Start; start2 < span.End; ++start2)
                {
                    var bufferPosition = new SnapshotPoint(view.TextSnapshot, start2);
                    var ch = bufferPosition.GetChar();
                    if (ch == ' ')
                    {
                        spaceBounds.Add(line.GetCharacterBounds(bufferPosition));
                    }
                    else if (ch == '\t')
                    {
                        tabBounds.Add(line.GetCharacterBounds(bufferPosition));
                    }
                    else if (ch >= ' ')
                    {
                        if (ch >= ' ' && ch <= '\x200D' || ch == ' ' || ch == ' ' || ch == ' ' || ch == '\x180E' ||
                            ch == '\x2060')
                            spaceBounds.Add(line.GetCharacterBounds(bufferPosition));
                        else if (ch == '　')
                            ideographicSpaceBounds.Add(line.GetCharacterBounds(bufferPosition));
                    }
                }
        }

        private static SnapshotPoint GetBufferPositionFromXCoordinate(ITextViewLine line, double x)
        {
            if (x <= line.TextLeft)
                return line.Start;
            if (x >= line.TextRight)
                return line.End;
            return line.GetBufferPositionFromXCoordinate(x).Value;
        }

        private void CreateEndOfFileMarker(ITextViewLine line)
        {
            var rectangle = new Rectangle {Height = line.TextHeight * 0.4};
            rectangle.Width = rectangle.Height * 0.6;
            rectangle.Stroke = ForegroundBrush;
            rectangle.StrokeThickness = line.TextHeight * 0.05;
            Canvas.SetTop(rectangle, line.TextTop + line.Baseline - rectangle.Height);
            Canvas.SetLeft(rectangle, line.TextRight);
            _visibleWhitespaceLayer.AddAdornment(new SnapshotSpan(_view.TextSnapshot, _view.TextSnapshot.Length, 0),
                null, rectangle);
        }

        private void CreateLineVisuals(ITextViewLine line)
        {
            var spaceBounds = new List<TextBounds>();
            var tabBounds = new List<TextBounds>();
            var ideographicSpaceBounds = new List<TextBounds>();
            ScanLine(_view, line, spaceBounds, tabBounds, ideographicSpaceBounds, _left, _right);
            if (tabBounds.Count <= 0 && spaceBounds.Count <= 0 && ideographicSpaceBounds.Count <= 0)
                return;
            var whitespaceAdornment = new VisibleWhitespaceAdornment(_view, spaceBounds, tabBounds,
                ideographicSpaceBounds, ForegroundBrush);
            _visibleWhitespaceLayer.AddAdornment(line.Extent, null, whitespaceAdornment);
        }

        private void CreateVisuals(IEnumerable<ITextViewLine> lines, double left, double right)
        {
            if (left < _left || right > _right)
            {
                _left = left - 200.0;
                _right = right + 200.0;
                _visibleWhitespaceLayer.RemoveAllAdornments();
                lines = _view.TextViewLines;
            }

            if (lines == null || !lines.Any())
                return;
            foreach (var line in lines)
                CreateLineVisuals(line);
            var bufferPosition = new SnapshotPoint(_view.TextSnapshot, _view.TextSnapshot.Length);
            var line1 = lines.Last();
            if (!line1.ContainsBufferPosition(bufferPosition))
                return;
            CreateEndOfFileMarker(line1);
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("Visible Whitespace"))
                return;
            UpdateForegroundBrush();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!_visible)
                return;
            CreateVisuals(e.NewOrReformattedLines, e.NewViewState.ViewportLeft, e.NewViewState.ViewportRight);
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultTextViewOptions.UseVisibleWhitespaceId.Name)
                return;
            Visible = _editorOptions.IsVisibleWhitespaceEnabled();
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _view.LayoutChanged -= OnLayoutChanged;
            _view.Closed -= OnTextViewClosed;
            _editorOptions.OptionChanged -= OnOptionChanged;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
        }
    }
}