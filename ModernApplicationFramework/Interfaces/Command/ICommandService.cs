using System;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandService
    {
        DefinitionBase GetCommandDefinition(Type definitionType);
    }
}