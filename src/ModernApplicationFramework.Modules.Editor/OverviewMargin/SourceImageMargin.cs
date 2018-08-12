using System;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class SourceImageMargin : ITextViewMargin
    {
        private readonly SourceImageMarginElement _sourceImageMarginElement;
        private bool _isDisposed;
        public const double MarginWidth = 100.0;

        public SourceImageMargin(ITextViewHost textViewHost, IVerticalScrollBar scrollBar, SourceImageMarginFactory factory)
        {
            if (textViewHost == null)
                throw new ArgumentNullException(nameof(textViewHost));
            _sourceImageMarginElement = new SourceImageMarginElement(textViewHost.TextView, factory, scrollBar);
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return _sourceImageMarginElement;
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

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return _sourceImageMarginElement.Enabled;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, "OverviewSourceImageMargin", StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _sourceImageMarginElement.Dispose();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("OverviewSourceImageMargin");
        }
    }
}