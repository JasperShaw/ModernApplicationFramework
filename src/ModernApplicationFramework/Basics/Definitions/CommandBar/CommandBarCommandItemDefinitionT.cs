using System;
using ModernApplicationFramework.Basics.Definitions.Command;

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
        private bool _bindItemVisibilityToCommandExecution;

        /// <summary>
        /// When set to <see langword="true"/> the command bar item will become invisible when the command can not be executed
        /// </summary>
        public bool BindItemVisibilityToCommandExecution
        {
            get => _bindItemVisibilityToCommandExecution;
            set
            {
                if (value == _bindItemVisibilityToCommandExecution)
                    return;
                _bindItemVisibilityToCommandExecution = value;
                OnPropertyChanged();
            }
        }

	    public override Guid Id { get; }

        public CommandBarCommandItemDefinition(Guid id, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false,
            bool bindItemVisibilityToCommandExecution = false, bool isCustomizable = true, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
            : base(null, sortOrder, group, isVisible, isChecked, isCustom, isCustomizable, flags)
        {
            Id = id;
            BindItemVisibilityToCommandExecution = bindItemVisibilityToCommandExecution;
        }


	    protected override void OnCommandChanged(object sender, EventArgs e)
	    {
	        base.OnCommandChanged(sender, e);
	        if (BindItemVisibilityToCommandExecution && !InternalCommandDefinition.Command.Enabled)
	            IsVisible = false;
	    }
	}
}