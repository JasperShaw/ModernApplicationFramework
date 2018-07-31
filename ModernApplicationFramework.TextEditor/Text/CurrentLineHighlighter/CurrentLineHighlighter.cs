using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Text.CurrentLineHighlighter
{
    internal sealed class CurrentLineHighlighter
    {
        private readonly Rectangle _rectBorder = new Rectangle();
        private double _rectWidth = double.MinValue;
        private double _rectHeight = double.MinValue;
        private double _rectLeft = double.MinValue;
        private double _rectTop = double.MinValue;
        private readonly ITextView _textView;
        private readonly IAdornmentLayer _currentLineHighlighterLayer;
        private readonly IEditorFormatMap _editorFormatMap;
        private bool _isHighlightCurrentLineEnabled;
        private Brush _foregroundBrush;
        private Brush _backgroundBrush;
        private bool _isHighlightAdded;
        private bool _isColorChanged;

        public CurrentLineHighlighter(ITextView textView, IEditorFormatMap editorFormatMap)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _currentLineHighlighterLayer = _textView.GetAdornmentLayer(nameof(CurrentLineHighlighter));
            if (_currentLineHighlighterLayer == null)
                throw new InvalidOperationException("Failed to get CurrentLineHighlighterLayer");
            _editorFormatMap = editorFormatMap ?? throw new ArgumentNullException(nameof(editorFormatMap));
            _rectBorder.StrokeThickness = 2.0;
            _rectBorder.RadiusX = 2.0;
            _isHighlightCurrentLineEnabled = _textView.Options.GetOptionValue(DefaultViewOptions.EnableHighlightCurrentLineId);
            if (_isHighlightCurrentLineEnabled)
            {
                SetColors();
                SubscribeToEvents();
            }
            _textView.Options.OptionChanged += HandleOptionsChanged;
        }

        private void SubscribeToEvents()
        {
            _textView.LayoutChanged += HandleLayoutChanged;
            _textView.Selection.SelectionChanged += HandleSelectionChanged;
            _editorFormatMap.FormatMappingChanged += HandleFormatMappingChanged;
        }

        private void UnsubscribeToEvents()
        {
            _textView.LayoutChanged -= HandleLayoutChanged;
            _textView.Selection.SelectionChanged -= HandleSelectionChanged;
            _editorFormatMap.FormatMappingChanged -= HandleFormatMappingChanged;
        }

        private void HandleOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultViewOptions.EnableHighlightCurrentLineId.Name)
                return;
            _isHighlightCurrentLineEnabled = _textView.Options.GetOptionValue(DefaultViewOptions.EnableHighlightCurrentLineId);
            if (_isHighlightCurrentLineEnabled)
            {
                SubscribeToEvents();
                SetColors();
                HighlightCurrentLine();
            }
            else
            {
                UnsubscribeToEvents();
                RemoveCurrentLineHighlight();
            }
        }

        private void HandleFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("CurrentLineActiveFormat"))
                return;
            SetColors();
            HighlightCurrentLine();
        }

        private void HandleSelectionChanged(object sender, EventArgs args)
        {
            HighlightCurrentLine();
        }

        private void HandleLayoutChanged(object sender, EventArgs args)
        {
            HighlightCurrentLine();
        }

        private void RemoveCurrentLineHighlight()
        {
            if (!_isHighlightAdded)
                return;
            _currentLineHighlighterLayer.RemoveAllAdornments();
            _isHighlightAdded = false;
        }

        private void HighlightCurrentLine()
        {
            ITextViewLine containingTextViewLine = _textView.Caret.ContainingTextViewLine;
            if (containingTextViewLine == null || !_textView.Selection.IsEmpty || (!_isHighlightCurrentLineEnabled || containingTextViewLine.VisibilityState == VisibilityState.Hidden) || (containingTextViewLine.VisibilityState == VisibilityState.Unattached || _textView.ViewportHeight == 0.0 || _textView.ViewportWidth < 2.0))
            {
                RemoveCurrentLineHighlight();
            }
            else
            {
                if (_isColorChanged)
                {
                    _rectBorder.Stroke = _foregroundBrush;
                    _rectBorder.Fill = _backgroundBrush;
                    _isColorChanged = false;
                }
                if (containingTextViewLine.TextHeight != _rectHeight)
                {
                    _rectHeight = containingTextViewLine.TextHeight;
                    _rectBorder.Height = _rectHeight + 1.0;
                }
                if (_textView.ViewportWidth != _rectWidth)
                {
                    _rectWidth = _textView.ViewportWidth;
                    _rectBorder.Width = _rectWidth;
                }
                if (_rectLeft != _textView.ViewportLeft)
                {
                    _rectLeft = _textView.ViewportLeft;
                    Canvas.SetLeft(_rectBorder, _rectLeft);
                }
                if (_rectTop != containingTextViewLine.TextTop)
                {
                    _rectTop = containingTextViewLine.TextTop;
                    Canvas.SetTop(_rectBorder, _rectTop);
                }
                if (_isHighlightAdded)
                    return;
                _isHighlightAdded = true;
                _currentLineHighlighterLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, new SnapshotSpan?(), null, _rectBorder, null);
            }
        }

        private ResourceDictionary GetResourceDictionaryFromFormatMap()
        {
            return _editorFormatMap.GetProperties("CurrentLineActiveFormat");
        }

        private void SetColors()
        {
            ResourceDictionary dictionaryFromFormatMap = GetResourceDictionaryFromFormatMap();
            _foregroundBrush = null;
            if (dictionaryFromFormatMap.Contains("Foreground"))
            {
                if (dictionaryFromFormatMap["Foreground"] is Brush brush && !WpfHelper.BrushesEqual(brush, _textView.Background))
                    _foregroundBrush = brush;
            }
            _backgroundBrush = null;
            if (dictionaryFromFormatMap.Contains("Background"))
            {
                if (dictionaryFromFormatMap["Background"] is Brush brush1 && !WpfHelper.BrushesEqual(brush1, _textView.Background) && !WpfHelper.BrushesEqual(brush1, Brushes.Transparent))
                {
                    Brush brush2 = brush1.Clone();
                    brush2.Opacity = 0.25;
                    if (brush2.CanFreeze)
                        brush2.Freeze();
                    _backgroundBrush = brush2;
                }
            }
            _isColorChanged = true;
        }
    }
}