using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface IWaitIndicator
    {
        WaitIndicatorResult Wait(string title, string message, bool allowCancel, Action<IWaitContext> action);

        IWaitContext StartWait(string title, string message, bool allowCancel);
    }
}