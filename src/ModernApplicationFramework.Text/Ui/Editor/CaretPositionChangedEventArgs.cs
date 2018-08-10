using System;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class CaretPositionChangedEventArgs : EventArgs
    {
        public CaretPosition NewPosition { get; }

        public CaretPosition OldPosition { get; }

        public ITextView TextView { get; }

        public CaretPositionChangedEventArgs(ITextView textView, CaretPosition oldPosition, CaretPosition newPosition)
        {
            TextView = textView;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }
}