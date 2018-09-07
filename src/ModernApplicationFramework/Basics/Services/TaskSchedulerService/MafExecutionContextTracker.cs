using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Core.InfoBarUtilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal sealed class MafExecutionContextTracker : DisposableObject, IMafExecutionContextTracker
    {
        private readonly AsyncLocal<MafExecutionContextStorage> _vsExecutionContextStore;
        private readonly CookieTable<uint, MafExecutionContextStorage> _contextCookies;
        private volatile HashSet<Tuple<Guid, IMafExecutionContextTrackerListener>> _listeners;
        private readonly ThreadLocal<List<PendingNotification>> _pendingNotifications;
        private readonly ThreadLocal<MafExecutionContextStorage> _noFlowContext;
        private readonly object _syncLock;
        private static MafExecutionContextTracker _instance;

        public MafExecutionContextTracker()
        {
            _instance = this;
            _contextCookies = new CookieTable<uint, MafExecutionContextStorage>(UIntCookieTraits.Default);
            _noFlowContext = new ThreadLocal<MafExecutionContextStorage> {Value = null};
            _pendingNotifications = new ThreadLocal<List<PendingNotification>>();
            _vsExecutionContextStore = new AsyncLocal<MafExecutionContextStorage>(OnExecutionContextValueChanged);
            _listeners = new HashSet<Tuple<Guid, IMafExecutionContextTrackerListener>>();
            _syncLock = new object();
        }

        private void OnExecutionContextValueChanged(AsyncLocalValueChangedArgs<MafExecutionContextStorage> args)
        {
            if (IsDisposed)
                return;
            var previousValue = args.PreviousValue;
            if (_noFlowContext.Value != null)
                previousValue = _noFlowContext.Value;
            RaiseNotificationForContextChange(previousValue, args.CurrentValue);
        }

        private MafExecutionContextStorage GetCurrentContextInternal()
        {
            if (IsDisposed)
                return MafExecutionContextStorage.GetEmptyContext();
            if (_noFlowContext.Value != null)
                return _noFlowContext.Value;
            return _vsExecutionContextStore.Value;
        }

        private void RaiseNotificationForContextChange(MafExecutionContextStorage oldContext, MafExecutionContextStorage newContext)
        {
            var listeners = _listeners;
            if (listeners == null || listeners.Count == 0)
                return;
            foreach (var tuple in listeners)
            {
                var oldValue = oldContext?.GetElement(tuple.Item1) ?? Guid.Empty;
                var newValue = newContext?.GetElement(tuple.Item1) ?? Guid.Empty;
                if (oldValue != newValue)
                {
                    if (!_pendingNotifications.IsValueCreated)
                        _pendingNotifications.Value = new List<PendingNotification>();
                    _pendingNotifications.Value.Add(new PendingNotification(tuple.Item1, oldValue, newValue, tuple.Item2));
                }
            }
            if (!_pendingNotifications.IsValueCreated)
                return;
            foreach (var pendingNotification in _pendingNotifications.Value)
                pendingNotification.Notify();
            _pendingNotifications.Value.Clear();
        }

        public uint GetCurrentContext()
        {
            var newContext = GetCurrentContextInternal();
            if (newContext == null || newContext.IsEmpty)
                return 0;
            if (newContext.IsNoFlowContext)
                newContext = new MafExecutionContextStorage(newContext.PreviousContext, newContext);
            return _contextCookies.Insert(newContext);
        }

        public void PopContext(uint contextCookie)
        {
            if (contextCookie == 0U || IsDisposed)
                return;
            var currentContextInternal = GetCurrentContextInternal();
            var previousContext = currentContextInternal?.PreviousContext;
            if (previousContext != null && _noFlowContext.Value != null && previousContext.IsNoFlowContext)
            {
                RaiseNotificationForContextChange(_noFlowContext.Value, previousContext);
                _noFlowContext.Value = previousContext;
            }
            else
            {
                if (_vsExecutionContextStore.Value == previousContext && currentContextInternal != null && currentContextInternal.IsNoFlowContext)
                    RaiseNotificationForContextChange(currentContextInternal, previousContext);
                else
                    _vsExecutionContextStore.Value = previousContext;
                _noFlowContext.Value = null;
            }
        }

        public void PushContext(uint contextCookie)
        {
            PushContextEx(contextCookie, false);
        }

        public void PushContextEx(uint contextCookie, bool dontTrackAsyncWork)
        {
            if (contextCookie == 0U || IsDisposed)
                return;
            var emptyContext = MafExecutionContextStorage.GetEmptyContext();
            if (!_contextCookies.TryGetValue(contextCookie, out emptyContext))
                return;
            var currentContextInternal = GetCurrentContextInternal();
            var newContext = new MafExecutionContextStorage(currentContextInternal, emptyContext);
            if (!dontTrackAsyncWork)
            {
                _vsExecutionContextStore.Value = newContext;
            }
            else
            {
                RaiseNotificationForContextChange(currentContextInternal, newContext);
                newContext.IsNoFlowContext = true;
                _noFlowContext.Value = newContext;
            }
        }

        public Guid SetAndGetContextElement(Guid contextTypeGuid, Guid contextElementGuid)
        {
            if (IsDisposed)
                return Guid.Empty;
            var executionContextStorage1 = GetCurrentContextInternal() ?? MafExecutionContextStorage.GetEmptyContext();
            var previousValue = Guid.Empty;
            var executionContextStorage2 = executionContextStorage1.UpdateElement(contextTypeGuid, contextElementGuid, out previousValue);
            _vsExecutionContextStore.Value = executionContextStorage2.IsEmpty ? null : executionContextStorage2;
            _noFlowContext.Value = null;
            return previousValue;
        }

        public void SetContextElement(Guid contextTypeGuid, Guid contextElementGuid)
        {
            SetAndGetContextElement(contextTypeGuid, contextElementGuid);
        }

        public Guid GetContextElement(Guid contextTypeGuid)
        {
            return (GetCurrentContextInternal() ?? MafExecutionContextStorage.GetEmptyContext()).GetElement(contextTypeGuid);
        }

        public void ReleaseContext(uint cookieContext)
        {
            if (cookieContext == 0U)
                return;
            _contextCookies.Remove(cookieContext);
        }

        public void Register(Guid contextValueType, IMafExecutionContextTrackerListener listener)
        {
            lock (_syncLock)
                _listeners = new HashSet<Tuple<Guid, IMafExecutionContextTrackerListener>>(_listeners)
                {
                    Tuple.Create(contextValueType, listener)
                };
        }

        public void Unregister(Guid contextValueType, IMafExecutionContextTrackerListener listener)
        {
            lock (_syncLock)
            {
                var tupleSet = new HashSet<Tuple<Guid, IMafExecutionContextTrackerListener>>(_listeners);
                tupleSet.Remove(Tuple.Create(contextValueType, listener));
                _listeners = tupleSet;
            }
        }

        private struct PendingNotification
        {
            private readonly Guid _contextType;
            private readonly Guid _oldValue;
            private readonly Guid _newValue;
            private readonly IMafExecutionContextTrackerListener _listener;

            public PendingNotification(Guid type, Guid oldValue, Guid newValue, IMafExecutionContextTrackerListener listener)
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
}