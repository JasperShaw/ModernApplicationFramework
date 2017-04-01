using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarDefinitionBase : INotifyPropertyChanged
    {
        private uint _sortOrder;
        private string _text;
        private bool _isChecked;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool IsCustom { get; }

        public virtual DefinitionBase CommandDefinition { get; }

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

        protected CommandBarDefinitionBase(string text, uint sortOrder, DefinitionBase definition, bool isCustom, bool isChecked)
        {
            _sortOrder = sortOrder;
            _text = text;
            CommandDefinition = definition;
            IsCustom = isCustom;
            _isChecked = isChecked;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}