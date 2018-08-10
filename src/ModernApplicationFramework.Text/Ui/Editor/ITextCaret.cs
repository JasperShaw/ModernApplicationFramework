using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextCaret
    {
        event EventHandler<CaretPositionChangedEventArgs> PositionChanged;

        double Bottom { get; }

        ITextViewLine ContainingTextViewLine { get; }

        double Height { get; }

        bool InVirtualSpace { get; }

        bool IsHidden { get; set; }

        double Left { get; }

        bool OverwriteMode { get; }

        CaretPosition Position { get; }

        double Right { get; }

        double Top { get; }

        double Width { get; }
        void EnsureVisible();

        CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate);

        CaretPosition MoveTo(ITextViewLine textLine, double xCoordinate, bool captureHorizontalPosition);

        CaretPosition MoveTo(ITextViewLine textLine);

        CaretPosition MoveTo(SnapshotPoint bufferPosition);

        CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity);

        CaretPosition MoveTo(SnapshotPoint bufferPosition, PositionAffinity caretAffinity,
            bool captureHorizontalPosition);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity);

        CaretPosition MoveTo(VirtualSnapshotPoint bufferPosition, PositionAffinity caretAffinity,
            bool captureHorizontalPosition);

        CaretPosition MoveToNextCaretPosition();

        CaretPosition MoveToPreferredCoordinates();

        CaretPosition MoveToPreviousCaretPosition();
    }
}