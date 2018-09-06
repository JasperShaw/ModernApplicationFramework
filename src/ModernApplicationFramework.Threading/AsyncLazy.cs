using System;
using System.Threading;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    public class AsyncLazy<T>
    {
        private static readonly object RecursiveCheckSentinel = new object();
        private readonly object _syncObject = new object();
        private readonly AsyncLocal<object> _recursiveFactoryCheck = new AsyncLocal<object>();
        private Func<Task<T>> _valueFactory;

        private Task<T> _value;
        private JoinableTaskFactory _jobFactory;
        private JoinableTask<T> _joinableTask;

        public bool IsValueCreated
        {
            get
            {
                Interlocked.MemoryBarrier();
                return _valueFactory == null;
            }
        }

        public bool IsValueFactoryCompleted
        {
            get
            {
                Interlocked.MemoryBarrier();
                return _value != null && _value.IsCompleted;
            }
        }


        public AsyncLazy(Func<Task<T>> valueFactory, JoinableTaskFactory joinableTaskFactory = null)
        {
            Validate.IsNotNull(valueFactory, nameof(valueFactory));
            _valueFactory = valueFactory;
            _jobFactory = joinableTaskFactory;
        }

        public Task<T> GetValueAsync() => GetValueAsync(CancellationToken.None);

        public Task<T> GetValueAsync(CancellationToken cancellationToken)
        {
            if ((_value == null || !_value.IsCompleted) && _recursiveFactoryCheck.Value != null)
                throw new InvalidOperationException();
            if (_value == null)
            {
                if (Monitor.IsEntered(_syncObject))
                    throw new InvalidOperationException();

                InlineResumable resumableAwaiter = null;
                lock (_syncObject)
                {
                    if (_value == null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        resumableAwaiter = new InlineResumable();
                        var originalValueFactory = _valueFactory;
                        _valueFactory = null;

                        async Task<T> ValueFactory()
                        {
                            try
                            {
                                await resumableAwaiter;
                                return await originalValueFactory().ConfigureAwaitRunInline();
                            }
                            finally
                            {
                                _jobFactory = null;
                                _joinableTask = null;
                            }
                        }

                        _recursiveFactoryCheck.Value = RecursiveCheckSentinel;
                        try
                        {
                            if (_jobFactory != null)
                            {
                                _joinableTask = _jobFactory.RunAsync((Func<Task<T>>) ValueFactory);
                                _value = _joinableTask.Task;
                            }
                            else
                            {
                                _value = ValueFactory();
                            }
                        }
                        finally
                        {
                            _recursiveFactoryCheck.Value = null;
                        }
                    }
                }

                resumableAwaiter?.Resume();
            }

            if (!_value.IsCompleted)
            {
                _joinableTask?.JoinAsync(cancellationToken).Forget();
            }

            return _value.WithCancellation(cancellationToken);
        }

        public T GetValue() => GetValue(CancellationToken.None);

        public T GetValue(CancellationToken cancellationToken)
        {
            if (IsValueFactoryCompleted)
                return _value.GetAwaiter().GetResult();
            var factory = _jobFactory;
            return factory != null
                ? factory.Run(() => GetValueAsync(cancellationToken))
                : GetValueAsync(cancellationToken).GetAwaiter().GetResult();
        }


        public override string ToString()
        {
            return (_value != null && _value.IsCompleted)
                ? (_value.Status == TaskStatus.RanToCompletion ? _value.Result.ToString() : "Lazy Faulted")
                : "Lazy not Created";
        }
    }
}
