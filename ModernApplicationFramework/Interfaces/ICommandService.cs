using System;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces
{
    public interface ICommandService
    {
        CommandDefinition GetCommandDefinition(Type commandDefinitionType);
    }
}