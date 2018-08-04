using System;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    public interface IUiThreadOperationExecutor
    {
        UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation, bool showProgress, Action<IUiThreadOperationContext> action);

        IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation, bool showProgress);
    }   
}