using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextViewLine
    {
        SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate, bool textOnly);

        SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate);

        VirtualSnapshotPoint GetVirtualBufferPositionFromXCoordinate(double xCoordinate);

        VirtualSnapshotPoint GetInsertionBufferPositionFromXCoordinate(double xCoordinate);

        bool ContainsBufferPosition(SnapshotPoint bufferPosition);

        SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition);

        TextBounds GetCharacterBounds(SnapshotPoint bufferPosition);

        TextBounds GetCharacterBounds(VirtualSnapshotPoint bufferPosition);

        TextBounds GetExtendedCharacterBounds(SnapshotPoint bufferPosition);

        TextBounds GetExtendedCharacterBounds(VirtualSnapshotPoint bufferPosition);

        TextBounds? GetAdornmentBounds(object identityTag);

        Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan);

        object IdentityTag { get; }

        bool IntersectsBufferSpan(SnapshotSpan bufferSpan);

        ReadOnlyCollection<object> GetAdornmentTags(object providerTag);

        ITextSnapshot Snapshot { get; }

        bool IsFirstTextViewLineForSnapshotLine { get; }

        bool IsLastTextViewLineForSnapshotLine { get; }

        double Baseline { get; }

        SnapshotSpan Extent { get; }

        IMappingSpan ExtentAsMappingSpan { get; }

        SnapshotSpan ExtentIncludingLineBreak { get; }

        IMappingSpan ExtentIncludingLineBreakAsMappingSpan { get; }

        SnapshotPoint Start { get; }

        int Length { get; }

        int LengthIncludingLineBreak { get; }

        SnapshotPoint End { get; }

        SnapshotPoint EndIncludingLineBreak { get; }

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

        LineTransform LineTransform { get; }

        LineTransform DefaultLineTransform { get; }

        VisibilityState VisibilityState { get; }

        double DeltaY { get; }

        TextViewLineChange Change { get; }
    }
}