using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple command bar item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public class ButtonDataSource : CommandBarItemDataSource
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

        public ButtonDataSource(Guid id, uint sortOrder, CommandDefinitionBase commandDefinition, bool isCustom = false)
            : base(null, sortOrder, null, commandDefinition, isCustom)
        {
            Id = id;
            Text = CommandDefinition?.Text;
            Name = CommandDefinition?.Name;
        }

        protected override void OnCommandChanged(object sender, EventArgs e)
        {
            base.OnCommandChanged(sender, e);
            IsChecked = InternalCommandDefinition.Command.Checked;
        }
    }
}