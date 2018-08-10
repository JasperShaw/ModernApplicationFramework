using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewLineCollection : IList<ITextViewLine>
    {
        ITextViewLine FirstVisibleLine { get; }

        SnapshotSpan FormattedSpan { get; }

        bool IsValid { get; }

        ITextViewLine LastVisibleLine { get; }
        ReadOnlyCollection<ITextViewLine> TextViewLines { get; }

        new ITextViewLine this[int index] { get; }

        bool ContainsBufferPosition(SnapshotPoint bufferPosition);

        TextBounds GetCharacterBounds(SnapshotPoint bufferPosition);

        int GetIndexOfTextLine(ITextViewLine textLine);

        Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan);

        Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        Geometry GetMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        Geometry GetMarkerGeometry(SnapshotSpan bufferSpan);

        Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan);

        SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition);

        Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan);

        Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding);

        ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition);

        ITextViewLine GetTextViewLineContainingYCoordinate(double y);

        Collection<ITextViewLine> GetTextViewLinesIntersectingSpan(SnapshotSpan bufferSpan);

        bool IntersectsBufferSpan(SnapshotSpan bufferSpan);
    }
}