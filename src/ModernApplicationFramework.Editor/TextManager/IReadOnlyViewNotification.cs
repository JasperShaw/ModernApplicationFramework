using System;

namespace ModernApplicationFramework.Editor.TextManager
{
    internal interface IReadOnlyViewNotification
    {
        int OnDisabledEditingCommand(Guid commandGroup, uint commandId);
    }
}