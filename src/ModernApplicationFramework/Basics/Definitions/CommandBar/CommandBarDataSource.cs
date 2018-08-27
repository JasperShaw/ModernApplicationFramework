using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc cref="DisposableObject" />
    /// <summary>
    /// Fundamental command bar element definition
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasTextProperty" />
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public abstract class CommandBarDataSource : DisposableObject, IHasTextProperty, IHasInternalName
    {
        private uint _sortOrder;
        private string _text;
        private bool _isChecked;
        private FlagsDataSource _flagsDataSource;
	    private string _name;
        private bool _acquireFocus;
        private bool _isEnabled = true;

        protected string OriginalText { get; set; }
        private FlagsDataSource _originalFlagStore;
        private bool _isVisible;
        private string _internalName;

        protected internal FlagsDataSource OriginalFlagStore =>
            _originalFlagStore ?? (_originalFlagStore = new FlagsDataSource());

        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// The <see cref="FlagsDataSource"/> of this definition
        /// </summary>
        public FlagsDataSource Flags => _flagsDataSource ?? (_flagsDataSource = new FlagsDataSource());

        /// <summary>
        /// Indicates whether this definition was created by the application's user
        /// </summary>
        public virtual bool IsCustom { get; }

        /// <summary>
        /// Indicates whether this definition can be modified
        /// </summary>
        public virtual bool IsCustomizable { get; }

        /// <summary>
        /// The command definition of the element
        /// </summary>
        public virtual CommandDefinitionBase CommandDefinition { get; }

        /// <summary>
        /// The groups that are hosted by the element
        /// </summary>
        public IList<CommandBarGroup> ContainedGroups { get; }

        /// <summary>
        /// The sorting order of the definition
        /// </summary>
        public virtual uint SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The localized definition's text
        /// </summary>
        public virtual string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                    return;
                _text = value;
                IsTextModified = true;
                OnPropertyChanged();
            }
        }

        public virtual CommandControlTypes UiType { get; } = CommandControlTypes.Button;

        /// <summary>
        /// The name of the definition
        /// </summary>
        public virtual string Name
	    {
		    get => _name;
		    set
		    {
			    if (value == _name) return;
			    _name = value;
			    OnPropertyChanged();
		    }
	    }

        /// <summary>
        /// Indicates whether this element's state is checked or not
        /// </summary>
        public virtual bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }


        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public virtual bool AcquireFocus
        {
            get => _acquireFocus;
            set
            {
                if (value == _acquireFocus)
                    return;
                _acquireFocus = value;
                OnPropertyChanged();
            }
        }

        public virtual bool IsTextModified { get; protected set; }

        public abstract Guid Id { get; }

        protected CommandBarDataSource(string text, uint sortOrder, CommandDefinitionBase definition, bool isCustom, 
            bool isChecked, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
        {
            _sortOrder = sortOrder;
            _text = text ?? definition?.Text;
            OriginalText = text ?? definition?.Text;
	        _name = text ?? definition?.Text;
            _internalName = new AccessKeyRemovingConverter().Convert(text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();
            CommandDefinition = definition;
            IsCustom = isCustom;
            _isChecked = isChecked;
            _isVisible = !flags.HasFlag(CommandBarFlags.CommandDefaultInvisible);
            IsCustomizable = !flags.HasFlag(CommandBarFlags.CommandNoCustomize);
            ContainedGroups = new List<CommandBarGroup>();
            Flags.EnableStyleFlags(flags);
            OriginalFlagStore.EnableStyleFlags(flags);
        }

        internal static CommandBarDataSource CreateInstance<T>(T itemDefinition) where T : CommandDefinitionBase
        {
            switch (itemDefinition.ControlType)
            {
                case CommandControlTypes.Button:
                    return new ButtonDataSource(Guid.Empty, 0, itemDefinition);
                case CommandControlTypes.Separator:
                    return SeparatorDataSource.NewInstance;
                case CommandControlTypes.SplitDropDown:
                    return new SplitButtonDataSource(Guid.Empty, itemDefinition.Text, 0, null, itemDefinition as CommandSplitButtonDefinition, false);
                case CommandControlTypes.Combobox:
                    return new ComboBoxDataSource(Guid.Empty, itemDefinition.Text, 0, null, itemDefinition as CommandComboBoxDefinition, false);
                case CommandControlTypes.Menu:
                    break;
                case CommandControlTypes.MenuController:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new ArgumentException();
        }

        internal CommandDefinition InternalCommandDefinition { get; set; }

        public virtual void Reset()
        {
            IsTextModified = false;
            _text = OriginalText;
            OnPropertyChanged(nameof(Text));
            Flags.EnableStyleFlags((CommandBarFlags)OriginalFlagStore.AllFlags);
        }

        protected virtual void UpdateText()
        {
            Text = CommandDefinition?.Text;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        public virtual bool InheritInternalName => true;
    }
}