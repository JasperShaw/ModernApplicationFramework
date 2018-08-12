using System;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Editor
{
    public interface IWaitIndicator
    {
        WaitIndicatorResult Wait(string title, string message, bool allowCancel, Action<IWaitContext> action);

        IWaitContext StartWait(string title, string message, bool allowCancel);
    }
}