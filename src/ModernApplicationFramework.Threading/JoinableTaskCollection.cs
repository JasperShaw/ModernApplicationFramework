using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public class JoinableTaskCollection : IEnumerable<JoinableTask>
    {
        private readonly WeakKeyDictionary<JoinableTask, int> _joinables =
            new WeakKeyDictionary<JoinableTask, int>(null, 2);

        private readonly WeakKeyDictionary<JoinableTask, int> _joiners = new WeakKeyDictionary<JoinableTask, int>(null, 2);

        private readonly bool _refCountAddedJobs;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _displayName;

        private AsyncManualResetEvent _emptyEvent;

        public JoinableTaskContext Context { get; }

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        public JoinableTaskCollection(JoinableTaskContext context, bool refCountAddedJobs = false)
        {
            Validate.IsNotNull(context, nameof(context));
            Context = context;
            _refCountAddedJobs = refCountAddedJobs;
        }

        public void Add(JoinableTask joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));
            if (joinableTask.Factory.Context != Context)
            {
                throw new ArgumentException(nameof(joinableTask));
            }

            if (!joinableTask.IsCompleted)
            {
                using (Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (Context.SyncContextLock)
                    {
                        if (!_joinables.TryGetValue(joinableTask, out var refCount) || _refCountAddedJobs)
                        {
                            _joinables[joinableTask] = refCount + 1;
                            if (refCount == 0)
                            {
                                joinableTask.OnAddedToCollection(this);
                                foreach (var joiner in _joiners)
                                    joiner.Key.AddDependency(joinableTask);
                            }
                        }

                        _emptyEvent?.Reset();
                    }
                }
            }
        }

        public void Remove(JoinableTask joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));

            using (Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (Context.SyncContextLock)
                {
                    if (_joinables.TryGetValue(joinableTask, out var refCount))
                    {
                        if (refCount == 1 || joinableTask.IsCompleted)
                        {
                            _joinables.Remove(joinableTask);
                            joinableTask.OnRemovedFromCollection(this);
                            foreach (var joiner in _joiners)
                                joiner.Key.RemoveDependency(joinableTask);

                            if (_emptyEvent != null && _joinables.Count == 0)
                            {
                                _emptyEvent.Set();
                            }
                        }
                        else
                        {
                            _joinables[joinableTask] = refCount - 1;
                        }
                    }
                }
            }
        }

        public JoinRelease Join()
        {
            var ambientJob = Context.AmbientTask;
            if (ambientJob == null)
                return default;

            using (Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (Context.SyncContextLock)
                {
                    _joiners.TryGetValue(ambientJob, out var count);
                    _joiners[ambientJob] = count + 1;
                    if (count == 0)
                    {
                        foreach (var joinable in _joinables)
                            ambientJob.AddDependency(joinable.Key);
                    }

                    return new JoinRelease(this, ambientJob);
                }
            }
        }

        public async Task JoinTillEmptyAsync()
        {
            if (_emptyEvent == null)
            {
                using (Context.NoMessagePumpSynchronizationContext.Apply())
                {
                    lock (Context.SyncContextLock)
                    {
                        Interlocked.CompareExchange(ref _emptyEvent, new AsyncManualResetEvent(_joinables.Count == 0), null);
                    }
                }
            }

            using (Join())
            {
                await _emptyEvent.WaitAsync().ConfigureAwait(false);
            }
        }

        public bool Contains(JoinableTask joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));

            using (Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (Context.SyncContextLock)
                {
                    return _joinables.ContainsKey(joinableTask);
                }
            }
        }

        public IEnumerator<JoinableTask> GetEnumerator()
        {
            using (Context.NoMessagePumpSynchronizationContext.Apply())
            {
                var joinables = new List<JoinableTask>();
                lock (Context.SyncContextLock)
                {
                    foreach (var item in _joinables)
                    {
                        joinables.Add(item.Key);
                    }
                }

                return joinables.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Disjoin(JoinableTask joinableTask)
        {
            Validate.IsNotNull(joinableTask, nameof(joinableTask));

            using (Context.NoMessagePumpSynchronizationContext.Apply())
            {
                lock (Context.SyncContextLock)
                {
                    _joiners.TryGetValue(joinableTask, out var count);
                    if (count == 1)
                    {
                        _joiners.Remove(joinableTask);

                        // We also need to disjoin this joinable task from all joinable tasks in this collection.
                        foreach (var joinable in _joinables)
                        {
                            joinableTask.RemoveDependency(joinable.Key);
                        }
                    }
                    else
                    {
                        _joiners[joinableTask] = count - 1;
                    }
                }
            }
        }

        public struct JoinRelease : IDisposable
        {
            private JoinableTask _joinedJob;
            private JoinableTask _joiner;
            private JoinableTaskCollection _joinedJobCollection;

            internal JoinRelease(JoinableTask joined, JoinableTask joiner)
            {
                Validate.IsNotNull(joined, nameof(joined));
                Validate.IsNotNull(joiner, nameof(joiner));

                _joinedJobCollection = null;
                _joinedJob = joined;
                _joiner = joiner;
            }

            internal JoinRelease(JoinableTaskCollection jobCollection, JoinableTask joiner)
            {
                Validate.IsNotNull(jobCollection, nameof(jobCollection));
                Validate.IsNotNull(joiner, nameof(joiner));

                _joinedJobCollection = jobCollection;
                _joinedJob = null;
                _joiner = joiner;
            }

            public void Dispose()
            {
                if (_joinedJob != null)
                {
                    _joinedJob.RemoveDependency(_joiner);
                    _joinedJob = null;
                }

                if (_joinedJobCollection != null)
                {
                    _joinedJobCollection.Disjoin(_joiner);
                    _joinedJobCollection = null;
                }

                _joiner = null;
            }
        }
    }
}
