using System.Collections.ObjectModel;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewLine
    {
        SnapshotPoint Start { get; }

        SnapshotPoint End { get; }

        SnapshotPoint EndIncludingLineBreak { get; }

        Rect VisibleArea { get; }

        int LineBreakLength { get; }

        double Left { get; }

        double Top { get; }

        double Height { get; }

        double TextTop { get; }

        double TextBottom { get; }

        double TextHeight { get; }

        double TextLeft { get; }

        double TextRight { get; }

        double TextWidth { get; }

        double Width { get; }

        double Bottom { get; }

        double Right { get; }

        double EndOfLineWidth { get; }

        double VirtualSpaceWidth { get; }

        bool IsValid { get; }

        VisibilityState VisibilityState { get; }

        double DeltaY { get; }

        bool ContainsBufferPosition(SnapshotPoint bufferPosition);

        SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition);

        Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan);

        TextBounds GetCharacterBounds(SnapshotPoint bufferPosition);
    }
}