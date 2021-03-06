﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextSnapshot
    {
        IContentType ContentType { get; }

        int Length { get; }

        int LineCount { get; }

        IEnumerable<ITextSnapshotLine> Lines { get; }
        ITextBuffer TextBuffer { get; }

        ITextImage TextImage { get; }

        ITextVersion Version { get; }

        char this[int position] { get; }

        void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode);

        ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode);

        ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode,
            TrackingFidelityMode trackingFidelity);

        ITextSnapshotLine GetLineFromLineNumber(int lineNumber);

        ITextSnapshotLine GetLineFromPosition(int position);

        int GetLineNumberFromPosition(int position);

        string GetText(Span span);

        string GetText(int startIndex, int length);

        string GetText();

        void SaveToFile(string filePath, bool replaceFile, Encoding encoding);

        char[] ToCharArray(int startIndex, int length);

        void Write(TextWriter writer, Span span);

        void Write(TextWriter writer);
    }
}