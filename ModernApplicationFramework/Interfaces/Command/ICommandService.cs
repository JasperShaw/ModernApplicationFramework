using System;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandService
    {
        CommandDefinition GetCommandDefinition(Type definitionType);
    }
}