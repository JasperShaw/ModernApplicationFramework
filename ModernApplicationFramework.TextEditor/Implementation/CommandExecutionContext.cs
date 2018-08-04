using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public sealed class CommandExecutionContext
    {
        public CommandExecutionContext(IUiThreadOperationContext operationContext)
        {
            OperationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
        }


        public IUiThreadOperationContext OperationContext { get; }
    }
}