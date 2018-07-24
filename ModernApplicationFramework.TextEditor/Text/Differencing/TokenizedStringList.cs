using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    internal abstract class TokenizedStringList : ITokenizedStringListInternal
    {
        protected List<Span> Tokens = new List<Span>();
        private readonly string _original;
        private readonly SnapshotSpan _originalSpan;

        protected TokenizedStringList(string original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        protected TokenizedStringList(SnapshotSpan originalSpan)
        {
            _originalSpan = originalSpan;
        }

        public string Original => _original ?? _originalSpan.GetText();

        internal int OriginalLength => _original?.Length ?? _originalSpan.Length;

        public string OriginalSubstring(int startIndex, int length)
        {
            if (_original != null)
                return _original.Substring(startIndex, length);
            var originalSpan = _originalSpan;
            var snapshot = originalSpan.Snapshot;
            originalSpan = _originalSpan;
            var startIndex1 = originalSpan.Start.Position + startIndex;
            var length1 = length;
            return snapshot.GetText(startIndex1, length1);
        }

        public Span GetElementInOriginal(int index)
        {
            if (index == Count)
                return new Span(OriginalLength, 0);
            return Tokens[index];
        }

        public Span GetSpanInOriginal(Span span)
        {
            if (span.Start == Tokens.Count)
                return new Span(OriginalLength, 0);
            var start = Tokens[span.Start].Start;
            var end = span.Length == 0 ? start : Tokens[span.End - 1].End;
            return Span.FromBounds(start, end);
        }

        public string this[int index]
        {
            get
            {
                var elementInOriginal = GetElementInOriginal(index);
                return OriginalSubstring(elementInOriginal.Start, elementInOriginal.Length);
            }
            set => throw new NotSupportedException();
        }

        internal char CharacterAt(int offset)
        {
            if (_original == null)
                return _originalSpan.Snapshot[_originalSpan.Start.Position + offset];
            return _original[offset];
        }

        public int Count => Tokens.Count;

        public bool IsReadOnly => true;

        public int IndexOf(string item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, string item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
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

        public bool Remove(string item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}