using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal abstract class ShiftClickScrollBarMargin : ScrollBar, ITextViewMargin
    {
        private bool _inShiftLeftClick;
        private readonly string _marginName;
        private readonly Orientation _orientation;
        private bool _isDisposed;

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> LeftShiftClick;

        protected ShiftClickScrollBarMargin(Orientation orientation, string marginName)
        {
            SetResourceReference(StyleProperty, typeof(ScrollBar));
            _marginName = marginName;
            _orientation = orientation;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (!_inShiftLeftClick)
                return;
            _inShiftLeftClick = false;
            var leftShiftClick = LeftShiftClick;
            leftShiftClick?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                _inShiftLeftClick = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                base.OnPreviewMouseLeftButtonDown(e);
            }
            finally
            {
                _inShiftLeftClick = false;
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                if (_orientation != Orientation.Vertical)
                    return ActualWidth;
                return ActualHeight;
            }
        }

        public abstract bool Enabled { get; }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (!_isDisposed && string.Compare(marginName, _marginName, StringComparison.OrdinalIgnoreCase) == 0)
                return this;
            return null;
        }

        public abstract void OnDispose();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _isDisposed = true;
            OnDispose();
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ShiftClickScrollBarMargin));
        }
    }
}