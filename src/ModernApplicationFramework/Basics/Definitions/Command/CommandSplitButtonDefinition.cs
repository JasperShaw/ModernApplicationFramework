using Caliburn.Micro;
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

        /// <summary>
        /// The collection of all items of the drop down list
        /// </summary>
        public abstract IObservableCollection<IHasTextProperty> Items { get; set; }

        public abstract IStatusStringCreator StatusStringCreator { get; }
    }

    public abstract class CommandSplitButtonDefinition<T> : CommandSplitButtonDefinition where T : ICommandDefinitionCommand
    {
        protected CommandSplitButtonDefinition()
        {
            Command = IoC.Get<T>();
        }
    }
}
