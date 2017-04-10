using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.Menu.MenuItems
{
    public sealed class CommandMenuItemDefinition<T> : MenuItemDefinition where T : DefinitionBase
    {
        private bool _registerVisibilityToCommand;
        public override DefinitionBase CommandDefinition { get; }

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

        public CommandMenuItemDefinition(MenuItemGroupDefinition group, uint sortOrder, bool registerVisibilityToCommand = false)
            : base(null, sortOrder, group, null, true, false, false)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            DisplayName = CommandDefinition.Text;
            RegisterVisibilityToCommand = registerVisibilityToCommand;
        }

        private void CommandWrapper_CanExecuteChanged(object sender, System.EventArgs e)
        {
            var cd = CommandDefinition as CommandDefinition;
            if (cd == null)
                return;
            IsVisible = cd.Command.CanExecute(null);
        }
    }
}