using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ICommandListHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandListDefinition
    {
        void Populate(Input.Command.Command command, List<CommandItemDefinitionBase> commands);
    }
}
