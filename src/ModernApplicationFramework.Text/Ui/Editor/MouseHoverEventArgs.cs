﻿using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class MouseHoverEventArgs : EventArgs
    {
        public int Position { get; }

        public IMappingPoint TextPosition { get; }

        public ITextView View { get; }

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
    }
}