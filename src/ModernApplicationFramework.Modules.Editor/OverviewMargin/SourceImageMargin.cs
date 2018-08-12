using System;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class SourceImageMargin : ITextViewMargin
    {
        public const double MarginWidth = 100.0;
        private readonly SourceImageMarginElement _sourceImageMarginElement;
        private bool _isDisposed;

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _sourceImageMarginElement.Enabled;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return _sourceImageMarginElement.ActualWidth;
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return _sourceImageMarginElement;
            }
        }

        public SourceImageMargin(ITextViewHost textViewHost, IVerticalScrollBar scrollBar,
            SourceImageMarginFactory factory)
        {
            if (textViewHost == null)
                throw new ArgumentNullException(nameof(textViewHost));
            _sourceImageMarginElement = new SourceImageMarginElement(textViewHost.TextView, factory, scrollBar);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _sourceImageMarginElement.Dispose();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, "OverviewSourceImageMargin", StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("OverviewSourceImageMargin");
        }
    }
}