using System;

namespace ModernApplicationFramework.TextEditor
{
    public class ViewState
    {
        public double ViewportLeft { get; }

        public double ViewportTop { get; }

        public double ViewportWidth { get; }

        public double ViewportHeight { get; }

        public double ViewportRight => ViewportLeft + ViewportWidth;

        public double ViewportBottom => ViewportTop + ViewportHeight;

        public ITextSnapshot VisualSnapshot { get; }

        public ITextSnapshot EditSnapshot { get; }

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