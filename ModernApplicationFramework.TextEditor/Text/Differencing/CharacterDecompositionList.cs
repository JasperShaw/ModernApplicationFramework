using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    internal class CharacterDecompositionList : ITokenizedStringListInternal
    {
        private readonly string _originalString;
        private SnapshotSpan _originalSpan;

        public CharacterDecompositionList(string original)
        {
            _originalString = original;
        }

        public CharacterDecompositionList(SnapshotSpan original)
        {
            _originalSpan = original;
        }

        public string Original => _originalString ?? _originalSpan.GetText();

        public string OriginalSubstring(int start, int length)
        {
            if (_originalString != null)
                return _originalString.Substring(start, length);
            return _originalSpan.Snapshot.GetText(start + _originalSpan.Start.Position, length);
        }

        public Span GetElementInOriginal(int index)
        {
            return new Span(index, index < Count ? 1 : 0);
        }

        public Span GetSpanInOriginal(Span span)
        {
            return span;
        }

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

        public string this[int index]
        {
            get => OriginalSubstring(index, 1);
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

        public int Count
        {
            get
            {
                if (_originalString == null)
                    return _originalSpan.Length;
                return _originalString.Length;
            }
        }

        public bool IsReadOnly => true;

        public bool Remove(string item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}