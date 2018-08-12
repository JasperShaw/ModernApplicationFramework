using System;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class OverviewChangeTrackingMargin : ITextViewMargin
    {
        public const double MarginWidth = 5.0;
        internal ChangeTrackingMarginElement ChangeTrackingMarginElement;
        internal bool IsDisposed;

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return ChangeTrackingMarginElement.Enabled;
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

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return ChangeTrackingMarginElement;
            }
        }

        private OverviewChangeTrackingMargin(ITextViewHost textViewHost, IVerticalScrollBar scrollBar,
            OverviewChangeTrackingMarginProvider provider)
        {
            ChangeTrackingMarginElement = new ChangeTrackingMarginElement(textViewHost.TextView, scrollBar, provider);
        }

        public static OverviewChangeTrackingMargin Create(ITextViewHost textViewHost, IVerticalScrollBar scrollBar,
            OverviewChangeTrackingMarginProvider provider)
        {
            if (textViewHost == null)
                throw new ArgumentNullException(nameof(textViewHost));
            return new OverviewChangeTrackingMargin(textViewHost, scrollBar, provider);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            ChangeTrackingMarginElement.Dispose();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, nameof(OverviewChangeTrackingMargin), StringComparison.OrdinalIgnoreCase) !=
                0)
                return null;
            return this;
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(OverviewChangeTrackingMargin));
        }
    }
}