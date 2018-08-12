using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    public sealed class TextViewLineCollection : ITextViewLineCollection
    {
        private readonly SnapshotSpan _formattedSpan;
        private ITextView _textView;
        private readonly ReadOnlyCollection<ITextViewLine> _textLines;

        public int Count => _textLines.Count;

        public ITextViewLine FirstVisibleLine
        {
            get
            {
                ThrowIfInvalid();
                var line = _textLines[0];
                if (line.VisibilityState == VisibilityState.Hidden && _textLines.Count > 1 &&
                    _textLines[1].VisibilityState != VisibilityState.Hidden)
                    line = _textLines[1];
                return line;
            }
        }

        private void ThrowIfInvalid()
        {
            if (!IsValid)
                throw new ObjectDisposedException(nameof(TextViewLineCollection));
        }

        public SnapshotSpan FormattedSpan
        {
            get
            {
                ThrowIfInvalid();
                return _formattedSpan;
            }
        }

        public bool IsReadOnly => true;

        public bool IsValid => _textView != null;

        public ITextViewLine LastVisibleLine
        {
            get
            {
                ThrowIfInvalid();
                var textLine = _textLines[_textLines.Count - 1];
                if (textLine.VisibilityState == VisibilityState.Hidden && _textLines.Count > 1 &&
                    _textLines[_textLines.Count - 2].VisibilityState != VisibilityState.Hidden)
                    textLine = _textLines[_textLines.Count - 2];
                return textLine;
            }
        }

        public ReadOnlyCollection<ITextViewLine> TextViewLines
        {
            get
            {
                ThrowIfInvalid();
                return _textLines;
            }
        }

        public TextViewLineCollection(ITextView textView, IList<IFormattedLine> textLines)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _textLines = new ReadOnlyCollection<ITextViewLine>(CreateOffsetTextLines(textLines));
            _formattedSpan = new SnapshotSpan(_textView.TextSnapshot,
                Span.FromBounds(textLines[0].Start, textLines[textLines.Count - 1].EndIncludingLineBreak));
        }

        public ITextViewLine this[int index]
        {
            get => _textLines[index];
            set => throw new NotSupportedException();
        }

        public void Add(ITextViewLine item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(ITextViewLine item)
        {
            return IndexOf(item) != -1;
        }

        public bool ContainsBufferPosition(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            ValidateBufferPosition(bufferPosition);
            if (bufferPosition < (int)_formattedSpan.Start)
                return false;
            var snapshotPoint1 = bufferPosition;
            var formattedSpan = _formattedSpan;
            var end1 = formattedSpan.End;
            if (snapshotPoint1 < end1)
                return true;
            var snapshotPoint2 = bufferPosition;
            formattedSpan = _formattedSpan;
            var end2 = formattedSpan.End;
            if (snapshotPoint2 == end2 && bufferPosition == _textView.TextSnapshot.Length)
                return _textLines[_textLines.Count - 1].LineBreakLength == 0;
            return false;
        }

        public void CopyTo(ITextViewLine[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + Count < Count)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            for (var index = 0; index < Count; ++index)
                array[index + arrayIndex] = _textLines[index];
        }

        public TextBounds GetCharacterBounds(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            ValidateBufferPosition(bufferPosition);
            var containingBufferPosition = GetTextViewLineContainingBufferPosition(bufferPosition);
            if (containingBufferPosition == null)
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            return containingBufferPosition.GetCharacterBounds(bufferPosition);
        }

        public IEnumerator<ITextViewLine> GetEnumerator()
        {
            return _textLines.GetEnumerator();
        }

        public int GetIndexOfTextLine(ITextViewLine textLine)
        {
            ThrowIfInvalid();
            return FindTextViewLineIndexContainingBufferPosition(textLine.Start);
        }

        public Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan)
        {
            return GetLineMarkerGeometry(bufferSpan, false, Markers.MultiLinePadding);
        }

        public Geometry GetLineMarkerGeometry(SnapshotSpan bufferSpan, bool clipToBounds, Thickness padding)
        {
            return GetMarkerGeometry(bufferSpan, clipToBounds, padding, false);
        }

        public Geometry GetMarkerGeometry(SnapshotSpan bufferSpan, bool clipToBounds, Thickness padding)
        {
            ValidateBufferSpan(bufferSpan);
            if (Markers.MarkerGeometrySpansMultipleLines(this, bufferSpan))
                return GetLineMarkerGeometry(bufferSpan);
            return GetTextMarkerGeometry(bufferSpan);
        }

        public Geometry GetMarkerGeometry(SnapshotSpan bufferSpan)
        {
            ValidateBufferSpan(bufferSpan);
            if (Markers.MarkerGeometrySpansMultipleLines(this, bufferSpan))
                return GetLineMarkerGeometry(bufferSpan);
            return GetTextMarkerGeometry(bufferSpan);
        }

        public Collection<TextBounds> GetNormalizedTextBounds(SnapshotSpan bufferSpan)
        {
            ThrowIfInvalid();
            ValidateBufferSpan(bufferSpan);
            var textBoundsList = new List<TextBounds>();
            var intersectingSpan = GetTextViewLinesIntersectingSpan(bufferSpan);
            if (intersectingSpan.Count > 0)
            {
                textBoundsList.AddRange(intersectingSpan[0].GetNormalizedTextBounds(bufferSpan));
                for (var index = 1; index < intersectingSpan.Count - 1; ++index)
                    textBoundsList.Add(new TextBounds(intersectingSpan[index].Left, intersectingSpan[index].Top, intersectingSpan[index].Width, intersectingSpan[index].Height, intersectingSpan[index].TextTop, intersectingSpan[index].TextHeight));
                if (intersectingSpan.Count > 1)
                    textBoundsList.AddRange(intersectingSpan[intersectingSpan.Count - 1].GetNormalizedTextBounds(bufferSpan));
            }
            return new Collection<TextBounds>(textBoundsList);
        }

        public SnapshotSpan GetTextElementSpan(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            ValidateBufferPosition(bufferPosition);
            var containingBufferPosition = GetTextViewLineContainingBufferPosition(bufferPosition);
            if (containingBufferPosition == null)
                throw new ArgumentOutOfRangeException(nameof(bufferPosition));
            return containingBufferPosition.GetTextElementSpan(bufferPosition);
        }

        public Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan)
        {
            return GetTextMarkerGeometry(bufferSpan, false, Markers.SingleLinePadding);
        }

        public Geometry GetTextMarkerGeometry(SnapshotSpan bufferSpan, bool clipToViewport, Thickness padding)
        {
            return GetMarkerGeometry(bufferSpan, clipToViewport, padding, true);
        }

        public ITextViewLine GetTextViewLineContainingBufferPosition(SnapshotPoint bufferPosition)
        {
            ThrowIfInvalid();
            ValidateBufferPosition(bufferPosition);
            var containingBufferPosition = FindTextViewLineIndexContainingBufferPosition(bufferPosition);
            if (containingBufferPosition != -1)
                return _textLines[containingBufferPosition];
            return null;
        }

        public ITextViewLine GetTextViewLineContainingYCoordinate(double y)
        {
            ThrowIfInvalid();
            if (double.IsNaN(y))
                throw new ArgumentOutOfRangeException(nameof(y));
            var containingYcoordinate = FindTextViewLineIndexContainingYCoordinate(y);
            if (containingYcoordinate != -1)
                return _textLines[containingYcoordinate];
            return null;
        }

        public Collection<ITextViewLine> GetTextViewLinesIntersectingSpan(SnapshotSpan bufferSpan)
        {
            ThrowIfInvalid();
            ValidateBufferSpan(bufferSpan);
            if (!IntersectsBufferSpan(bufferSpan))
                return new Collection<ITextViewLine>();
            var nullable = bufferSpan.Intersection(_formattedSpan);
            var num1 = FindTextViewLineIndexContainingBufferPosition(nullable.Value.Start);
            if (num1 == -1)
                num1 = _textLines.Count - 1;
            var num2 = FindTextViewLineIndexContainingBufferPosition(nullable.Value.End);
            if (num2 == -1)
                num2 = _textLines.Count - 1;
            IList<ITextViewLine> list = new List<ITextViewLine>(num2 - num1 + 1);
            for (var index = num1; index <= num2; ++index)
                list.Add(_textLines[index]);
            return new Collection<ITextViewLine>(list);
        }

        public int IndexOf(ITextViewLine item)
        {
            if (!(item is ITextViewLine wpfTextViewLine))
                return -1;
            return _textLines.IndexOf(wpfTextViewLine);
        }

        public void Insert(int index, ITextViewLine item)
        {
            throw new NotSupportedException();
        }

        public bool IntersectsBufferSpan(SnapshotSpan bufferSpan)
        {
            ThrowIfInvalid();
            ValidateBufferSpan(bufferSpan);
            if (bufferSpan.End < (int)FormattedSpan.Start)
                return false;
            var start1 = bufferSpan.Start;
            var formattedSpan = _formattedSpan;
            var end1 = formattedSpan.End;
            if (start1 < end1)
                return true;
            var start2 = bufferSpan.Start;
            formattedSpan = _formattedSpan;
            var end2 = formattedSpan.End;
            if (start2 == end2 && bufferSpan.Start == _textView.TextSnapshot.Length)
                return _textLines[_textLines.Count - 1].LineBreakLength == 0;
            return false;
        }

        public bool Remove(ITextViewLine item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        internal Geometry GetMarkerGeometry(SnapshotSpan bufferSpan, bool clipToBounds, Thickness padding, bool useTextBounds)
        {
            ValidateBufferSpan(bufferSpan);
            return Markers.GetMarkerGeometryFromRectangles(Markers.GetRectanglesFromBounds(GetNormalizedTextBounds(bufferSpan), padding, clipToBounds ? _textView.ViewportLeft - 2.0 : double.MinValue, clipToBounds ? _textView.ViewportRight + 2.0 : double.MaxValue, useTextBounds));
        }

        private static IList<ITextViewLine> CreateOffsetTextLines(IList<IFormattedLine> textLines)
        {
            if (textLines == null)
                throw new ArgumentNullException(nameof(textLines));
            if (textLines.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(textLines));
            var wpfTextViewLineList = new List<ITextViewLine>(textLines.Count);
            wpfTextViewLineList.AddRange(textLines);
            return wpfTextViewLineList;
        }

        private void ValidateBufferPosition(SnapshotPoint bufferPosition)
        {
            if (bufferPosition.Snapshot != _formattedSpan.Snapshot)
                throw new ArgumentException();
        }

        private void ValidateBufferSpan(SnapshotSpan bufferSpan)
        {
            if (bufferSpan.Snapshot != _formattedSpan.Snapshot)
                throw new ArgumentException();
        }

        private int FindTextViewLineIndexContainingBufferPosition(SnapshotPoint position)
        {
            if (position.Position < _textLines[0].Start.Position)
                return -1;
            var textLine1 = _textLines[_textLines.Count - 1];
            var position1 = position.Position;
            var start = textLine1.Start;
            var position2 = start.Position;
            if (position1 >= position2)
            {
                if (!textLine1.ContainsBufferPosition(position))
                    return -1;
                return _textLines.Count - 1;
            }
            var num1 = 0;
            var num2 = _textLines.Count;
            while (num1 < num2)
            {
                var index = (num1 + num2) / 2;
                var textLine2 = _textLines[index];
                var position3 = position.Position;
                start = textLine2.Start;
                var position4 = start.Position;
                if (position3 < position4)
                    num2 = index;
                else
                    num1 = index + 1;
            }
            return num1 - 1;
        }

        private int FindTextViewLineIndexContainingYCoordinate(double y)
        {
            if (y < _textLines[0].Top || y >= _textLines[_textLines.Count - 1].Bottom)
                return -1;
            var num1 = 0;
            var num2 = _textLines.Count;
            while (num1 < num2)
            {
                var index = (num1 + num2) / 2;
                var textLine = _textLines[index];
                if (y < textLine.Top)
                    num2 = index;
                else
                    num1 = index + 1;
            }
            var index1 = num1 - 1;
            var textTop = _textLines[index1].TextTop;
            var num3 = _textLines[index1].TextBottom + 1.0;
            if (y < textTop)
            {
                if (index1 > 0)
                {
                    var num4 = _textLines[index1 - 1].TextBottom + 1.0;
                    if (y - num4 < textTop - y)
                        --index1;
                }
            }
            else if (y > num3 && index1 < _textLines.Count - 1 && _textLines[index1 + 1].TextTop - y < y - num3)
                ++index1;
            return index1;
        }

        public void Invalidate()
        {
            _textView = null;
        }
    }
}