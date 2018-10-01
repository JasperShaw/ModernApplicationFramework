using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal class BraceCompletionAdornmentService : IBraceCompletionAdornmentService
    {
        private ITrackingPoint _trackingPoint;
        private IAdornmentLayer _adornmentLayer;
        private IEditorFormatMap _editorFormatMap;
        private ITextView _textView;
        private Brush _brush;

        private SnapshotSpan? TranslatedSpan
        {
            get
            {
                var nullable1 = new SnapshotSpan?();
                var textSnapshot = _textView.TextSnapshot;
                SnapshotPoint? nullable2 = _trackingPoint.GetPoint(_trackingPoint.TextBuffer.CurrentSnapshot);
                if (nullable2.Value.Snapshot != textSnapshot)
                    nullable2 = MappingPointSnapshot.MapUpToSnapshotNoTrack(textSnapshot, nullable2.Value, PositionAffinity.Predecessor);
                if (nullable2.HasValue && nullable2.Value.Position > 0)
                    nullable1 = new SnapshotSpan(nullable2.Value.Subtract(1), 1);
                return nullable1;
            }
        }

        public ITrackingPoint Point
        {
            get => _trackingPoint;
            set
            {
                if (_trackingPoint == value)
                    return;
                if (_trackingPoint != null)
                    _adornmentLayer.RemoveAllAdornments();
                _trackingPoint = value;
                if (_trackingPoint == null)
                    return;
                RenderAdornment();
            }
        }

        public BraceCompletionAdornmentService(ITextView textView, IEditorFormatMap editorFormatMap)
        {
            _editorFormatMap = editorFormatMap;
            _textView = textView;
            if (_textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (_editorFormatMap == null)
                throw new ArgumentNullException(nameof(editorFormatMap));
            _adornmentLayer = _textView.GetAdornmentLayer("BraceCompletion");
            SetBrush();
            RegisterEvents();
        }

        private void SetBrush()
        {
            var properties = _editorFormatMap.GetProperties("BraceCompletionClosingBrace");
            _brush = null;
            if (properties != null && properties.Contains("Background"))
            {
                var brush = properties["Background"] as Brush;
                if (brush == null || brush.Opacity <= 0.0)
                    return;
                _brush = brush;
                SetBrushAndRedrawAdornment();
            }
            else
                _brush = Brushes.LightBlue;
        }

        private void RegisterEvents()
        {
            _textView.Closed += TextView_Closed;
            _textView.LayoutChanged += TextView_LayoutChanged;
            _editorFormatMap.FormatMappingChanged += EditorFormatMap_FormatMappingChanged;
        }

        private void EditorFormatMap_FormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("BraceCompletionClosingBrace"))
                return;
            SetBrush();
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            UnregisterEvents();
        }

        private void UnregisterEvents()
        {
            _textView.Closed -= TextView_Closed;
            _textView.LayoutChanged -= TextView_LayoutChanged;
            _editorFormatMap.FormatMappingChanged -= EditorFormatMap_FormatMappingChanged;
        }

        private void TextView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (_trackingPoint == null || _brush == null || !_adornmentLayer.IsEmpty)
                return;
            RenderAdornment();
        }

        private void SetBrushAndRedrawAdornment()
        {
            if (_trackingPoint == null)
                return;
            _adornmentLayer.RemoveAllAdornments();
            RenderAdornment();
        }

        private void RenderAdornment()
        {
            if (_trackingPoint == null || _brush == null || (_textView.IsClosed || _textView.TextViewLines == null))
                return;
            var translatedSpan = TranslatedSpan;
            if (!translatedSpan.HasValue)
                return;
            var formattedSpan = _textView.TextViewLines.FormattedSpan;
            if (!formattedSpan.Contains(translatedSpan.Value.Start))
                return;
            var textViewLines = _textView.TextViewLines;
            formattedSpan = translatedSpan.Value;
            var start = formattedSpan.Start;
            var characterBounds = textViewLines.GetCharacterBounds(start);
            var rectangle = new Rectangle
            {
                Width = characterBounds.Width,
                Height = 2.0,
                Fill = _brush
            };
            Canvas.SetLeft(rectangle, characterBounds.Left);
            Canvas.SetTop(rectangle, characterBounds.TextBottom);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, translatedSpan, null, rectangle,
                null);
        }
    }
}
