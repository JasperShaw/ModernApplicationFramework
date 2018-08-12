using System;
using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class TrivialNormalizedTextChangeCollection : INormalizedTextChangeCollection, ITextChange3
    {
        private readonly char _data;
        private readonly bool _isInsertion;
        private readonly int _position;

        public int Count => 1;

        public bool IncludesLineChanges => false;

        public bool IsOpaque { get; internal set; }

        public bool IsReadOnly => true;

        int ITextChange.Delta => !_isInsertion ? -1 : 1;

        int ITextChange.LineCountDelta => 0;

        int ITextChange.NewEnd
        {
            get
            {
                if (!_isInsertion)
                    return _position;
                return _position + 1;
            }
        }

        int ITextChange.NewLength => !_isInsertion ? 0 : 1;

        int ITextChange.NewPosition => _position;

        Span ITextChange.NewSpan => new Span(_position, _isInsertion ? 1 : 0);

        string ITextChange.NewText
        {
            get
            {
                if (!_isInsertion)
                    return "";
                return new string(_data, 1);
            }
        }

        int ITextChange.OldEnd
        {
            get
            {
                if (!_isInsertion)
                    return _position + 1;
                return _position;
            }
        }

        int ITextChange.OldLength => !_isInsertion ? 1 : 0;

        int ITextChange.OldPosition => _position;

        Span ITextChange.OldSpan => new Span(_position, _isInsertion ? 0 : 1);

        string ITextChange.OldText
        {
            get
            {
                if (!_isInsertion)
                    return new string(_data, 1);
                return "";
            }
        }

        public TrivialNormalizedTextChangeCollection(char data, bool isInsertion, int position)
        {
            _data = data;
            _isInsertion = isInsertion;
            _position = position;
        }

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

        public void Add(ITextChange item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
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
            array[arrayIndex] = this;
        }

        public IEnumerator<ITextChange> GetEnumerator()
        {
            yield return this;
        }

        public string GetNewText(Span span)
        {
            if (span.End > 1)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (!_isInsertion)
                return "";
            return new string(_data, span.Length);
        }

        public char GetNewTextAt(int position)
        {
            if (position > 0 || !_isInsertion)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _data;
        }

        public string GetOldText(Span span)
        {
            if (span.End > 1)
                throw new ArgumentOutOfRangeException(nameof(span));
            if (!_isInsertion)
                return new string(_data, span.Length);
            return "";
        }

        public char GetOldTextAt(int position)
        {
            if (position > 0 || _isInsertion)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _data;
        }

        public int IndexOf(ITextChange item)
        {
            return item != this ? -1 : 0;
        }

        public void Insert(int index, ITextChange item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(ITextChange item)
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
    }
}