using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [ExportImplementation(typeof(IUiThreadOperationExecutor))]
    [Name("default")]
    internal class DefaultUiThreadOperationExecutor : IUiThreadOperationExecutor
    {
        public IUiThreadOperationContext BeginExecute(string title, string defaultDescription, bool allowCancellation, bool showProgress)
        {
            return new DefaultUiThreadOperationContext(allowCancellation, defaultDescription);
        }

        public UiThreadOperationStatus Execute(string title, string defaultDescription, bool allowCancellation, bool showProgress, Action<IUiThreadOperationContext> action)
        {
            var operationContext = new DefaultUiThreadOperationContext(allowCancellation, defaultDescription);
            action(operationContext);
            return UiThreadOperationStatus.Completed;
        }
    }
}