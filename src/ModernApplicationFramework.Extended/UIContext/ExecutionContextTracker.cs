using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using ModernApplicationFramework.Core.InfoBarUtilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.UIContext
{
    [Export(typeof(IExecutionContextTracker))]
    internal class ExecutionContextTracker : DisposableObject, IExecutionContextTracker
    {
        private readonly ThreadLocal<ExecutionContextStorage> _noFlowContext;
        private readonly AsyncLocal<ExecutionContextStorage> _executionContextStore;
        private readonly Dictionary<Guid, List<IExecutionContextTrackerListener>> _listeners;
        private readonly ThreadLocal<List<PendingNotification>> _pendingNotifications;
        private readonly CookieTable<uint, ExecutionContextStorage> _contextCookies;


        public ExecutionContextTracker()
        {
            _contextCookies = new CookieTable<uint, ExecutionContextStorage>(UIntCookieTraits.Default);
            _noFlowContext = new ThreadLocal<ExecutionContextStorage> {Value = null};
            _executionContextStore = new AsyncLocal<ExecutionContextStorage>(OnExecutionContextValueChanged);
            _pendingNotifications = new ThreadLocal<List<PendingNotification>>();
            _listeners = new Dictionary<Guid, List<IExecutionContextTrackerListener>>();
        }

        public void SetContextElement(Guid contextTypeGuid, Guid contextElement)
        {
            SetAndGetContextElement(contextTypeGuid, contextElement);
        }

        public Guid SetAndGetContextElement(Guid contextTypeGuid, Guid contextElement)
        {
            if (IsDisposed)
                return Guid.Empty;
            var storage1 = GetCurrentContextInternal() ?? ExecutionContextStorage.GetEmptyContext();
            var storage2 = storage1.UpdateElement(contextTypeGuid, contextElement, out var previeousValue);
            _executionContextStore.Value = storage2.IsEmpty ? null : storage2;
            _noFlowContext.Value = null;
            return previeousValue;
        }

        public void PushContext(uint cookie)
        {
            PushContextEx(cookie, false);
        }

        public void PushContextEx(uint cookie, bool dontTrackAsyncWork)
        {
            if (cookie == 0 || IsDisposed)
                return;
            if (!_contextCookies.TryGetValue(cookie, out var emptyContext))
                return;
            var currentContext = GetCurrentContextInternal();
            var newContext = new ExecutionContextStorage(currentContext, emptyContext);
            if (!dontTrackAsyncWork)
                _executionContextStore.Value = newContext;
            else
            {
                RaiseNotificationForContextChange(currentContext, newContext);
                newContext.IsNoFlowContext = true;
                _noFlowContext.Value = newContext;
            }
        }

        public void PopContext(uint cookie)
        {
            if (cookie == 0 || IsDisposed)
                return;
            var currentContext = GetCurrentContextInternal();
            var previousContext = currentContext?.PreviousContext;
            if (previousContext != null && _noFlowContext.Value != null && previousContext.IsNoFlowContext)
            {
                RaiseNotificationForContextChange(_noFlowContext.Value, previousContext);
                _noFlowContext.Value = previousContext;
            }
            else
            {
                if (_executionContextStore.Value == previousContext && currentContext != null &&
                    currentContext.IsNoFlowContext)
                    RaiseNotificationForContextChange(currentContext, previousContext);
                else
                    _executionContextStore.Value = previousContext;
                _noFlowContext.Value = null;
            }
        }

        public uint GetCurrentContext()
        {
            var newContext = GetCurrentContextInternal();
            if (newContext == null || newContext.IsEmpty)
                return 0;
            if (newContext.IsNoFlowContext)
                newContext = new ExecutionContextStorage(newContext.PreviousContext, newContext);
            return _contextCookies.Insert(newContext);
        }

        public void ReleaseContext(uint cookie)
        {
            if (cookie == 0)
                return;
            _contextCookies.Remove(cookie);
        }

        private ExecutionContextStorage GetCurrentContextInternal()
        {
            if (IsDisposed)
                return ExecutionContextStorage.GetEmptyContext();
            return _noFlowContext.Value ?? _executionContextStore.Value;
        }

        private void OnExecutionContextValueChanged(AsyncLocalValueChangedArgs<ExecutionContextStorage> obj)
        {
            if (IsDisposed)
                return;
            var previousValue = obj.PreviousValue;
            if (_noFlowContext.Value != null)
                previousValue = _noFlowContext.Value;
            RaiseNotificationForContextChange(previousValue, obj.CurrentValue);
        }

        private void RaiseNotificationForContextChange(ExecutionContextStorage oldContext, ExecutionContextStorage newContext)
        {
            var listeners = _listeners;
            if (listeners == null || listeners.Count == 0)
                return;
            lock (listeners)
            {
                foreach (var keyValuePair in listeners)
                {
                    var oldValue = oldContext?.GetElement(keyValuePair.Key) ?? Guid.Empty;
                    var newValue = newContext?.GetElement(keyValuePair.Key) ?? Guid.Empty;
                    if (oldValue != newValue)
                    {
                        if (!_pendingNotifications.IsValueCreated)
                            _pendingNotifications.Value = new List<PendingNotification>();
                        foreach (var listener in keyValuePair.Value)
                            _pendingNotifications.Value.Add(new PendingNotification(keyValuePair.Key, oldValue, newValue, listener));
                    }
                }
            }
            if (!_pendingNotifications.IsValueCreated)
                return;
            foreach (var pendingNotification in _pendingNotifications.Value)
                pendingNotification.Notify();
            _pendingNotifications.Value.Clear();
        }

        private struct PendingNotification
        {
            private readonly Guid _contextType;
            private readonly Guid _oldValue;
            private readonly Guid _newValue;
            private readonly IExecutionContextTrackerListener _listener;

            public PendingNotification(Guid type, Guid oldValue, Guid newValue, IExecutionContextTrackerListener listener)
            {
                Validate.IsNotNull(listener, nameof(listener));
                _contextType = type;
                _oldValue = oldValue;
                _newValue = newValue;
                _listener = listener;
            }

            public void Notify()
            {
                _listener.OnExecutionContextValueChanged(_contextType, _oldValue, _newValue);
            }
        } 
    }

    public interface IExecutionContextTrackerListener
    {
        void OnExecutionContextValueChanged(Guid contextValueType, Guid previousValue, Guid newValue);
    }

    internal class ExecutionContextStorage
    {
        private static readonly ExecutionContextStorage EmptyContext = new ExecutionContextStorage();

        private readonly List<Tuple<Guid, Guid>> _elements;

        public ExecutionContextStorage PreviousContext { get; }

        public bool IsEmpty => _elements == null;

        public bool IsNoFlowContext { get; set; }

        private ExecutionContextStorage()
        {
            PreviousContext = null;
        }

        private ExecutionContextStorage(ExecutionContextStorage previousContext, List<Tuple<Guid, Guid>> elements)
        {
            PreviousContext = previousContext;
            _elements = elements;
        }

        public ExecutionContextStorage(ExecutionContextStorage previousContext, ExecutionContextStorage newContext)
        {
            _elements = newContext._elements;
            PreviousContext = previousContext;
        }

        public Guid GetElement(Guid elementType)
        {
            if (_elements != null)
            {
                foreach (var element in _elements)
                {
                    if (element.Item1 == elementType)
                        return element.Item2;
                }
            }
            return Guid.Empty;
        }

        public ExecutionContextStorage UpdateElement(Guid elementType, Guid value, out Guid previeousValue)
        {
            GetElement(elementType);
            var flag = true;
            previeousValue = Guid.Empty;
            if (value == Guid.Empty && (_elements == null || flag && _elements.Count == 1))
                return EmptyContext;
            var elements = new List<Tuple<Guid, Guid>>();
            if (value != Guid.Empty)
                elements.Add(Tuple.Create(elementType, value));
            if (_elements != null)
            {
                foreach (var element in _elements)
                {
                    if (element.Item1 != elementType)
                        elements.Add(element);
                    else
                        previeousValue = element.Item2;
                }
            }
            return new ExecutionContextStorage(PreviousContext, elements);
        }

        public static ExecutionContextStorage GetEmptyContext()
        {
            return EmptyContext;
        }

    }
}
