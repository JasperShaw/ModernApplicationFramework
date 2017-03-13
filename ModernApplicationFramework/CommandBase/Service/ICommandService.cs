using System;

namespace ModernApplicationFramework.CommandBase.Service
{
    public interface ICommandService
    {
        CommandDefinition GetCommandDefinition(Type commandDefinitionType);
    }
}