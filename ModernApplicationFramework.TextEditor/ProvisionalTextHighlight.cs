using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ProvisionalTextHighlight
    {
        private bool _useBlinkBrush = true;
        private readonly Brush _baseBrush;
        private readonly Brush _blinkBrush;
        private readonly DispatcherTimer _blinkTimer;
        private ITrackingSpan _provisionalSpan;
        private readonly ITextView _wpfTextView;
        private Path _highlightAdornment;
        private bool _isClosed;
        private readonly IAdornmentLayer _adornmentLayer;

        public ProvisionalTextHighlight(ITextView wpfTextView)
        {
            _wpfTextView = wpfTextView;
            var highlightColor = SystemColors.HighlightColor;
            var color1 = Color.FromArgb(96, highlightColor.R, highlightColor.G, highlightColor.B);
            var color2 = Color.FromArgb(180, (byte)(highlightColor.R / 3U), (byte)(highlightColor.G / 3U), (byte)(highlightColor.B / 3U));
            _baseBrush = new SolidColorBrush(color1);
            _blinkBrush = new SolidColorBrush(color2);
            _provisionalSpan = null;
            _highlightAdornment = null;
            _adornmentLayer = _wpfTextView.GetAdornmentLayer("SelectionAndProvisionHighlight");
            _wpfTextView.LayoutChanged += OnLayoutChanged;
            var caretBlinkTime = CaretBlinkTimeManager.GetCaretBlinkTime();
            if (caretBlinkTime <= 0)
                return;
            _blinkTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, caretBlinkTime), DispatcherPriority.Normal, OnTimerElapsed, _wpfTextView.VisualElement.Dispatcher);
        }

        public ITrackingSpan ProvisionalSpan
        {
            get => _provisionalSpan;
            set
            {
                _provisionalSpan = value;
                if (_provisionalSpan == null)
                {
                    ClearAdornment();
                }
                else
                {
                    if (_blinkTimer != null && !_blinkTimer.IsEnabled)
                    {
                        _blinkTimer.Start();
                        _useBlinkBrush = true;
                    }
                    Repaint();
                }
            }
        }

        private void ClearAdornment()
        {
            if (_highlightAdornment == null)
                return;
            _adornmentLayer.RemoveAdornment(_highlightAdornment);
            _highlightAdornment = null;
        }

        private void Repaint()
        {
            if (_provisionalSpan == null)
                return;
            ClearAdornment();
            var textMarkerGeometry = _wpfTextView.TextViewLines.GetTextMarkerGeometry(_provisionalSpan.GetSpan(_wpfTextView.TextSnapshot));
            if (textMarkerGeometry != null)
            {
                _highlightAdornment = new Path
                {
                    Data = textMarkerGeometry,
                    Fill = _useBlinkBrush ? _blinkBrush : _baseBrush
                };
            }
            if (_highlightAdornment == null)
                return;
            _adornmentLayer.AddAdornment(_provisionalSpan.GetSpan(_wpfTextView.TextSnapshot), this, _highlightAdornment);
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (_provisionalSpan == null)
                return;
            var span = _provisionalSpan.GetSpan(_wpfTextView.TextSnapshot);
            if (!e.NewOrReformattedSpans.OverlapsWith(span))
                return;
            Repaint();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (_highlightAdornment != null)
            {
                _highlightAdornment.Fill = _useBlinkBrush ? _blinkBrush : _baseBrush;
                _useBlinkBrush = !_useBlinkBrush;
            }
            else
                _blinkTimer.Stop();
        }

        public void Close()
        {
            if (_isClosed)
                return;
            _isClosed = true;
            ClearAdornment();
            _wpfTextView.LayoutChanged -= OnLayoutChanged;
            _blinkTimer?.Stop();
        }
    }
}