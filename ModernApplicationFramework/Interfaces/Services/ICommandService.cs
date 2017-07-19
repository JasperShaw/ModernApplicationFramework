using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandService
    {
        DefinitionBase GetCommandDefinition(Type definitionType);
    }
}