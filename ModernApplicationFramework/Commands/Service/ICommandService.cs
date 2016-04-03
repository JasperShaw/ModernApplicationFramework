using System;

namespace ModernApplicationFramework.Commands.Service
{
    public interface ICommandService
    {
        CommandDefinition GetCommandDefinition(Type commandDefinitionType);
    }
}
