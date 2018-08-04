using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface IReadOnlyViewNotification
    {
        int OnDisabledEditingCommand(ref Guid commandGroup, uint commandId);
    }
}