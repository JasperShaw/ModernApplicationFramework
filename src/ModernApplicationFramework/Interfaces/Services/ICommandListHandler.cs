using System.Collections.Generic;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandListHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : ListCommandDefinition
    {
        void Populate(Input.Command.Command command, List<CommandItemDefinitionBase> commands);
    }
}
