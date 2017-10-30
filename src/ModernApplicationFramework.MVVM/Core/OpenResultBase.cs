using System;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        protected Action<TTarget> SetData;
        protected Action<TTarget> OnConfigure;
        protected Action<TTarget> OnShutDown;

        Action<TTarget> IOpenResult<TTarget>.OnConfigure
        {
            get => OnConfigure;
            set => OnConfigure = value;
        }

        Action<TTarget> IOpenResult<TTarget>.OnShutDown
        {
            get => OnShutDown;
            set => OnShutDown = value;
        }

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            Completed?.Invoke(this, new ResultCompletionEventArgs { Error = exception, WasCancelled = wasCancelled });
        }

        public abstract void Execute(CoroutineExecutionContext context);

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
