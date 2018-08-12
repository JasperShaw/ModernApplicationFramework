using System;
using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class SnapshotLineList : ITokenizedStringListInternal
    {
        private readonly Func<ITextSnapshotLine, string> _getLineTextCallback;
        private readonly Span _lineSpan;
        private readonly StringDifferenceOptions _options;
        private SnapshotSpan _snapshotSpan;

        public int Count => _lineSpan.Length;

        public bool IsReadOnly => true;

        public string Original => _snapshotSpan.GetText();

        public SnapshotLineList(SnapshotSpan snapshotSpan, Func<ITextSnapshotLine, string> getLineTextCallback,
            StringDifferenceOptions options)
        {
            if ((options.DifferenceType & StringDifferenceTypes.Line) == 0)
                throw new InvalidOperationException("This collection can only be used for line differencing");
            _snapshotSpan = snapshotSpan;
            _getLineTextCallback = getLineTextCallback ?? throw new ArgumentNullException(nameof(getLineTextCallback));
            _options = options;
            var containingLine = snapshotSpan.Start.GetContainingLine();
            var lineNumber = snapshotSpan.Start.GetContainingLine().LineNumber;
            var end1 = snapshotSpan.End;
            var end2 = (end1.Position < containingLine.EndIncludingLineBreak
                           ? lineNumber
                           : end1.GetContainingLine().LineNumber) + 1;
            _lineSpan = Span.FromBounds(lineNumber, end2);
        }

        public string this[int index]
        {
            get
            {
                var spanOfIndex = GetSpanOfIndex(index);
                var lineFromLineNumber = _snapshotSpan.Snapshot.GetLineFromLineNumber(_lineSpan.Start + index);
                var flag = spanOfIndex.Length != lineFromLineNumber.LengthIncludingLineBreak;
                var str = !flag ? _getLineTextCallback(lineFromLineNumber) : spanOfIndex.GetText();
                if (_options.IgnoreTrimWhiteSpace)
                {
                    if (flag)
                    {
                        if (spanOfIndex.Start == lineFromLineNumber.Start)
                            str = str.TrimStart(Array.Empty<char>());
                        if (spanOfIndex.End == lineFromLineNumber.EndIncludingLineBreak)
                            str = str.TrimEnd(Array.Empty<char>());
                    }
                    else
                    {
                        str = str.Trim();
                    }
                }

                return str;
            }
            set => throw new NotSupportedException();
        }

        public void Add(string item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(string item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public Span GetElementInOriginal(int index)
        {
            if (index == _lineSpan.Length)
                return new Span(_snapshotSpan.End, 0);
            var spanOfIndex = GetSpanOfIndex(index);
            return new Span(spanOfIndex.Start - _snapshotSpan.Start, spanOfIndex.Length);
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        public Span GetSpanInOriginal(Span span)
        {
            var start = GetElementInOriginal(span.Start).Start;
            if (span.IsEmpty)
                return new Span(start, 0);
            var end = GetElementInOriginal(span.End - 1).End;
            return Span.FromBounds(start, end);
        }

        public int IndexOf(string item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, string item)
        {
            throw new NotSupportedException();
        }

        public string OriginalSubstring(int startIndex, int length)
        {
            return _snapshotSpan.Snapshot.GetText(_snapshotSpan.Start + startIndex, length);
        }

        public bool Remove(string item)
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

        private SnapshotSpan GetSpanOfIndex(int index)
        {
            if (index < 0 || index >= _lineSpan.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            var lineFromLineNumber = _snapshotSpan.Snapshot.GetLineFromLineNumber(_lineSpan.Start + index);
            var nullable = lineFromLineNumber.ExtentIncludingLineBreak.Intersection(_snapshotSpan);
            if (!nullable.HasValue)
                return new SnapshotSpan(lineFromLineNumber.Start, 0);
            return nullable.Value;
        }
    }
}