using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandListHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : CommandListDefinition
    {
        void Populate(CommandBase.Command command, List<DefinitionBase> commands);
    }
}
