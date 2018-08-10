using System;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public sealed class CommandExecutionContext
    {
        public IUiThreadOperationContext OperationContext { get; }

        public CommandExecutionContext(IUiThreadOperationContext operationContext)
        {
            OperationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
        }
    }
}