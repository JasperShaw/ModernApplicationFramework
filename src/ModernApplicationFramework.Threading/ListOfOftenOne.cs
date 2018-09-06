using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal struct ListOfOftenOne<T> : IEnumerable<T> where T : class
    {
        private object _value;

        public Enumerator GetEnumerator()
        {
            return new Enumerator(Volatile.Read(ref _value));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T value)
        {
            object priorValue;
            object fieldBeforeExchange;
            do
            {
                priorValue = Volatile.Read(ref _value);
                var newValue = Combine(priorValue, value);
                fieldBeforeExchange = Interlocked.CompareExchange(ref _value, newValue, priorValue);
            } while (priorValue != fieldBeforeExchange);
        }

        public void Remove(T value)
        {
            object priorValue;
            object fieldBeforeExchange;
            do
            {
                priorValue = Volatile.Read(ref _value);
                var newValue = Remove(priorValue, value);
                fieldBeforeExchange = Interlocked.CompareExchange(ref _value, newValue, priorValue);
            } while (priorValue != fieldBeforeExchange);
        }

        public bool Contains(T value)
        {
            foreach (var item in this)
            {
                if (item == value)
                {
                    return true;
                }
            }

            return false;
        }

        internal Enumerator EnumerateAndClear()
        {
            // Enumeration is atomically destructive.
            var enumeratedValue = Interlocked.Exchange(ref _value, null);
            return new Enumerator(enumeratedValue);
        }

        private static object Combine(object baseValue, T value)
        {
            Validate.IsNotNull(value, nameof(value));

            if (baseValue == null)
            {
                return value;
            }

            if (baseValue is T singleValue)
            {
                return new[] { singleValue, value };
            }

            var oldArray = (T[])baseValue;
            var result = new T[oldArray.Length + 1];
            oldArray.CopyTo(result, 0);
            result[result.Length - 1] = value;
            return result;
        }

        private static object Remove(object baseValue, T value)
        {
            if (baseValue == value || baseValue == null)
            {
                return null;
            }

            if (baseValue is T)
            {
                return baseValue; // the value to remove wasn't in the list anyway.
            }

            var oldArray = (T[])baseValue;
            var index = Array.IndexOf(oldArray, value);
            if (index < 0)
            {
                return baseValue;
            }
            else if (oldArray.Length == 2)
            {
                return oldArray[index == 0 ? 1 : 0]; // return the one remaining value.
            }
            else
            {
                var result = new T[oldArray.Length - 1];
                Array.Copy(oldArray, result, index);
                Array.Copy(oldArray, index + 1, result, index, result.Length - index);
                return result;
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private const int IndexBeforeFirstArrayElement = -1;
            private const int IndexSingleElement = -2;
            private const int IndexBeforeSingleElement = -3;

            private readonly object _enumeratedValue;

            private int _currentIndex;

            internal Enumerator(object enumeratedValue)
            {
                _enumeratedValue = enumeratedValue;
                _currentIndex = 0;
                Reset();
            }

            public T Current
            {
                get
                {
                    if (_currentIndex == IndexBeforeFirstArrayElement || _currentIndex == IndexBeforeSingleElement)
                    {
                        throw new InvalidOperationException();
                    }

                    return _currentIndex == IndexSingleElement
                        ? (T)_enumeratedValue
                        : ((T[])_enumeratedValue)[_currentIndex];
                }
            }

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentIndex == IndexBeforeSingleElement && _enumeratedValue != null)
                {
                    _currentIndex = IndexSingleElement;
                    return true;
                }

                if (_currentIndex == IndexSingleElement)
                {
                    return false;
                }

                if (_currentIndex == IndexBeforeFirstArrayElement)
                {
                    _currentIndex = 0;
                    return true;
                }

                var array = (T[])_enumeratedValue;
                if (_currentIndex >= 0 && _currentIndex < array.Length)
                {
                    _currentIndex++;
                    return _currentIndex < array.Length;
                }

                return false;
            }

            public void Reset()
            {
                _currentIndex = _enumeratedValue is T[] ? IndexBeforeFirstArrayElement : IndexBeforeSingleElement;
            }
        }
    }
}
