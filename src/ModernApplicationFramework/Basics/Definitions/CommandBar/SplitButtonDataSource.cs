using System;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Command bar split button definition that contains a <see cref="CommandBarDataSource"/>
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class SplitButtonDataSource<T> : SplitButtonDataSource where T : CommandDefinitionBase
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


        public SplitButtonDataSource(Guid id, string statusString, CommandBarGroup group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : this(id, group, sortOrder, isVisible, isChecked, isCustom, isCustomizable)
        {
            StatusString = statusString;
        }

        public SplitButtonDataSource(Guid id, CommandBarGroup group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(id, null, sortOrder, group, null, isCustom)
        {

            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            _text = CommandDefinition.Text;

            if (CommandDefinition is CommandSplitButtonDefinition splitButtonDefinition)
                StringCreator = splitButtonDefinition.StatusStringCreator;

            StatusString = StringCreator?.CreateDefaultMessage();
        }
	}

    public class SplitButtonDataSource : CommandBarItemDataSource
    {
        private int _selectedIndex;
        private string _statusString;

        /// <summary>
        /// The currently selected item index
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                OnPropertyChanged();
                if (StringCreator != null)
                    StatusString = StringCreator.CreateMessage(value + 1);
            }
        }

        /// <summary>
        /// The message of the status bar of a split button
        /// </summary>
        public string StatusString
        {
            get => _statusString;
            set
            {
                if (value == _statusString) return;
                _statusString = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="IStatusStringCreator"/> that creates an localized status message
        /// </summary>
        public IStatusStringCreator StringCreator { get; set; }


        internal SplitButtonDataSource(Guid id, string text, uint sortOrder, CommandBarGroup group, CommandDefinitionBase definition, 
            bool isCustom = false) 
            : base(text, sortOrder, group, definition, isCustom, false)
        {
            Id = id;

            if (definition is CommandSplitButtonDefinition splitButtonDefinition)
                StringCreator = splitButtonDefinition.StatusStringCreator;

            StatusString = StringCreator?.CreateDefaultMessage();
        }

        public override Guid Id { get; }
    }
}