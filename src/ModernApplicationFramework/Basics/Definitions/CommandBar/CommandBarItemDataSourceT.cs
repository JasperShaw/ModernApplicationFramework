using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Fundamental command bar item definition that contains a <see cref="CommandBarDataSource"/>
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public abstract class CommandBarItemDataSource<T> : CommandBarItemDataSource where T : CommandDefinitionBase
	{
		private string _text;
		private string _name;

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
			    IsTextModified = true;
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

		protected CommandBarItemDataSource(string text, uint sortOrder, CommandBarGroupDefinition group, 
			bool visible, bool isChecked, bool isCustom, bool isCustomizable, CommandBarFlags flags) 
			: base(text, sortOrder, group, IoC.Get<ICommandService>().GetCommandDefinition(typeof(T)), visible, isChecked, isCustom, isCustomizable, flags)
		{
		    var def = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            OriginalText = def.Text;
            _text = def.Text;
			_name = def.Name;
		}

	    public override void Reset()
	    {
	        IsTextModified = false;
	        _text = OriginalText;
	        UpdateInternalName();
	        OnPropertyChanged(nameof(Text));
	        Flags.EnableStyleFlags((CommandBarFlags)OriginalFlagStore.AllFlags);
        }
    }
}
