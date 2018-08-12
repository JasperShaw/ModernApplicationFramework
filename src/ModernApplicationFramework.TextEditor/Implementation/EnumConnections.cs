using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class EnumConnections<TEventType> : IEnumConnections
    {
        private readonly List<TEventType> _eventSinks;
        private List<TEventType>.Enumerator _connectionEnumerator;
        private int _currentCookie;

        internal EnumConnections(List<TEventType> eventSinks)
        {
            _eventSinks = eventSinks;
            _connectionEnumerator = eventSinks.GetEnumerator();
            _currentCookie = 0;
        }

        public int Next(int cConnections, CONNECTDATA[] rgcd, IntPtr pcFetched)
        {
            int num1 = -2147024809;
            if (cConnections > 0U && rgcd != null)
            {
                uint num2 = 0;
                if (cConnections > (long)rgcd.Length)
                {
                    cConnections = rgcd.Length;
                }
                else
                {
                    num1 = 0;
                    for (; num2 < cConnections; ++num2)
                    {
                        if (!MoveNextNonNull())
                        {
                            num1 = 1;
                            break;
                        }
                        rgcd[(int)num2].dwCookie = _currentCookie;
                        rgcd[(int)num2].pUnk = _connectionEnumerator.Current;
                    }
                }
                for (; num2 < cConnections; ++num2)
                {
                    rgcd[(int)num2].dwCookie = 0;
                    rgcd[(int)num2].pUnk = null;
                }
            }
            return num1;
        }

        public int Skip(int cConnections)
        {
            if (cConnections == 0U)
                return -2147024809;
            for (uint index = 0; index < cConnections; ++index)
            {
                if (!MoveNextNonNull())
                    return 1;
            }
            return 0;
        }

        public void Reset()
        {
            _connectionEnumerator = _eventSinks.GetEnumerator();
            _currentCookie = 0;
        }

        public void Clone(out IEnumConnections ppenum)
        {
            ppenum = new EnumConnections<TEventType>(_eventSinks);
        }

        private bool MoveNextNonNull()
        {
            while (_connectionEnumerator.MoveNext())
            {
                ++_currentCookie;
                if (_connectionEnumerator.Current != null)
                    return true;
            }
            return false;
        }
    }
}