using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class ViewState
    {
        public ITextSnapshot EditSnapshot { get; }

        public double ViewportBottom => ViewportTop + ViewportHeight;

        public double ViewportHeight { get; }
        public double ViewportLeft { get; }

        public double ViewportRight => ViewportLeft + ViewportWidth;

        public double ViewportTop { get; }

        public double ViewportWidth { get; }

        public ITextSnapshot VisualSnapshot { get; }

        public ViewState(ITextView view, double effectiveViewportWidth, double effectiveViewportHeight)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            ViewportLeft = view.ViewportLeft;
            ViewportTop = view.ViewportTop;
            ViewportWidth = effectiveViewportWidth;
            ViewportHeight = effectiveViewportHeight;
            VisualSnapshot = view.VisualSnapshot;
            EditSnapshot = view.TextSnapshot;
        }

        public ViewState(ITextView view)
            : this(view, view?.ViewportWidth ?? 0.0, view?.ViewportHeight ?? 0.0)
        {
        }
    }
}