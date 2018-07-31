using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    internal class DragDropVisualManager
    {
        [Export]
        [Name("DragDropAdornmentLayer")]
        [Order(After = "Text")]
        private static AdornmentLayerDefinition _dragDropAdornmentLayer;
        internal Rectangle PointerAdornment;
        internal Brush PointerBrush;
        private bool _adornmentAdded;
        private readonly IClassificationFormatMapService _classificationFormatMapService;
        private IAdornmentLayer _adornmentLayer;
        private readonly ITextView _textView;

        private IAdornmentLayer AdormentLayer => _adornmentLayer ?? (_adornmentLayer = _textView.GetAdornmentLayer("DragDropAdornmentLayer"));

        public DragDropVisualManager(ITextView textView, IClassificationFormatMapService classificationFormatMapService)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _classificationFormatMapService = classificationFormatMapService ?? throw new ArgumentNullException(nameof(classificationFormatMapService));
            SetPointerBrush();
            var formatMap = classificationFormatMapService.GetClassificationFormatMap(_textView);
            formatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
            _textView.Closed += (EventHandler)((param1, param2) => formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged);
        }

        public virtual void EnableDragDropVisuals()
        {
            _textView.Caret.IsHidden = true;
        }

        public virtual void DisableDragDropVisuals()
        {
            _textView.Caret.IsHidden = false;
            ClearTracker();
        }

        public virtual void DrawTracker(TextBounds insertionBounds)
        {
            CreatePointerAdornment(insertionBounds);
            Canvas.SetLeft(PointerAdornment, insertionBounds.Left);
            Canvas.SetTop(PointerAdornment, insertionBounds.Top);
            if (_adornmentAdded)
                return;
            AdormentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, new SnapshotSpan?(), this, PointerAdornment, null);
            _adornmentAdded = true;
        }

        public virtual void ClearTracker()
        {
            if (!_adornmentAdded)
                return;
            AdormentLayer.RemoveAdornment(PointerAdornment);
            _adornmentAdded = false;
        }

        public void ScrollView(ITextViewLine containingLine, TextBounds insertionBounds)
        {
            var val21 = Math.Max(0.0, (_textView.ViewportHeight - containingLine.Height) * 0.5);
            if (containingLine.VisibilityState == VisibilityState.Unattached)
            {
                _textView.DisplayTextLineContainingBufferPosition(containingLine.Start, Math.Min(15.0, val21), containingLine.Start.Position < _textView.TextViewLines.FormattedSpan.Start ? ViewRelativePosition.Top : ViewRelativePosition.Bottom);
            }
            else
            {
                var num1 = containingLine.Top - (_textView.ViewportTop + 15.0);
                var num2 = _textView.ViewportBottom - 15.0 - containingLine.Bottom;
                if (num1 < 0.0 != num2 < 0.0)
                {
                    _textView.DisplayTextLineContainingBufferPosition(containingLine.Start, Math.Min(15.0, val21),
                        num1 < 0.0 ? ViewRelativePosition.Top : ViewRelativePosition.Bottom);
                }
            }
            var val22 = insertionBounds.Left - Math.Max(_textView.ViewportLeft + 15.0, 0.0);
            var val23 = _textView.ViewportRight - 15.0 - insertionBounds.Right;
            if (val22 < 0.0 == val23 < 0.0)
                return;
            if (val22 < 0.0)
                _textView.ViewportLeft = insertionBounds.Left - Math.Min(15.0, val23);
            else
                _textView.ViewportLeft = insertionBounds.Right + Math.Min(15.0, val22) - _textView.ViewportWidth;
        }

        private void CreatePointerAdornment(TextBounds bounds)
        {
            if (PointerAdornment == null)
            {
                PointerAdornment = new Rectangle
                {
                    IsHitTestVisible = false,
                    Width = SystemParameters.CaretWidth * 2.0,
                    Fill = PointerBrush
                };
            }
            if (PointerAdornment.Height == bounds.Height)
                return;
            PointerAdornment.Height = bounds.Height;
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            SetPointerBrush();
            if (PointerAdornment == null)
                return;
            PointerAdornment.Fill = PointerBrush;
        }

        private void SetPointerBrush()
        {
            var defaultTextProperties = _classificationFormatMapService.GetClassificationFormatMap(_textView).DefaultTextProperties;
            PointerBrush = defaultTextProperties.ForegroundBrushEmpty ? SystemColors.WindowTextBrush : defaultTextProperties.ForegroundBrush;
        }
    }
}