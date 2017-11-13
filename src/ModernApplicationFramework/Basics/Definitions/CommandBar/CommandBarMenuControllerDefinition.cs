using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Menu controller command bar item definition that contains a <see cref="CommandBarDefinitionBase" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarMenuControllerDefinition<T> : CommandBarMenuControllerDefinition where T : CommandDefinitionBase
	{
	    public override CommandDefinitionBase CommandDefinition { get; }

        public CommandBarMenuControllerDefinition(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            Flags.TextIsAnchor = true;

            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            Name = CommandDefinition.Name;

            if (CommandDefinition is CommandMenuControllerDefinition menuControllerDefinition)
                AnchorItem = menuControllerDefinition.Items.FirstOrDefault();
        }
    }

    public abstract class CommandBarMenuControllerDefinition : CommandBarItemDefinition
    {
        private CommandBarItemDefinition _anchorItem;

        /// <summary>
        /// The currently anchored command bar item
        /// </summary>
        public CommandBarItemDefinition AnchorItem
        {
            get => _anchorItem;
            set
            {
                if (Equals(value, _anchorItem)) return;
                _anchorItem = value;
                OnPropertyChanged();
            }
        }

        protected CommandBarMenuControllerDefinition(string text, uint sortOrder, CommandBarGroupDefinition @group, CommandDefinitionBase definition, bool visible, bool isChecked, bool isCustom, bool isCustomizable) : base(text, sortOrder, @group, definition, visible, isChecked, isCustom, isCustomizable)
        {
        }
    }
}