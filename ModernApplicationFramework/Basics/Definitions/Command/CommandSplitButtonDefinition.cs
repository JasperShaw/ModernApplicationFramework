using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="CommandDefinition"/> for split button models
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" />
    public abstract class CommandSplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        /// <summary>
        /// The collection of all items of the drop down list
        /// </summary>
        public abstract IObservableCollection<IHasTextProperty> Items { get; set; }

        /// <summary>
        /// The action performed after selecting items in the drop down list 
        /// </summary>
        /// <param name="count">The amount of items that have been selected</param>
        public abstract void Execute(int count);
    }
}
