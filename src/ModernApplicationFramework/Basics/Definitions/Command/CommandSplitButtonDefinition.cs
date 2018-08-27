using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="CommandDefinition"/> for split button commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" />
    public abstract class CommandSplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        public abstract SplitButtonModel Model { get; }

        protected CommandSplitButtonDefinition(ICommandDefinitionCommand command)
        {
            Command = command;
        }
    }

    public abstract class CommandSplitButtonDefinition<T> : CommandSplitButtonDefinition where T : ICommandDefinitionCommand
    {
        protected CommandSplitButtonDefinition() : base(IoC.Get<T>())
        {
        }
    }
}
