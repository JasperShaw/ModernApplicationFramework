using System;
using ModernApplicationFramework.Basics.Definitions;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandService
    {
        DefinitionBase GetCommandDefinition(Type definitionType);
    }
}