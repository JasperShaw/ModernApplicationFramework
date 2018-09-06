using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    public class AsyncManualResetEvent
    {
        private readonly bool _allowInliningAwaiters;
        private readonly object _syncObject = new object();
        private TaskCompletionSourceWithoutInlining<EmptyStruct> _taskCompletionSource;
        private bool _isSet;

        public bool IsSet
        {
            get
            {
                lock (_syncObject)
                {
                    return _isSet;
                }
            }
        }

        public AsyncManualResetEvent(bool initialState = false, bool allowInliningAwaiters = false)
        {
            _allowInliningAwaiters = allowInliningAwaiters;

            _taskCompletionSource = CreateTaskSource();
            _isSet = initialState;
            if (initialState)
            {
                _taskCompletionSource.SetResult(EmptyStruct.Instance);
            }
        }

        public Task WaitAsync()
        {
            lock (_syncObject)
            {
                return _taskCompletionSource.Task;
            }
        }

        [Obsolete("Use Set() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public Task SetAsync()
        {
            TaskCompletionSourceWithoutInlining<EmptyStruct> tcs;
            bool transitionRequired;
            lock (_syncObject)
            {
                transitionRequired = !_isSet;
                tcs = _taskCompletionSource;
                _isSet = true;
            }

            Task result = tcs.Task;

            if (transitionRequired)
            {
                tcs.TrySetResult(default);
            }

            return result;
        }

        public void Set()
        {
#pragma warning disable CS0618
            SetAsync();
#pragma warning restore CS0618
        }

        public void Reset()
        {
            lock (_syncObject)
            {
                if (_isSet)
                {
                    _taskCompletionSource = CreateTaskSource();
                    _isSet = false;
                }
            }
        }

        [Obsolete("Use PulseAll() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public Task PulseAllAsync()
        {
            TaskCompletionSourceWithoutInlining<EmptyStruct> tcs;
            lock (_syncObject)
            {
                tcs = _taskCompletionSource;
                _taskCompletionSource = CreateTaskSource();
                _isSet = false;
            }

            Task result = tcs.Task;
            tcs.TrySetResult(default);
            return result;
        }

        public void PulseAll()
        {
#pragma warning disable CS0618
            PulseAllAsync();
#pragma warning restore CS0618
        }

        public TaskAwaiter GetAwaiter()
        {
            return WaitAsync().GetAwaiter();
        }

        private TaskCompletionSourceWithoutInlining<EmptyStruct> CreateTaskSource()
        {
            return new TaskCompletionSourceWithoutInlining<EmptyStruct>(_allowInliningAwaiters);
        }
    }
}
