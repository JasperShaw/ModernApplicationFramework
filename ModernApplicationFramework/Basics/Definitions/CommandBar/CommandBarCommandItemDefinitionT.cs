using System;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple command bar item definition that contains a <see cref="CommandBarDefinitionBase" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarCommandItemDefinition<T> : CommandBarItemDefinition<T> where T : CommandDefinitionBase
	{
        private bool _registerVisibilityToCommand;

        /// <summary>
        /// When set to <see langword="true"/> the command bar item will become invisible when the command can not be executed
        /// </summary>
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