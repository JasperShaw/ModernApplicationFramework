using System;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Menu controller command bar item definition that contains a <see cref="CommandBarDataSource" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class MenuControllerDataSource<T> : MenuControllerDataSource where T : CommandDefinitionBase
	{
	    private string _text;
	    public override CommandDefinitionBase CommandDefinition { get; }

	    public override string Text
	    {
	        get => _text;
	        set
	        {
	            if (value == _text)
                    return;
	            IsTextModified = true;
	            _text = value;
	            OnPropertyChanged();
	        }
	    }

	    public override void Reset()
	    {
	        IsTextModified = false;
	        _text = OriginalText;
	        UpdateInternalName();
	        OnPropertyChanged(nameof(Text));
	        Flags.EnableStyleFlags((CommandBarFlags)OriginalFlagStore.AllFlags);
        }

        public override Guid Id { get; }

	    public MenuControllerDataSource(Guid id, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true, CommandBarFlags flags = CommandBarFlags.CommandFlagTextIsAnchor)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable, flags)
	    {
	        Id = id;

            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            _text = CommandDefinition.Text;
            Name = CommandDefinition.Name;

            if (CommandDefinition is CommandMenuControllerDefinition menuControllerDefinition)
                AnchorItem = menuControllerDefinition.Items.FirstOrDefault();
        }
    }

    public abstract class MenuControllerDataSource : CommandBarItemDataSource
    {
        private CommandBarItemDataSource _anchorItem;

        /// <summary>
        /// The currently anchored command bar item
        /// </summary>
        public CommandBarItemDataSource AnchorItem
        {
            get => _anchorItem;
            set
            {
                if (Equals(value, _anchorItem)) return;
                _anchorItem = value;
                OnPropertyChanged();
            }
        }

        protected MenuControllerDataSource(string text, uint sortOrder, CommandBarGroupDefinition group, CommandDefinitionBase definition, bool visible, bool isChecked, bool isCustom, bool isCustomizable, CommandBarFlags flags)
            : base(text, sortOrder, group, definition, visible, isChecked, isCustom, isCustomizable, flags)
        {
        }
    }
}