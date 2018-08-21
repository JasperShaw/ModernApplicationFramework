using System;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Base;

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

                    var commandWrapper = commandDefinition.Command as AbstractCommandWrapper;
                    var routedCommand = commandDefinition.Command as RoutedCommand;

                    if (commandWrapper != null || routedCommand != null)
                    {
                        if (commandWrapper != null)
                            commandWrapper.CanExecuteChanged += CommandWrapper_CanExecuteChanged;
                        if (routedCommand != null)
                            routedCommand.CanExecuteChanged += CommandWrapper_CanExecuteChanged;

                    }
                }
                else
                {
                    if (!(CommandDefinition is CommandDefinition commandDefinition))
                        return;
                    var commandWrapper = commandDefinition.Command as AbstractCommandWrapper;
                    var routedCommand = commandDefinition.Command as RoutedCommand;

                    if (commandWrapper != null || routedCommand != null)
                    {
                        if (commandWrapper != null)
                            commandWrapper.CanExecuteChanged -= CommandWrapper_CanExecuteChanged;
                        if (routedCommand != null)
                            routedCommand.CanExecuteChanged -= CommandWrapper_CanExecuteChanged;

                    }
                }
            }
        }

	    public override Guid Id { get; }

        public CommandBarCommandItemDefinition(Guid id, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false,
            bool registerVisibilityToCommand = false, bool isCustomizable = true, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
            : base(null, sortOrder, group, isVisible, isChecked, isCustom, isCustomizable, flags)
        {
            Id = id;
            //RegisterVisibilityToCommand = registerVisibilityToCommand;

            //this.OnCommandChanged();

        }

	    private void CommandWrapper_CanExecuteChanged(object sender, EventArgs e)
        {
            if (!(CommandDefinition is CommandDefinition cd))
                return;
            IsVisible = cd.Command.CanExecute(null);
        }
	}
}