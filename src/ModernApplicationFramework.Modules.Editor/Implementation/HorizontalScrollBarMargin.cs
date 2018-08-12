using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class HorizontalScrollBarMargin : ShiftClickScrollBarMargin
    {
        private readonly ITextView _textView;

        public override bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _textView.Options.IsHorizontalScrollBarEnabled();
            }
        }

        private bool IsWordWrapEnabled
        {
            get
            {
                ThrowIfDisposed();
                return (_textView.Options.WordWrapStyle() & WordWrapStyles.WordWrap) == WordWrapStyles.WordWrap;
            }
        }

        public HorizontalScrollBarMargin(ITextView textView)
            : base(Orientation.Horizontal, "HorizontalScrollBar")
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            Name = "EditorUIHorizontalScrollbar";
            Orientation = Orientation.Horizontal;
            SetValue(SmallChangeProperty, 12.0);
            SetValue(LargeChangeProperty, _textView.ViewportWidth);
            Minimum = 0.0;
            MinHeight = 15.0;
            VerticalAlignment = VerticalAlignment.Top;
            OnOptionsChanged(null, null);
            _textView.Options.OptionChanged += OnOptionsChanged;
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool) e.NewValue)
                {
                    if (_textView.IsClosed)
                        return;
                    _textView.LayoutChanged += EditorLayoutChanged;
                    _textView.MaxTextRightCoordinateChanged += EditorLayoutChanged;
                    Scroll += HorizontalScrollBarScrolled;
                    LeftShiftClick += OnLeftShiftClick;
                    EditorLayoutChanged(null, null);
                }
                else
                {
                    _textView.LayoutChanged -= EditorLayoutChanged;
                    _textView.MaxTextRightCoordinateChanged -= EditorLayoutChanged;
                    Scroll -= HorizontalScrollBarScrolled;
                    LeftShiftClick -= OnLeftShiftClick;
                }
            };
            SetResourceReference(StyleProperty, typeof(ScrollBar));
        }

        public override void OnDispose()
        {
            _textView.Options.OptionChanged -= OnOptionsChanged;
        }

        internal void EditorLayoutChanged(object sender, EventArgs e)
        {
            if (IsWordWrapEnabled)
            {
                Maximum = 0.0;
            }
            else
            {
                Maximum = Math.Max(_textView.MaxTextRightCoordinate + 200.0 - _textView.ViewportWidth,
                    _textView.ViewportLeft);
                SetValue(LargeChangeProperty, _textView.ViewportWidth);
                ViewportSize = _textView.ViewportWidth;
                Value = _textView.ViewportLeft;
            }
        }

        internal void HorizontalScrollBarScrolled(object sender, ScrollEventArgs e)
        {
            _textView.ViewportLeft = e.NewValue;
        }

        private void OnLeftShiftClick(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _textView.ViewportLeft = e.NewValue;
        }

        private void OnOptionsChanged(object sender, EventArgs e)
        {
            Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
            IsEnabled = !IsWordWrapEnabled;
        }
    }
}