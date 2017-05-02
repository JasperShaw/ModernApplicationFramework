using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarCommandItemDefinition<T> : CommandBarItemDefinition<T> where T : DefinitionBase
	{
        private bool _registerVisibilityToCommand;


        public bool RegisterVisibilityToCommand
        {
            get => _registerVisibilityToCommand;
            set
            {
                if (value == _registerVisibilityToCommand)
                    return;
                _registerVisibilityToCommand = value;
                OnPropertyChanged();
                if (value)
                {
                    if (!(CommandDefinition is CommandDefinition commandDefinition))
                        return;
                    var commandWrapper = commandDefinition.Command as CommandWrapper;
                    if (commandWrapper != null)
                        commandWrapper.CanExecuteChanged += CommandWrapper_CanExecuteChanged;
                }
                else
                {
                    if (!(CommandDefinition is CommandDefinition commandDefinition))
                        return;
                    var commandWrapper = commandDefinition.Command as CommandWrapper;
                    if (commandWrapper != null)
                        commandWrapper.CanExecuteChanged -= CommandWrapper_CanExecuteChanged;
                }
            }
        }

        public CommandBarCommandItemDefinition(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false,
            bool registerVisibilityToCommand = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            RegisterVisibilityToCommand = registerVisibilityToCommand;
        }

        private void CommandWrapper_CanExecuteChanged(object sender, EventArgs e)
        {
            var cd = CommandDefinition as CommandDefinition;
            if (cd == null)
                return;
            IsVisible = cd.Command.CanExecute(null);
        }
    }
}