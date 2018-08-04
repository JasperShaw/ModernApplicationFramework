using System;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Commanding
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