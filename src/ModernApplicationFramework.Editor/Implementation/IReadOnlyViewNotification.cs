using System;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal interface IReadOnlyViewNotification
    {
        int OnDisabledEditingCommand(ref Guid commandGroup, uint commandId);
    }
}