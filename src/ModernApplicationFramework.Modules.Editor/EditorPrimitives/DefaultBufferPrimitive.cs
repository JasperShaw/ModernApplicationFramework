using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class DefaultBufferPrimitive : PrimitiveTextBuffer
    {
        private readonly IBufferPrimitivesFactoryService _bufferPrimitivesFactory;

        public DefaultBufferPrimitive(ITextBuffer textBuffer, IBufferPrimitivesFactoryService bufferPrimitivesFactory)
        {
            AdvancedTextBuffer = textBuffer;
            _bufferPrimitivesFactory = bufferPrimitivesFactory;
        }

        public override TextPoint GetTextPoint(int position)
        {
            if (position < 0 || position > AdvancedTextBuffer.CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _bufferPrimitivesFactory.CreateTextPoint(this, position);
        }

        public override TextPoint GetTextPoint(int line, int column)
        {
            if (line < 0 || line > AdvancedTextBuffer.CurrentSnapshot.LineCount)
                throw new ArgumentOutOfRangeException(nameof(line));
            ITextSnapshotLine lineFromLineNumber = AdvancedTextBuffer.CurrentSnapshot.GetLineFromLineNumber(line);
            if (column < 0 || column > lineFromLineNumber.Length)
                throw new ArgumentOutOfRangeException(nameof(column));
            return _bufferPrimitivesFactory.CreateTextPoint(this, lineFromLineNumber.Start + column);
        }

        public override TextRange GetLine(int line)
        {
            if (line < 0 || line > AdvancedTextBuffer.CurrentSnapshot.LineCount)
                throw new ArgumentOutOfRangeException(nameof(line));
            ITextSnapshotLine lineFromLineNumber = AdvancedTextBuffer.CurrentSnapshot.GetLineFromLineNumber(line);
            return GetTextRange(lineFromLineNumber.Extent.Start, lineFromLineNumber.Extent.End);
        }

        public override TextRange GetTextRange(TextPoint startPoint, TextPoint endPoint)
        {
            if (startPoint == null)
                throw new ArgumentNullException(nameof(startPoint));
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            if (!ReferenceEquals(startPoint.TextBuffer, this))
                throw new ArgumentException();
            if (!ReferenceEquals(endPoint.TextBuffer, this))
                throw new ArgumentException();
            return _bufferPrimitivesFactory.CreateTextRange(this, startPoint, endPoint);
        }

        public override TextRange GetTextRange(int startPosition, int endPosition)
        {
            if (startPosition < 0 || startPosition > AdvancedTextBuffer.CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(startPosition));
            if (endPosition < 0 || endPosition > AdvancedTextBuffer.CurrentSnapshot.Length)
                throw new ArgumentOutOfRangeException(nameof(endPosition));
            return _bufferPrimitivesFactory.CreateTextRange(this, GetTextPoint(startPosition), GetTextPoint(endPosition));
        }

        public override ITextBuffer AdvancedTextBuffer { get; }

        public override TextPoint GetStartPoint()
        {
            return GetTextPoint(0);
        }

        public override TextPoint GetEndPoint()
        {
            return GetTextPoint(AdvancedTextBuffer.CurrentSnapshot.Length);
        }

        public override IEnumerable<TextRange> Lines
        {
            get
            {
                foreach (ITextSnapshotLine line in AdvancedTextBuffer.CurrentSnapshot.Lines)
                    yield return GetTextRange(line.Start, line.End);
            }
        }
    }
}