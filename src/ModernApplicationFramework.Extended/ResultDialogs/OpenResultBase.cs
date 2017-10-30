using System;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.ResultDialogs
{
    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        protected Action<TTarget> SetData;
        protected Action<TTarget> OnConfigure;
        protected Action<TTarget> OnShutDown;

        public event EventHandler<ResultCompletionEventArgs> Completed;

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

        public abstract void Execute(CoroutineExecutionContext context);

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            Completed?.Invoke(this, new ResultCompletionEventArgs {Error = exception, WasCancelled = wasCancelled});
        }
    }
}