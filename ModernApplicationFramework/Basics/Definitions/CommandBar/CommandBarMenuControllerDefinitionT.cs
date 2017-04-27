using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarMenuControllerDefinitionT<T> : CommandBarItemDefinition where T : DefinitionBase
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

        public override DefinitionBase CommandDefinition { get; }


        public CommandBarMenuControllerDefinitionT(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;

            if (CommandDefinition is CommandMenuControllerDefinition menuControllerDefinition)
                AnchorItem = menuControllerDefinition.Items[0];
        }
    }
}