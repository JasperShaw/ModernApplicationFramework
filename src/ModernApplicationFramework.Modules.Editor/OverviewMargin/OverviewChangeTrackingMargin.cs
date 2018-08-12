using System;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class OverviewChangeTrackingMargin : ITextViewMargin
    {
        internal ChangeTrackingMarginElement ChangeTrackingMarginElement;
        internal bool IsDisposed;
        public const double MarginWidth = 5.0;

        private OverviewChangeTrackingMargin(ITextViewHost textViewHost, IVerticalScrollBar scrollBar, OverviewChangeTrackingMarginProvider provider)
        {
            ChangeTrackingMarginElement = new ChangeTrackingMarginElement(textViewHost.TextView, scrollBar, provider);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(OverviewChangeTrackingMargin));
        }

        public static OverviewChangeTrackingMargin Create(ITextViewHost textViewHost, IVerticalScrollBar scrollBar, OverviewChangeTrackingMarginProvider provider)
        {
            if (textViewHost == null)
                throw new ArgumentNullException(nameof(textViewHost));
            return new OverviewChangeTrackingMargin(textViewHost, scrollBar, provider);
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return ChangeTrackingMarginElement;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ChangeTrackingMarginElement.ActualWidth;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return ChangeTrackingMarginElement.Enabled;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, nameof(OverviewChangeTrackingMargin), StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            ChangeTrackingMarginElement.Dispose();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }
}