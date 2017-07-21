using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Menu controller command bar item definition that contains a <see cref="CommandBarDefinitionBase" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarMenuControllerDefinition<T> : CommandBarItemDefinition<T> where T : CommandDefinitionBase
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

        public CommandBarMenuControllerDefinition(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            Flags.TextIsAnchor = true;

            if (CommandDefinition is CommandMenuControllerDefinition menuControllerDefinition)
                AnchorItem = menuControllerDefinition.Items.FirstOrDefault();
        }
    }
}