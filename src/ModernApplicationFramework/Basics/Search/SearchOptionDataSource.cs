using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Basics.Search
{
    public class SearchOptionDataSource : INotifyPropertyChanged
    {
        private SearchOptionType _type;
        private string _displayText;
        private string _tooltip;
        private bool _value;
        public event PropertyChangedEventHandler PropertyChanged;

        public SearchOptionType Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (value == _displayText) return;
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public string Tooltip
        {
            get => _tooltip;
            set
            {
                if (value == _tooltip) return;
                _tooltip = value;
                OnPropertyChanged();
            }
        }

        public bool Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        internal static void Select(SearchOptionDataSource option)
        {
            option.OnSelect();
        }

        protected virtual void OnSelect()
        {
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
