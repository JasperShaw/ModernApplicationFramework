using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchControlDataSource : INotifyPropertyChanged
    {
        private string _searchText;
        private SearchStatus _seachStatus;
        private int _searchProgress;
        private int _searchResultsCount;
        private SearchSettingsDataSource _searchSettings;
        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public SearchStatus SeachStatus
        {
            get => _seachStatus;
            set
            {
                if (value == _seachStatus) return;
                _seachStatus = value;
                OnPropertyChanged();
            }
        }

        public int SearchProgress
        {
            get => _searchProgress;
            set
            {
                if (value == _searchProgress) return;
                _searchProgress = value;
                OnPropertyChanged();
            }
        }

        public int SearchResultsCount
        {
            get => _searchResultsCount;
            set
            {
                if (value == _searchResultsCount) return;
                _searchResultsCount = value;
                OnPropertyChanged();
            }
        }

        public SearchSettingsDataSource SearchSettings
        {
            get => _searchSettings;
            set
            {
                if (Equals(value, _searchSettings)) return;
                _searchSettings = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
