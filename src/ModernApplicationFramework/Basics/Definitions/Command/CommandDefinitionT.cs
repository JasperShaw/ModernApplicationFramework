using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandDefinition<T> : CommandDefinition where T : ICommandDefinitionCommand
    {
        protected CommandDefinition()
        {
            Command = IoC.Get<T>();
        }
    }
}
