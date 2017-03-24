using System;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandService
    {
        DefinitionBase GetCommandDefinition(Type definitionType);
    }
}