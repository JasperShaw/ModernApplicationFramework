using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal abstract class ShiftClickScrollBarMargin : ScrollBar, ITextViewMargin
    {
        private readonly string _marginName;
        private readonly Orientation _orientation;
        private bool _inShiftLeftClick;
        private bool _isDisposed;

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> LeftShiftClick;

        public abstract bool Enabled { get; }

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

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        protected ShiftClickScrollBarMargin(Orientation orientation, string marginName)
        {
            SetResourceReference(StyleProperty, typeof(ScrollBar));
            _marginName = marginName;
            _orientation = orientation;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _isDisposed = true;
            OnDispose();
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (!_isDisposed && string.Compare(marginName, _marginName, StringComparison.OrdinalIgnoreCase) == 0)
                return this;
            return null;
        }

        public abstract void OnDispose();

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

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (!_inShiftLeftClick)
                return;
            _inShiftLeftClick = false;
            var leftShiftClick = LeftShiftClick;
            leftShiftClick?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, newValue));
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ShiftClickScrollBarMargin));
        }
    }
}