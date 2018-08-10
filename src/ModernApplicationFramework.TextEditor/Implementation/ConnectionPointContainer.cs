using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class ConnectionPointContainer
    {
        private readonly IConnectionAdviseHelper _source;
        private readonly Dictionary<Type, IConnectionPoint> _eventTypeToConnectionPointMap = new Dictionary<Type, IConnectionPoint>();
        private readonly Dictionary<Guid, Type> _guidToEventTypeMap = new Dictionary<Guid, Type>();

        public ConnectionPointContainer(object source)
        {
            if (!(source is IConnectionAdviseHelper eventSource))
                throw new ArgumentException();
            _source = eventSource;
        }

        public void AddEventType<TEventType>() where TEventType : class
        {
            var key = typeof(TEventType);
            if (!key.IsInterface)
                throw new ArgumentException("Not an interface");
            if (_eventTypeToConnectionPointMap.TryGetValue(key, out _))
                throw new ArgumentException("already added");
            _eventTypeToConnectionPointMap.Add(key, new ConnectionPoint<TEventType>(this, _source));
            _guidToEventTypeMap.Add(key.GUID, key);
        }

        public void EnumConnectionPoints(out IEnumConnectionPoints ppEnum)
        {
            ppEnum = new EnumConnectionPoints(_eventTypeToConnectionPointMap);
        }

        public void FindConnectionPoint(ref Guid riid, out IConnectionPoint ppCP)
        {
            ppCP = null;
            if (!_guidToEventTypeMap.TryGetValue(riid, out var key) || !_eventTypeToConnectionPointMap.TryGetValue(key, out var connectionPoint))
                throw new ArgumentException("Connection point not found for given riid.");
            ppCP = connectionPoint;
        }
    }
}