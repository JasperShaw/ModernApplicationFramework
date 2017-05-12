using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarDefinitionBase : IHasTextProperty
    {
        private uint _sortOrder;
        private string _text;
        private bool _isChecked;
        private FlagStorage _flagStorage;
	    private string _name;
        private bool _acquireFocus;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual FlagStorage Flags => _flagStorage ?? (_flagStorage = new FlagStorage());

        public virtual bool IsCustom { get; }
        public virtual bool IsCustomizable { get; }

        public virtual DefinitionBase CommandDefinition { get; }

        public IList<CommandBarGroupDefinition> ContainedGroups { get; }

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

        public virtual string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

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

        protected CommandBarDefinitionBase(string text, uint sortOrder, DefinitionBase definition, bool isCustom,
            bool isCustomizable, bool isChecked)
        {
            _sortOrder = sortOrder;
            _text = text;
	        _name = text;
            CommandDefinition = definition;
            IsCustom = isCustom;
            _isChecked = isChecked;
            IsCustomizable = isCustomizable;
            ContainedGroups = new List<CommandBarGroupDefinition>();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}