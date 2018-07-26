using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextSnapshot
    {
        ITextBuffer TextBuffer { get; }

        IContentType ContentType { get; }

        ITextVersion Version { get; }

        int Length { get; }

        int LineCount { get; }

        string GetText(Span span);

        string GetText(int startIndex, int length);

        string GetText();

        char[] ToCharArray(int startIndex, int length);

        void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

        char this[int position] { get; }

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity);

        ITextSnapshotLine GetLineFromLineNumber(int lineNumber);

        ITextSnapshotLine GetLineFromPosition(int position);

        int GetLineNumberFromPosition(int position);

        IEnumerable<ITextSnapshotLine> Lines { get; }

        void Write(TextWriter writer, Span span);

        void Write(TextWriter writer);

        ITextImage TextImage { get; }

        void SaveToFile(string filePath, bool replaceFile, Encoding encoding);
    }
}
