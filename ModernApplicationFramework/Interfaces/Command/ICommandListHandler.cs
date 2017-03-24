using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandListHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandListDefinition
    {
        void Populate(CommandBase.Command command, List<DefinitionBase> commands);
    }
}
