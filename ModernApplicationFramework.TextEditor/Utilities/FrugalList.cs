using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal class FrugalList<T> : IList<T>, IReadOnlyList<T>
    {
        private static readonly List<T> UnitaryTail = new List<T>(0);
        private T _head;
        private List<T> _tail;

        public int Count
        {
            get
            {
                if (_tail == null)
                    return 0;
                return 1 + _tail.Count;
            }
        }

        public bool IsReadOnly => false;

        public FrugalList()
        {
            
        }

        public FrugalList(IList<T> elements)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));
            switch (elements.Count)
            {
                case 0:
                    break;
                case 1:
                    _head = elements[0];
                    _tail = UnitaryTail;
                    break;
                default:
                    _head = elements[0];
                    _tail = new List<T>(2);
                    for (var index = 1; index < elements.Count; ++index)
                        _tail.Add(elements[index]);
                    break;
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            var num = 0;
            for (var index = Count - 1; index >= 0; --index)
            {
                if (match(this[index]))
                {
                    ++num;
                    RemoveAt(index);
                }
            }
            return num;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new FrugalEnumerator(this);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FrugalEnumerator(this);
        }

        public void Add(T item)
        {
            if (Count == 0)
            {
                _head = item;
                _tail = UnitaryTail;
            }
            else
            {
                if (_tail == UnitaryTail)
                    _tail = new List<T>(2);
                _tail.Add(item);
            }
        }

        public void AddRange(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            foreach (var t in list)
                Add(t);
        }

        public void Clear()
        {
            _head = default(T);
            _tail = null;
        }

        public bool Contains(T item)
        {
            var count = Count;
            if (count > 0)
            {
                if (EqualityComparer<T>.Default.Equals(_head, item))
                    return true;
                if (count > 1)
                    return _tail.Contains(item);
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            var count = Count;
            if (count <= 0)
                return;
            array[arrayIndex++] = _head;
            if (count <= 1)
                return;
            _tail.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (Count <= 0)
                return false;
            if (!EqualityComparer<T>.Default.Equals(_head, item))
                return _tail.Remove(item);
            RemoveAt(0);
            return true;
        }

        public int IndexOf(T item)
        {
            if (Count <= 0)
                return -1;
            if (EqualityComparer<T>.Default.Equals(_head, item))
                return 0;
            var num = _tail.IndexOf(item);
            if (num < 0)
                return -1;
            return num + 1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index == 0)
            {
                switch (Count)
                {
                    case 0:
                        _tail = UnitaryTail;
                        break;
                    case 1:
                        _tail = new List<T>(2);
                        _tail.Add(_head);
                        break;
                    default:
                        _tail.Insert(0, _head);
                        break;
                }
                _head = item;
            }
            else
            {
                if (_tail == UnitaryTail)
                    _tail = new List<T>(2);
                _tail.Insert(index - 1, item);
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var count = Count;
            if (index == 0)
            {
                if (count == 1)
                {
                    _head = default(T);
                    _tail = null;
                }
                else
                {
                    _head = _tail[0];
                    if (count == 2)
                        _tail = UnitaryTail;
                    else
                        _tail.RemoveAt(0);
                }
            }
            else if (count == 2)
                _tail = UnitaryTail;
            else
                _tail.RemoveAt(index - 1);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (index == 0)
                    return _head;
                return _tail[index - 1];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (index == 0)
                    _head = value;
                else
                    _tail[index - 1] = value;
            }
        }

        public struct FrugalEnumerator : IEnumerator<T>
        {
            private readonly FrugalList<T> _list;
            private int _position;

            public FrugalEnumerator(FrugalList<T> list)
            {
                _list = list;
                _position = -1;
            }

            public T Current => _list[_position];

            object IEnumerator.Current => _list[_position];

            public bool MoveNext()
            {
                if (_position >= _list.Count - 1)
                    return false;
                ++_position;
                return true;
            }

            public void Reset()
            {
                _position = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}