using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Fundamental command bar element definition
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasTextProperty" />
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public abstract class CommandBarDefinitionBase : IHasTextProperty
    {
        private uint _sortOrder;
        private string _text;
        private bool _isChecked;
        private FlagStorage _flagStorage;
	    private string _name;
        private bool _acquireFocus;
        private bool _isEnabled = true;

        protected string OriginalText { get; set; }
        private FlagStorage _originalFlagStore;

        protected internal FlagStorage OriginalFlagStore =>
            _originalFlagStore ?? (_originalFlagStore = new FlagStorage());

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CommandBarItemDefinition> Items { get; set; }
        
        /// <summary>
        /// The <see cref="FlagStorage"/> of this definition
        /// </summary>
        public FlagStorage Flags => _flagStorage ?? (_flagStorage = new FlagStorage());

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
        public IList<CommandBarGroupDefinition> ContainedGroups { get; }

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

        protected CommandBarDefinitionBase(string text, uint sortOrder, CommandDefinitionBase definition, bool isCustom,
            bool isCustomizable, bool isChecked, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
        {
            _sortOrder = sortOrder;
            _text = text;
            OriginalText = text;
	        _name = text;
            CommandDefinition = definition;
            IsCustom = isCustom;
            _isChecked = isChecked;
            IsCustomizable = isCustomizable;
            ContainedGroups = new List<CommandBarGroupDefinition>();
            Flags.EnableStyleFlags(flags);
            OriginalFlagStore.EnableStyleFlags(flags);
        }

        public virtual void Reset()
        {
            IsTextModified = false;
            _text = OriginalText;
            OnPropertyChanged(nameof(Text));
            Flags.EnableStyleFlags((CommandBarFlags)OriginalFlagStore.AllFlags);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}