using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Fundamental command bar item definition that contains a <see cref="CommandBarDefinitionBase"/>
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public abstract class CommandBarItemDefinition<T> : CommandBarItemDefinition where T : CommandDefinitionBase
	{
		private string _text;
		private string _name;

        /// <inheritdoc />
        /// <summary>
        /// The command definition of the element
        /// </summary>
        public sealed override CommandDefinitionBase CommandDefinition { get; }

        /// <summary>
        /// The localized definition's text
        /// </summary>
        /// <inheritdoc />
        public sealed override string Text
		{
			get => _text;
			set
			{
				if (value == _text) return;
				_text = value;
				OnPropertyChanged();
			}
		}

        /// <inheritdoc />
        /// <summary>
        /// The name of the definition
        /// </summary>
        public sealed override string Name
		{
			get => _name;
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged();
			}
		}

		protected CommandBarItemDefinition(string text, uint sortOrder, CommandBarGroupDefinition @group, CommandDefinitionBase definition, 
			bool visible, bool isChecked, bool isCustom, bool isCustomizable) 
			: base(text, sortOrder, @group, definition, visible, isChecked, isCustom, isCustomizable)
		{
			CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
			Text = CommandDefinition.Text;
			Name = CommandDefinition.Name;
		}
	}
}
