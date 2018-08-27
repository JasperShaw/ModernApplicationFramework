using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple command bar item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    internal class ButtonDataSource : CommandBarItemDataSource
    {
        private bool _isChecked;

        public override Guid Id { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool Checkable { get; }

        public ButtonDataSource(Guid id, string text, uint sortOrder, CommandBarGroup group, 
            CommandItemDefinitionBase itemDefinition, bool isCustom, bool isChecked, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
            : base(text, sortOrder, group, itemDefinition, isCustom, flags)
        {
            Id = id;

            _isChecked = isChecked;

            Checkable = itemDefinition.Checkable;

            if (itemDefinition is CommandDefinition commandDefinition)
            {
                InternalCommandDefinition = commandDefinition;
                commandDefinition.Command.CommandChanged += OnCommandChanged;
            }
        }

        protected void OnCommandChanged(object sender, EventArgs e)
        {
            IsEnabled = InternalCommandDefinition.Command.Enabled;
            if (!Flags.AllFlags.HasFlag(CommandBarFlags.CommandDefaultInvisible))
                IsVisible = InternalCommandDefinition.Command.Visible;
            IsChecked = InternalCommandDefinition.Command.Checked;
            if (Flags.AllFlags.HasFlag(CommandBarFlags.CommandDynamicVisibility) && !IsEnabled)
                IsVisible = false;     
        }
    }
}