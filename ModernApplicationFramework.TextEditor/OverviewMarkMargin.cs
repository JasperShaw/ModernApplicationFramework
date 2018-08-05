using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class OverviewMarkMargin : ITextViewMargin, IOverviewMarkMarginTest
    {
        internal BaseMarginElement BaseMarginElement;
        private bool _isDisposed;
        public const double MarginWidth = 6.0;

        private OverviewMarkMargin(BaseMarginElement baseMarginElement)
        {
            BaseMarginElement = baseMarginElement;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(OverviewMarkMargin));
        }

        public static OverviewMarkMargin Create(BaseMarginElement baseMarginElement)
        {
            if (baseMarginElement == null)
                throw new ArgumentNullException(nameof(baseMarginElement));
            return new OverviewMarkMargin(baseMarginElement);
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return BaseMarginElement;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return BaseMarginElement.ActualWidth;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return BaseMarginElement.Enabled;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, BaseMarginElement.MarginName, StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            BaseMarginElement.Dispose();
            _isDisposed = true;
        }

        public IList<Tuple<string, NormalizedSnapshotSpanCollection, int>> GetMarks()
        {
            return BaseMarginElement.GetMarks();
        }
    }
}