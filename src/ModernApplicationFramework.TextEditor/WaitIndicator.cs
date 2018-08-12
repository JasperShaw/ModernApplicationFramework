namespace ModernApplicationFramework.Editor
{
    //TODO: Implement

    //[Export(typeof(IWaitIndicator))]
    //internal sealed class WaitIndicator : IWaitIndicator
    //{
    //    [ImportingConstructor]
    //    public WaitIndicator(IWaitDialogFactory serviceProvider)
    //    {
    //        this.serviceProvider = serviceProvider;
    //    }

    //    public WaitIndicatorResult Wait(string title, string message, bool allowCancel, Action<IWaitContext> action)
    //    {
    //        using (WaitContext studioWaitContext = StartWait(title, message, allowCancel))
    //        {
    //            try
    //            {
    //                action(studioWaitContext);
    //                return WaitIndicatorResult.Completed;
    //            }
    //            catch (OperationCanceledException)
    //            {
    //                return WaitIndicatorResult.Canceled;
    //            }
    //            catch (AggregateException ex)
    //            {
    //                if (ex.InnerExceptions[0] is OperationCanceledException)
    //                    return WaitIndicatorResult.Canceled;
    //                throw;
    //            }
    //        }
    //    }

    //    private WaitContext StartWait(string title, string message, bool allowCancel)
    //    {
    //        var service = GetService(typeof(IWaitDialogFactory));
    //        if (service == null)
    //            throw new InvalidOperationException("Cannot get IVsThreadedWaitDialogFactory");
    //        return new WaitContext(service, title, message, allowCancel);
    //    }

    //    IWaitContext IWaitIndicator.StartWait(string title, string message, bool allowCancel)
    //    {
    //        return StartWait(title, message, allowCancel);
    //    }
    //}
}