using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IUiThreadOperationExecutor
    {
        UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation, bool showProgress, Action<IUiThreadOperationContext> action);

        IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation, bool showProgress);
    }   
}