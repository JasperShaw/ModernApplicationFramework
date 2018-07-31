﻿using System;

namespace ModernApplicationFramework.TextEditor
{
    public class CaretPositionChangedEventArgs : EventArgs
    {
        public CaretPositionChangedEventArgs(ITextView textView, CaretPosition oldPosition, CaretPosition newPosition)
        {
            TextView = textView;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public ITextView TextView { get; }

        public CaretPosition OldPosition { get; }

        public CaretPosition NewPosition { get; }
    }
}