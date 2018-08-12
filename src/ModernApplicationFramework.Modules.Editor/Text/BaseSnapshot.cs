using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal abstract class BaseSnapshot : ITextSnapshot
    {
        internal readonly StringRebuilder Content;
        internal readonly ITextImage CachingContent;

        public ITextBuffer TextBuffer => TextBufferHelper;
        public IContentType ContentType { get; }
        public ITextVersion Version { get; }

        public int Length => CachingContent.Length;

        public int LineCount => CachingContent.LineCount;

        public ITextImage TextImage => CachingContent;

        protected abstract ITextBuffer TextBufferHelper { get; }

        protected BaseSnapshot(ITextVersion version, StringRebuilder content)
        {
            Version = version;
            Content = content;
            CachingContent = CachingTextImage.Create(Content, version.ImageVersion);
            ContentType = version.TextBuffer.ContentType;
        }

        public string GetText(Span span)
        {
            return CachingContent.GetText(span);
        }

        public string GetText(int startIndex, int length)
        {
            return GetText(new Span(startIndex, length));
        }

        public string GetText()
        {
            return GetText(new Span(0, Length));
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            return CachingContent.ToCharArray(startIndex, length);
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            CachingContent.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        public char this[int position] => CachingContent[position];

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            return new TextSnapshotLine(this, CachingContent.GetLineFromLineNumber(lineNumber));
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            return Version.CreateTrackingPoint(position, trackingMode);
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            return Version.CreateTrackingPoint(position, trackingMode, trackingFidelity);
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            return Version.CreateTrackingSpan(start, length, trackingMode);
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            return Version.CreateTrackingSpan(start, length, trackingMode, trackingFidelity);
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            return Version.CreateTrackingSpan(span, trackingMode, TrackingFidelityMode.Forward);
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            return Version.CreateTrackingSpan(span, trackingMode, trackingFidelity);
        }

        public void SaveToFile(string filePath, bool replaceFile, Encoding encoding)
        {
            FileUtilities.SaveSnapshot(this, replaceFile ? FileMode.Create : FileMode.CreateNew, encoding, filePath);
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            return GetLineFromLineNumber(CachingContent.GetLineNumberFromPosition(position));
        }

        public int GetLineNumberFromPosition(int position)
        {
            return CachingContent.GetLineNumberFromPosition(position);
        }

        public IEnumerable<ITextSnapshotLine> Lines
        {
            get
            {
                var lineCount = CachingContent.LineCount;
                for (var line = 0; line < lineCount; ++line)
                {
                    yield return GetLineFromLineNumber(line);
                }
            }
        }

        public void Write(TextWriter writer, Span span)
        {
            CachingContent.Write(writer, span);
        }

        public void Write(TextWriter writer)
        {
            CachingContent.Write(writer, new Span(0, CachingContent.Length));
        }

        public override string ToString()
        {
            return
                $"version: {Version.VersionNumber} lines: {LineCount} length: {Length} \r\n content: {TextUtilities.Escape(GetText(0, Math.Min(40, Length)))}";
        }
    }
}