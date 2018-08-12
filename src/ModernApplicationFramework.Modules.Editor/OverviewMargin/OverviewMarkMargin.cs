using System;
using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.OverviewMargin;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class OverviewMarkMargin : ITextViewMargin, IOverviewMarkMarginTest
    {
        public const double MarginWidth = 6.0;
        internal BaseMarginElement BaseMarginElement;
        private bool _isDisposed;

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return BaseMarginElement.Enabled;
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

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return BaseMarginElement;
            }
        }

        private OverviewMarkMargin(BaseMarginElement baseMarginElement)
        {
            BaseMarginElement = baseMarginElement;
        }

        public static OverviewMarkMargin Create(BaseMarginElement baseMarginElement)
        {
            if (baseMarginElement == null)
                throw new ArgumentNullException(nameof(baseMarginElement));
            return new OverviewMarkMargin(baseMarginElement);
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

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, BaseMarginElement.MarginName, StringComparison.OrdinalIgnoreCase) != 0)
                return null;
            return this;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(OverviewMarkMargin));
        }
    }
}