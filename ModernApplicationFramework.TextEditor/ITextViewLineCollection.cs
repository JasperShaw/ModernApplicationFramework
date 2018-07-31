using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewLineCollection : IList<ITextViewLine>
    {
        ReadOnlyCollection<ITextViewLine> TextViewLines { get; }

        Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan);

        Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan);

        Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        Geometry GetMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        Geometry GetMarkerGeometry(SnapshotSpan bufferSpan);

        ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition);

        ITextViewLine this[int index] { get; }

        ITextViewLine FirstVisibleLine { get; }

        ITextViewLine LastVisibleLine { get; }

        bool ContainsBufferPosition(SnapshotPoint bufferPosition);

        bool IntersectsBufferSpan(SnapshotSpan bufferSpan);

        ITextViewLine GetTextViewLineContainingYCoordinate(double y);

        Collection<ITextViewLine> GetTextViewLinesIntersectingSpan(SnapshotSpan bufferSpan);

        SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition);

        TextBounds GetCharacterBounds(SnapshotPoint bufferPosition);

        Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan);

        int GetIndexOfTextLine(ITextViewLine textLine);

        SnapshotSpan FormattedSpan { get; }

        bool IsValid { get; }
    }
}