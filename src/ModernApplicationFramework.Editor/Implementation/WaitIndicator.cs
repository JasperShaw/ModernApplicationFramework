using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IWaitIndicator))]
    internal sealed class WaitIndicator : IWaitIndicator
    {
        private readonly IWaitDialogFactory _serviceProvider;

        [ImportingConstructor]
        public WaitIndicator(IWaitDialogFactory serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public WaitIndicatorResult Wait(string title, string message, bool allowCancel, Action<IWaitContext> action)
        {
            using (var studioWaitContext = StartWait(title, message, allowCancel))
            {
                try
                {
                    action(studioWaitContext);
                    return WaitIndicatorResult.Completed;
                }
                catch (OperationCanceledException)
                {
                    return WaitIndicatorResult.Canceled;
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions[0] is OperationCanceledException)
                        return WaitIndicatorResult.Canceled;
                    throw;
                }
            }
        }

        private WaitContext StartWait(string title, string message, bool allowCancel)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("Cannot get IVsThreadedWaitDialogFactory");
            return new WaitContext(_serviceProvider, title, message, allowCancel);
        }

        IWaitContext IWaitIndicator.StartWait(string title, string message, bool allowCancel)
        {
            return StartWait(title, message, allowCancel);
        }
    }
}