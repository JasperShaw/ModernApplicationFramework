using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class ConnectionPoint<TEventType> : IConnectionPoint where TEventType : class
    {
        private readonly List<TEventType> _eventSinks = new List<TEventType>();
        private readonly ConnectionPointContainer _connectionPointContainer;
        private readonly IConnectionAdviseHelper _eventSource;
        private int _cookieCount;
        private Type _eventInterface;

        internal ConnectionPoint(ConnectionPointContainer connectionPointContainer, IConnectionAdviseHelper eventSource)
        {
            _connectionPointContainer = connectionPointContainer;
            _eventSource = eventSource;
        }

        public void GetConnectionInterface(out Guid pIid)
        {
            pIid = EventInterface.GUID;
        }

        public void GetConnectionPointContainer(out IConnectionPointContainer ppCpc)
        {
            ppCpc = (IConnectionPointContainer)_connectionPointContainer;
        }

        public void Advise(object sink, out int pdwCookie)
        {
            pdwCookie = 0;
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));
            if (!(sink is TEventType eventType))
                throw new ArgumentException("sink not castable to event type.");
            _eventSource.Advise(EventInterface, eventType);
            _eventSinks.Add(eventType);
            pdwCookie = ++_cookieCount;
        }

        public void Unadvise(int dwCookie)
        {
            int index = dwCookie - 1;
            if (index < 0 || index >= _eventSinks.Count || _eventSinks[index] == null)
                throw new ArgumentException("Invalid dwCookie.");
            TEventType eventSink = _eventSinks[index];
            _eventSinks[index] = default;
            _eventSource.Unadvise(EventInterface, eventSink);
        }

        public void EnumConnections(out IEnumConnections ppEnum)
        {
            ppEnum = new EnumConnections<TEventType>(_eventSinks);
        }

        private Type EventInterface
        {
            get
            {
                if (null == _eventInterface)
                    _eventInterface = typeof(TEventType);
                return _eventInterface;
            }
        }
    }
}