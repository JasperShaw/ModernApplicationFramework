using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextViewLine
    {
        double Baseline { get; }

        double Bottom { get; }

        TextViewLineChange Change { get; }

        LineTransform DefaultLineTransform { get; }

        double DeltaY { get; }

        SnapshotPoint End { get; }

        SnapshotPoint EndIncludingLineBreak { get; }

        double EndOfLineWidth { get; }

        SnapshotSpan Extent { get; }

        IMappingSpan ExtentAsMappingSpan { get; }

        SnapshotSpan ExtentIncludingLineBreak { get; }

        IMappingSpan ExtentIncludingLineBreakAsMappingSpan { get; }

        double Height { get; }

        object IdentityTag { get; }

        bool IsFirstTextViewLineForSnapshotLine { get; }

        bool IsLastTextViewLineForSnapshotLine { get; }

        bool IsValid { get; }

        double Left { get; }

        int Length { get; }

        int LengthIncludingLineBreak { get; }

        int LineBreakLength { get; }

        LineTransform LineTransform { get; }

        double Right { get; }

        ITextSnapshot Snapshot { get; }

        SnapshotPoint Start { get; }

        double TextBottom { get; }

        double TextHeight { get; }

        double TextLeft { get; }

        double TextRight { get; }

        double TextTop { get; }

        double TextWidth { get; }

        double Top { get; }

        double VirtualSpaceWidth { get; }

        VisibilityState VisibilityState { get; }

        double Width { get; }

        bool ContainsBufferPosition(SnapshotPoint bufferPosition);

        TextBounds? GetAdornmentBounds(object identityTag);

        ReadOnlyCollection<object> GetAdornmentTags(object providerTag);
        SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate, bool textOnly);

        SnapshotPoint? GetBufferPositionFromXCoordinate(double xCoordinate);

        TextBounds GetCharacterBounds(SnapshotPoint bufferPosition);

        TextBounds GetCharacterBounds(VirtualSnapshotPoint bufferPosition);

        TextBounds GetExtendedCharacterBounds(SnapshotPoint bufferPosition);

        TextBounds GetExtendedCharacterBounds(VirtualSnapshotPoint bufferPosition);

        VirtualSnapshotPoint GetInsertionBufferPositionFromXCoordinate(double xCoordinate);

        Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan);

        SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition);

        VirtualSnapshotPoint GetVirtualBufferPositionFromXCoordinate(double xCoordinate);

        bool IntersectsBufferSpan(SnapshotSpan bufferSpan);
    }
}