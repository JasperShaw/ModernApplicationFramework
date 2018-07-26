using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    internal class TrivialNormalizedTextChangeCollection : INormalizedTextChangeCollection,ITextChange3
    {
        private readonly char _data;
        private readonly bool _isInsertion;
        private readonly int _position;

        public TrivialNormalizedTextChangeCollection(char data, bool isInsertion, int position)
        {
            _data = data;
            _isInsertion = isInsertion;
            _position = position;
        }

        public bool IncludesLineChanges => false;

        public ITextChange this[int index]
        {
            get
            {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return this;
            }
            set => throw new NotSupportedException();
        }

        public void Insert(int index, ITextChange item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int Count => 1;

        public bool IsReadOnly => true;

        public void Add(ITextChange item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public int IndexOf(ITextChange item)
        {
            return item != this ? -1 : 0;
        }

        public bool Contains(ITextChange item)
        {
            return item == this;
        }

        public void CopyTo(ITextChange[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Rank > 1 || arrayIndex >= array.Length)
                throw new ArgumentException("Bad arguments to CopyTo");
            array[arrayIndex] = (ITextChange)this;
        }

        public bool Remove(ITextChange item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<ITextChange> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Span ITextChange.OldSpan => new Span(_position, _isInsertion ? 0 : 1);

        Span ITextChange.NewSpan => new Span(_position, _isInsertion ? 1 : 0);

        int ITextChange.OldPosition => _position;

        int ITextChange.NewPosition => _position;

        int ITextChange.Delta => !_isInsertion ? -1 : 1;

        int ITextChange.OldEnd
        {
            get
            {
                if (!_isInsertion)
                    return _position + 1;
                return _position;
            }
        }

        int ITextChange.NewEnd
        {
            get
            {
                if (!_isInsertion)
                    return _position;
                return _position + 1;
            }
        }

        string ITextChange.OldText
        {
            get
            {
                if (!_isInsertion)
                    return new string(_data, 1);
                return "";
            }
        }

        string ITextChange.NewText
        {
            get
            {
                if (!_isInsertion)
                    return "";
                return new string(_data, 1);
            }
        }

        int ITextChange.OldLength => !_isInsertion ? 1 : 0;

        int ITextChange.NewLength => !_isInsertion ? 0 : 1;

        int ITextChange.LineCountDelta => 0;

        public bool IsOpaque { get; internal set; }

        public string GetOldText(Span span)
        {
            if (span.End > 1)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (!_isInsertion)
                return new string(_data, span.Length);
            return "";
        }

        public string GetNewText(Span span)
        {
            if (span.End > 1)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (!_isInsertion)
                return "";
            return new string(_data, span.Length);
        }

        public char GetOldTextAt(int position)
        {
            if (position > 0 || _isInsertion)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _data;
        }

        public char GetNewTextAt(int position)
        {
            if (position > 0 || !_isInsertion)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _data;
        }
    }
}