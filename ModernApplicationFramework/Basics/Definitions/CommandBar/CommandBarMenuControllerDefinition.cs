using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarMenuControllerDefinition<T> : CommandBarItemDefinition<T> where T : DefinitionBase
	{
        private CommandBarItemDefinition _anchorItem;

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