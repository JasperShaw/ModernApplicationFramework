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

        protected CommandBarDefinitionBase(string text, uint sortOrder, DefinitionBase definition, bool isCustom)
        {
            _sortOrder = sortOrder;
            _text = text;
            CommandDefinition = definition;
            IsCustom = isCustom;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}