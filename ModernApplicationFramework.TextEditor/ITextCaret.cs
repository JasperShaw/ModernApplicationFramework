using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextCaret
    {
        void EnsureVisible();

        CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate);

        CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate, bool captureHorizontalPosition);

        CaretPosition MoveTo(ITextViewLine textLine);

        CaretPosition MoveTo(SnapshotPoint bufferPosition);

        CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity);

        CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity, bool captureHorizontalPosition);

        CaretPosition MoveToPreferredCoordinates();

        CaretPosition MoveToNextCaretPosition();

        CaretPosition MoveToPreviousCaretPosition();

        ITextViewLine ContainingTextViewLine { get; }

        double Left { get; }

        double Width { get; }

        double Right { get; }

        double Top { get; }

        double Height { get; }

        double Bottom { get; }

        CaretPosition Position { get; }

        bool OverwriteMode { get; }

        bool InVirtualSpace { get; }

        bool IsHidden { get; set; }

        event EventHandler<CaretPositionChangedEventArgs> PositionChanged;
    }
}