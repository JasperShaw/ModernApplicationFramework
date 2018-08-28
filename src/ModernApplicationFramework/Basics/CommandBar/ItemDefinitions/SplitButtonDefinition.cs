using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Models;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.CommandBar.ItemDefinitions
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="CommandDefinition"/> for split button commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.ItemDefinitions.CommandDefinition" />
    public abstract class SplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        public abstract SplitButtonModel Model { get; }

        protected SplitButtonDefinition(ICommandDefinitionCommand command) : base(command)
        {
        }
    }

    public abstract class SplitButtonDefinition<T> : SplitButtonDefinition where T : ICommandDefinitionCommand
    {
        protected SplitButtonDefinition() : base(IoC.Get<T>())
        {
        }
    }
}
