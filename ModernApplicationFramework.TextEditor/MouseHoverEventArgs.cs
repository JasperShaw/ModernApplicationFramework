using System;

namespace ModernApplicationFramework.TextEditor
{
    public class MouseHoverEventArgs : EventArgs
    {
        public MouseHoverEventArgs(ITextView view, int position, IMappingPoint textPosition)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (position < 0 || position > view.TextSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            View = view;
            Position = position;
            TextPosition = textPosition ?? throw new ArgumentNullException(nameof(textPosition));
        }

        public ITextView View { get; }

        public int Position { get; }

        public IMappingPoint TextPosition { get; }
    }
}