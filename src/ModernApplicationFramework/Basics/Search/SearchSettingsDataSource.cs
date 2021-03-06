﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Basics.Search
{
    public class SearchSettingsDataSource : INotifyPropertyChanged
    {
        private SearchStartType _searchStartType = SearchStartType.Delayed;
        private uint _searchStartDelay = 1000;
        private uint _searchStartMinChars = 1;
        private string _controlPaddingThickness = "0";
        private string _controlBorderThickness = "1";
        private bool _forwardEnterKeyOnSearchStart;
        private bool _searchButtonVisible = true;
        private uint _controlMaxWidth = 400;
        private uint _controlMinWidth = 100;
        private string _searchClearTooltip = Search_Resources.SearchClearTooltip;
        private string _searchStopTooltip = Search_Resources.SearchStopTooltip;
        private string _searchStartTooltip = Search_Resources.SearchStartTooltip;
        private string _searchTooltip = Search_Resources.SearchTooltip;
        private bool _searchWatermarkWhenFocused;
        private string _searchWatermark = Search_Resources.SearchWatermark;
        private uint _searchProgressShowDelay = 200;
        private SearchProgressType _searchProgressType = SearchProgressType.Indeterminate;
        private bool _searchTrimsWhitespaces = true;
        private bool _restartSearchIfUnchanged;

        private bool _searchUseMru = true;
        private bool _prefixFilterMruItems = true;
        private uint _maximumMruItems = 5;
        private bool _searchPopupAutoDropdown = true;
        private uint _searchPopupCloseDelay = 4000;
        private uint _controlMinPopupWidth = 200;


        public event PropertyChangedEventHandler PropertyChanged;


        public bool SearchUseMru
        {
            get => _searchUseMru;
            set
            {
                if (value == _searchUseMru) return;
                _searchUseMru = value;
                OnPropertyChanged();
            }
        }

        public bool PrefixFilterMruItems
        {
            get => _prefixFilterMruItems;
            set
            {
                if (value == _prefixFilterMruItems) return;
                _prefixFilterMruItems = value;
                OnPropertyChanged();
            }
        }

        public uint MaximumMruItems
        {
            get => _maximumMruItems;
            set
            {
                if (value == _maximumMruItems) return;
                _maximumMruItems = value;
                OnPropertyChanged();
            }
        }

        public bool SearchPopupAutoDropdown
        {
            get => _searchPopupAutoDropdown;
            set
            {
                if (value == _searchPopupAutoDropdown) return;
                _searchPopupAutoDropdown = value;
                OnPropertyChanged();
            }
        }

        public uint SearchPopupCloseDelay
        {
            get => _searchPopupCloseDelay;
            set
            {
                if (value == _searchPopupCloseDelay) return;
                _searchPopupCloseDelay = value;
                OnPropertyChanged();
            }
        }

        public uint ControlMinPopupWidth
        {
            get => _controlMinPopupWidth;
            set
            {
                if (value == _controlMinPopupWidth) return;
                _controlMinPopupWidth = value;
                OnPropertyChanged();
            }
        }


        public SearchStartType SearchStartType
        {
            get => _searchStartType;
            set
            {
                if (value == _searchStartType) return;
                _searchStartType = value;
                OnPropertyChanged();
            }
        }

        public uint SearchStartDelay
        {
            get => _searchStartDelay;
            set
            {
                if (value == _searchStartDelay) return;
                _searchStartDelay = value;
                OnPropertyChanged();
            }
        }

        public uint SearchStartMinChars
        {
            get => _searchStartMinChars;
            set
            {
                if (value == _searchStartMinChars) return;
                _searchStartMinChars = value;
                OnPropertyChanged();
            }
        }

        public bool RestartSearchIfUnchanged
        {
            get => _restartSearchIfUnchanged;
            set
            {
                if (value == _restartSearchIfUnchanged) return;
                _restartSearchIfUnchanged = value;
                OnPropertyChanged();
            }
        }

        public bool SearchTrimsWhitespaces
        {
            get => _searchTrimsWhitespaces;
            set
            {
                if (value == _searchTrimsWhitespaces) return;
                _searchTrimsWhitespaces = value;
                OnPropertyChanged();
            }
        }

        public SearchProgressType SearchProgressType
        {
            get => _searchProgressType;
            set
            {
                if (value == _searchProgressType) return;
                _searchProgressType = value;
                OnPropertyChanged();
            }
        }

        public uint SearchProgressShowDelay
        {
            get => _searchProgressShowDelay;
            set
            {
                if (value == _searchProgressShowDelay) return;
                _searchProgressShowDelay = value;
                OnPropertyChanged();
            }
        }

        public string SearchWatermark
        {
            get => _searchWatermark;
            set
            {
                if (value == _searchWatermark) return;
                _searchWatermark = value;
                OnPropertyChanged();
            }
        }

        public bool SearchWatermarkWhenFocused
        {
            get => _searchWatermarkWhenFocused;
            set
            {
                if (value == _searchWatermarkWhenFocused) return;
                _searchWatermarkWhenFocused = value;
                OnPropertyChanged();
            }
        }

        public string SearchTooltip
        {
            get => _searchTooltip;
            set
            {
                if (value == _searchTooltip) return;
                _searchTooltip = value;
                OnPropertyChanged();
            }
        }

        public string SearchStartTooltip
        {
            get => _searchStartTooltip;
            set
            {
                if (value == _searchStartTooltip) return;
                _searchStartTooltip = value;
                OnPropertyChanged();
            }
        }

        public string SearchStopTooltip
        {
            get => _searchStopTooltip;
            set
            {
                if (value == _searchStopTooltip) return;
                _searchStopTooltip = value;
                OnPropertyChanged();
            }
        }

        public string SearchClearTooltip
        {
            get => _searchClearTooltip;
            set
            {
                if (value == _searchClearTooltip) return;
                _searchClearTooltip = value;
                OnPropertyChanged();
            }
        }

        public uint ControlMinWidth
        {
            get => _controlMinWidth;
            set
            {
                if (value == _controlMinWidth) return;
                _controlMinWidth = value;
                OnPropertyChanged();
            }
        }

        public uint ControlMaxWidth
        {
            get => _controlMaxWidth;
            set
            {
                if (value == _controlMaxWidth) return;
                _controlMaxWidth = value;
                OnPropertyChanged();
            }
        }

        public bool SearchButtonVisible
        {
            get => _searchButtonVisible;
            set
            {
                if (value == _searchButtonVisible) return;
                _searchButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public bool ForwardEnterKeyOnSearchStart
        {
            get => _forwardEnterKeyOnSearchStart;
            set
            {
                if (value == _forwardEnterKeyOnSearchStart) return;
                _forwardEnterKeyOnSearchStart = value;
                OnPropertyChanged();
            }
        }

        public string ControlBorderThickness
        {
            get => _controlBorderThickness;
            set
            {
                if (value == _controlBorderThickness) return;
                _controlBorderThickness = value;
                OnPropertyChanged();
            }
        }

        public string ControlPaddingThickness
        {
            get => _controlPaddingThickness;
            set
            {
                if (value == _controlPaddingThickness) return;
                _controlPaddingThickness = value;
                OnPropertyChanged();
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum SearchStartType
    {
        Instant,
        Delayed,
        OnDemand
    }

    public enum SearchProgressType
    {
        None,
        Indeterminate,
        Determinate
    }
}