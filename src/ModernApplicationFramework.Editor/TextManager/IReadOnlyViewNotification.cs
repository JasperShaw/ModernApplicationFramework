using System;

namespace ModernApplicationFramework.Editor.TextManager
{
    internal interface IReadOnlyViewNotification
    {
        int OnDisabledEditingCommand(ref Guid commandGroup, uint commandId);
    }
}