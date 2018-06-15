﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchControlDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _searchText = string.Empty;
        private SearchStatus _searchStatus = SearchStatus.NotStarted;
        private int _searchProgress;
        private int _searchResultsCount = -1;
        private SearchSettingsDataSource _searchSettings = new SearchSettingsDataSource();
        
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

        public SearchStatus SearchStatus
        {
            get => _searchStatus;
            set
            {
                if (value == _searchStatus) return;
                _searchStatus = value;
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

        protected virtual void OnStartSearch(string searchText)
        {
        }

        protected virtual void OnStopSearch()
        {
        }

        protected virtual void OnClearSearch()
        {
        }

        protected virtual bool OnNotifyNavigationKey(SearchNavigationKeys searchNavigationKeys, UIAccelModifiers uiAccelModifiers)
        {
            return false;
        }

        internal static void StartSearchAction(SearchControlDataSource dataSource, string searchText)
        {
            dataSource.SearchResultsCount = -1;
            dataSource.OnStartSearch(searchText);
        }

        internal static void StropSearchAction(SearchControlDataSource dataSource)
        {
            dataSource.OnStopSearch();
        }

        internal static void ClearSearchAction(SearchControlDataSource dataSource)
        {
            dataSource.SearchResultsCount = -1;
            dataSource.OnClearSearch();
        }

        internal static object NotifyNavigationKeyAction(SearchControlDataSource dataSource, object parameter)
        {
            return Boxes.Box(dataSource.OnNotifyNavigationKey((SearchNavigationKeys) ((object[]) parameter)[0],
                (UIAccelModifiers) ((object[]) parameter)[1]));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
