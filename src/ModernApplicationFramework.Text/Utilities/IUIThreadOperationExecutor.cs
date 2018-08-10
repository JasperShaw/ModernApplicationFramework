using System;

namespace ModernApplicationFramework.Text.Utilities
{
    public interface IUiThreadOperationExecutor
    {
        IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation,
            bool showProgress);

        UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation,
            bool showProgress, Action<IUiThreadOperationContext> action);
    }
}