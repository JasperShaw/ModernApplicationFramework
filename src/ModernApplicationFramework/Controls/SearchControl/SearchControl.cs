using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    [TemplatePart(Name = "PART_SearchBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_SearchButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SearchDropdownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_SearchPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_SearchProgressBar", Type = typeof(SmoothProgressBar))]
    public class SearchControl : Control
    {
        public static readonly DependencyProperty HasPopupProperty = DependencyProperty.Register(
            "HasPopup", typeof(bool), typeof(SearchControl), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly SearchSettingsDataSource DefaultSettings = new SearchSettingsDataSource();

        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(SearchControl),
            new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsPopupOpenChanged));


        private uint _delayedClosePopupMilliseconds = DefaultSettings.SearchPopupCloseDelay;
        private uint _delayedSearchStartMilliseconds = DefaultSettings.SearchStartDelay;
        private uint _delayedShowProgressMilliseconds = DefaultSettings.SearchProgressShowDelay;
        private bool _forwardEnterKeyOnSearch = DefaultSettings.ForwardEnterKeyOnSearchStart;
        private bool _prefixFilterMruItems = DefaultSettings.PrefixFilterMruItems;
        private bool _restartSearchIfUnchanged = DefaultSettings.RestartSearchIfUnchanged;
        private SearchTextBox _searchBox;
        private Button _searchButton;
        private ToggleButton _searchDropdownButton;
        private Popup _searchPopup;
        private bool _searchPopupAutoDropdown = DefaultSettings.SearchPopupAutoDropdown;
        private SmoothProgressBar _searchProgressBar;
        private SearchProgressType _searchProgressType = DefaultSettings.SearchProgressType;
        private uint _searchStartMinChars = DefaultSettings.SearchStartMinChars;

        private SearchStartType _searchStartType = DefaultSettings.SearchStartType;
        private bool _searchTrimsWhitespaces = DefaultSettings.SearchTrimsWhitespaces;

        private bool _searchUseMru = DefaultSettings.SearchUseMru;
        private SearchControlTimer[] _timers;

        public bool HasPopup
        {
            get => (bool) GetValue(HasPopupProperty);
            set => SetValue(HasPopupProperty, value);
        }

        public bool IsPopupOpen
        {
            get => (bool) GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        private SearchControlDataSource DataSource => DataContext as SearchControlDataSource;

        private bool InternalStatusChange { get; set; }

        private bool IsDataSourceAvailable => DataSource != null;

        private bool IsTextChangedDuringInput { get; set; }

        private bool IsTextInputInProgress { get; set; }

        private string LastMruPopulationRequestText { get; set; }

        private string LastMruPopulationText { get; set; }

        private bool LastSearchCleared { get; set; }

        private string LastSearchText { get; set; }

        private SearchStatus SearchStatus { get; set; }

        static SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl),
                new FrameworkPropertyMetadata(typeof(SearchControl)));
            EventManager.RegisterClassHandler(typeof(SearchControl), PreviewMouseDownEvent,
                new MouseButtonEventHandler(OnPreviewMouseButtonDown));
            EventManager.RegisterClassHandler(typeof(SearchControl), GotMouseCaptureEvent,
                new MouseEventHandler(OnGotMouseCapture));
            EventManager.RegisterClassHandler(typeof(SearchControl), LostMouseCaptureEvent,
                new MouseEventHandler(OnLostMouseCapture), true);
        }

        public SearchControl()
        {
            DataContextChanged += OnDataContextChanged;
            //Loaded += SeachControl_Loaded;
            //Unloaded += SearchControl_Unloaded;
        }

        private enum TimerId
        {
            InitiateSearch,
            ShowProgress,
            ClosePopup
        }

        public void FocusEnd()
        {
            if (_searchBox.IsKeyboardFocused)
                return;
            _searchBox.Focus();
            var seachBox = _searchBox;
            var text = _searchBox.Text;
            var num = text.Length;
            seachBox.CaretIndex = num;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AddHandler(SearchMruListBoxItem.MruItemSelectedEvent, new RoutedEventHandler(OnMruItemSelected));
            AddHandler(SearchMruListBoxItem.MruItemDeletedEvent, new RoutedEventHandler(OnMruItemDeleted));
            _searchBox = GetTemplateChild("PART_SearchBox") as SearchTextBox;
            _searchBox.LostKeyboardFocus += SearchBox_LostKeyboardFocus;
            _searchBox.AddHandler(Binding.SourceUpdatedEvent,
                new EventHandler<DataTransferEventArgs>(SearchBox_SourceUpdated), true);
            TextCompositionManager.AddTextInputStartHandler(_searchBox, SearchBox_TextInputStart);
            _searchBox.AddHandler(TextCompositionManager.TextInputEvent, new RoutedEventHandler(SearchBox_TextInput),
                true);
            _searchButton = GetTemplateChild("PART_SearchButton") as Button;
            _searchButton.Click += SearchButton_Click;
            _searchDropdownButton = GetTemplateChild("PART_SearchDropdownButton") as ToggleButton;
            _searchPopup = GetTemplateChild("PART_SearchPopup") as Popup;
            _searchProgressBar = GetTemplateChild("PART_SearchProgressBar") as SmoothProgressBar;

            if (DataSource == null)
                return;
            InitializeSearchStatus();
        }


        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!e.Handled && Equals(e.OriginalSource, this))
            {
                FocusSearchBox();
                e.Handled = true;
            }

            base.OnGotKeyboardFocus(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    if (IsPopupOpen)
                    {
                        OnPopupControlKeyPress(e.Key, e);
                        if (e.Handled)
                            break;
                    }

                    var str = TrimSearchString(_searchBox.Text);
                    AddToMruItems(str);
                    if (ShouldSearchRestart(str))
                    {
                        IsPopupOpen = false;
                        StopTimer(TimerId.InitiateSearch);
                        if (SearchStatus == SearchStatus.InProgress)
                            StopSearch();
                        StartSearch(str);
                        if (_forwardEnterKeyOnSearch)
                            NotifyKeyPress(e.Key, e);
                        e.Handled = true;
                        break;
                    }

                    e.Handled = NotifyKeyPress(e.Key, e);
                    break;
                case Key.Escape:
                    if (IsPopupOpen)
                    {
                        IsPopupOpen = false;
                        e.Handled = true;
                        break;
                    }

                    switch (SearchStatus)
                    {
                        case SearchStatus.NotStarted:
                            if (!string.IsNullOrEmpty(_searchBox.Text))
                            {
                                SetSearchBoxText(string.Empty);
                                e.Handled = true;
                                break;
                            }

                            e.Handled = NotifyKeyPress(e.Key, e);
                            break;
                        case SearchStatus.InProgress:
                            StopSearch();
                            e.Handled = true;
                            break;
                        case SearchStatus.Complete:
                            ClearSearch();
                            e.Handled = true;
                            break;
                    }

                    break;
                case Key.F1:
                    break;
                case Key.System:
                    if ((e.SystemKey == Key.Up || e.SystemKey == Key.Down) && HasPopup)
                    {
                        IsPopupOpen = !IsPopupOpen;
                        e.Handled = true;
                        break;
                    }

                    e.Handled = NotifyKeyPress(e.SystemKey, e);
                    break;
                default:
                    e.Handled = NotifyKeyPress(e.Key, e);
                    break;
            }

            if (e.Handled)
                return;
            base.OnPreviewKeyDown(e);
        }

        private static UiAccelModifiers GetUiAccelModifiers(ModifierKeys modifiers)
        {
            var uiAccelModifiers = UiAccelModifiers.None;
            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                uiAccelModifiers |= UiAccelModifiers.Control;
            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                uiAccelModifiers |= UiAccelModifiers.Shift;
            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                uiAccelModifiers |= UiAccelModifiers.Alt;
            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                uiAccelModifiers |= UiAccelModifiers.Windows;
            return uiAccelModifiers;
        }

        private static void OnGotMouseCapture(object sender, MouseEventArgs e)
        {
            if (!(sender is SearchControl searchControl) || !(e.OriginalSource is FrameworkElement frameworkElement))
                return;
            if (frameworkElement.Parent != searchControl._searchPopup)
                return;
            searchControl._searchPopup.StaysOpen = true;
            Mouse.Capture(searchControl, CaptureMode.SubTree);
            searchControl._searchPopup.StaysOpen = false;
            e.Handled = true;
        }

        private static void OnIsPopupOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchControl = d as SearchControl;
            var newValue = (bool) e.NewValue;

            if (newValue)
            {
                SearchPopupNavigationService.Register(searchControl._searchPopup);
                var searchText = !searchControl._prefixFilterMruItems
                    ? string.Empty
                    : searchControl.DataSource.SearchText;
                searchControl.PopulateMruItemsList(searchText, false);
                searchControl.FocusSearchBox();
                Mouse.Capture(searchControl, CaptureMode.SubTree);
                searchControl.TemporarilyDisableNavigationLocationChanges();
            }
            else
            {
                SearchPopupNavigationService.Unregister(searchControl._searchPopup);
                if (Mouse.Captured == searchControl)
                    Mouse.Capture(null);
            }

            searchControl.StopTimer(TimerId.ClosePopup);
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var element = (SearchControl) sender;
            var originalSource = e.OriginalSource as FrameworkElement;
            if (Mouse.Captured == element)
                return;

            if (originalSource == element)
            {
                if (Mouse.Captured != null && element.IsLogicalAncestorOf(Mouse.Captured as DependencyObject))
                    return;
                element.IsPopupOpen = false;
                e.Handled = true;
            }
            else if (element.IsLogicalAncestorOf(originalSource))
            {
                if (!element.IsPopupOpen || Mouse.Captured != null ||
                    !(User32.GetCapture() == IntPtr.Zero))
                    return;
                Mouse.Capture(element, CaptureMode.SubTree);
                e.Handled = true;
            }
            else
            {
                element.IsPopupOpen = false;
            }
        }

        private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var searchControl = sender as SearchControl;

            if (!(e.OriginalSource is Visual originalSource) || !searchControl.IsPopupOpen ||
                searchControl._searchPopup == originalSource ||
                searchControl._searchPopup.IsLogicalAncestorOf(originalSource))
                return;
            searchControl.IsPopupOpen = false;
            if (searchControl._searchDropdownButton != originalSource &&
                !searchControl._searchDropdownButton.IsLogicalAncestorOf(originalSource))
                return;
            e.Handled = true;
        }

        private void AddToMruItems(string searchedText)
        {
            if (!_searchUseMru || searchedText.Length == 0)
                return;

            var items = DataSource.SearchMruItems;
            var num = items.Count;

            for (var i = 0; i < num; ++i)
            {
                var item = items[i];
                if (string.Equals(item.Text, searchedText, StringComparison.CurrentCulture))
                {
                    if (i == 0)
                        return;
                    SearchMruItem.Select(item);
                    return;
                }
            }

            SearchControlDataSource.AddMruItemAction(DataSource, searchedText);
        }

        private void ClearCurrentNavigationLocation()
        {
            if (!HasPopup)
                return;
            SearchPopupNavigationService.ClearCurrentLocation(_searchPopup);
        }


        private void ClearSearch(bool fClearSearchText = true)
        {
            if (fClearSearchText)
            {
                SetSearchBoxText(string.Empty);
                LastSearchText = string.Empty;
                IsPopupOpen = false;
            }

            if (LastSearchCleared)
                return;
            LastSearchCleared = true;
            try
            {
                InternalStatusChange = true;
                DataSource.SearchStatus = SearchStatus.NotStarted;
            }
            finally
            {
                InternalStatusChange = false;
            }

            SearchControlDataSource.ClearSearchAction(DataSource);
        }

        private void ClosePopupTimer_Tick(object sender, EventArgs e)
        {
            if (IsPopupOpen && _searchPopup.IsMouseOver)
                StartTimer(TimerId.ClosePopup);
            else
                IsPopupOpen = false;
        }

        private void FocusSearchBox()
        {
            if (_searchBox.IsKeyboardFocused)
                return;
            _searchBox.Focus();
        }

        private SearchControlTimer GetTimer(TimerId timerId)
        {
            return _timers?[(int) timerId];
        }

        private void HideProgressBar()
        {
            if (_searchProgressBar == null)
                return;
            _searchProgressBar.Visibility = Visibility.Collapsed;
        }

        private void InitializeSearchPopupStatus()
        {
            //TODO: Filter stuff
            HasPopup = _searchUseMru;
        }


        private void InitializeSearchStatus()
        {
            SearchStatus = DataSource.SearchStatus;
            if (_searchProgressBar != null)
            {
                StopTimer(TimerId.ShowProgress);
                if (SearchStatus != SearchStatus.InProgress)
                {
                    HideProgressBar();
                }
                else
                {
                    _searchProgressBar.InitializeProgress();
                    StartTimer(TimerId.ShowProgress);
                }
            }

            if (InternalStatusChange)
                return;
            if (SearchStatus == SearchStatus.NotStarted)
            {
                LastSearchCleared = true;
                LastSearchText = string.Empty;
            }
            else
            {
                if (SearchStatus != SearchStatus.InProgress)
                    return;
                LastSearchCleared = false;
                LastSearchText = DataSource.SearchText;
            }
        }

        private void InitializeTimers()
        {
            TerminateTimers();
            _timers = new[]
            {
                new SearchControlTimer(InitiateSearchTimer_Tick, _delayedSearchStartMilliseconds,
                    DispatcherPriority.Background),
                new SearchControlTimer(ShowProgressTimer_Tick, _delayedShowProgressMilliseconds),
                new SearchControlTimer(ClosePopupTimer_Tick, _delayedClosePopupMilliseconds)
            };
        }

        private void InitiateSearchTimer_Tick(object sender, EventArgs e)
        {
            StartSearch(TrimSearchString(_searchBox.Text), true);
        }

        private bool NotifyKeyPress(Key key, KeyEventArgs e)
        {
            bool flag;
            if (!IsPopupOpen)
            {
                flag = NotifyNavigationKeyPress(key);
            }
            else

            {
                flag = OnPopupControlKeyPress(key, e);
                if (!flag)
                    flag = NotifyNavigationKeyPress(key);
            }

            return flag;
        }

        private bool NotifyNavigationKeyPress(Key key)
        {
            SearchNavigationKeys searchNavigationKeys;
            switch (key)
            {
                case Key.Return:
                    searchNavigationKeys = SearchNavigationKeys.Enter;
                    break;
                case Key.Escape:
                    searchNavigationKeys = SearchNavigationKeys.Escape;
                    break;
                case Key.Prior:
                    searchNavigationKeys = SearchNavigationKeys.PageUp;
                    break;
                case Key.Next:
                    searchNavigationKeys = SearchNavigationKeys.PageDown;
                    break;
                case Key.End:
                    searchNavigationKeys = SearchNavigationKeys.End;
                    break;
                case Key.Home:
                    searchNavigationKeys = SearchNavigationKeys.Home;
                    break;
                case Key.Up:
                    searchNavigationKeys = SearchNavigationKeys.Up;
                    break;
                case Key.Down:
                    searchNavigationKeys = SearchNavigationKeys.Down;
                    break;
                default:
                    return false;
            }

            var uiAccelModifiers = GetUiAccelModifiers(NativeMethods.ModifierKeys);

            var parameter = new object[]
            {
                searchNavigationKeys,
                uiAccelModifiers
            };
            return (bool) SearchControlDataSource.NotifyNavigationKeyAction(DataSource, parameter);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldValue)
                oldValue.PropertyChanged -= OnDataSourcePropertyChanged;
            if (e.NewValue is SearchControlDataSource newValue)
            {
                InitializeSearchStatus();
                newValue.PropertyChanged += OnDataSourcePropertyChanged;
                var searchSettings = newValue.SearchSettings;
                _searchStartType = searchSettings.SearchStartType;
                _delayedSearchStartMilliseconds = searchSettings.SearchStartDelay;
                _restartSearchIfUnchanged = searchSettings.RestartSearchIfUnchanged;
                _searchTrimsWhitespaces = searchSettings.SearchTrimsWhitespaces;
                _searchStartMinChars = searchSettings.SearchStartMinChars;
                _searchProgressType = searchSettings.SearchProgressType;
                _delayedShowProgressMilliseconds = searchSettings.SearchProgressShowDelay;
                _forwardEnterKeyOnSearch = searchSettings.ForwardEnterKeyOnSearchStart;
                _delayedClosePopupMilliseconds = searchSettings.SearchPopupCloseDelay;
                _searchPopupAutoDropdown = searchSettings.SearchPopupAutoDropdown;

                _searchUseMru = searchSettings.SearchUseMru;
                _prefixFilterMruItems = searchSettings.PrefixFilterMruItems;
                InitializeSearchPopupStatus();
                InitializeTimers();
            }
            else
            {
                TerminateTimers();
            }
        }

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchControlDataSource.SearchStatus))
                InitializeSearchStatus();
            else
                InitializeSearchPopupStatus();
        }

        private void OnMruItemDeleted(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as SearchMruListBoxItem;
            var typedValue = DataSource.SearchMruItems;
            var num1 = typedValue.Count;
            int num2;
            for (num2 = 0; num2 < num1; ++num2)
            {
                var item = typedValue.ElementAt(num2);
                if (source.DataContext.Equals(item))
                    break;
            }

            OnMruItemDeleted(num2, source.DataContext as SearchMruItem);
        }

        private void OnMruItemDeleted(int index, SearchMruItem searchMruItem)
        {
            Validate.IsNotNull(searchMruItem, nameof(searchMruItem));
            using (new TemporarilyPauseTimer(GetTimer(TimerId.ClosePopup)))
            {
                SearchMruItem.Delete(searchMruItem);
                _searchPopup.Dispatcher.Invoke(DispatcherPriority.Render, (Action) (() => { }));
                var items = DataSource.SearchMruItems;
                var count = items.Count;
                SearchPopupNavigationService.NavigateNext(_searchPopup,
                    index >= count ? (count <= 0 ? 1 : count) : index + 1);
            }
        }

        private void OnMruItemSelected(object sender, RoutedEventArgs e)
        {
            OnMruItemSelected((e.OriginalSource as SearchMruListBoxItem)?.DataContext as SearchMruItem);
        }

        private void OnMruItemSelected(SearchMruItem mruItem)
        {
            Validate.IsNotNull(mruItem, nameof(mruItem));
            IsPopupOpen = false;
            StopTimer(TimerId.InitiateSearch);
            var str = TrimSearchString(mruItem.Text);
            SearchMruItem.Select(mruItem);
            SetSearchBoxText(str);
            if (SearchStatus == SearchStatus.InProgress)
            {
                if (!ShouldSearchRestart(str))
                    return;
                StopSearch();
            }

            StartSearch(str);
        }

        private bool OnPopupControlKeyPress(Key key, KeyEventArgs e)
        {
            switch (key)
            {
                case Key.Prior:
                    SearchPopupNavigationService.NavigateFirst(_searchPopup);
                    break;
                case Key.Next:
                    SearchPopupNavigationService.NavigateLast(_searchPopup);
                    break;
                case Key.End:
                    if (SearchPopupNavigationService.GetCurrentLocation(_searchPopup) == null)
                        return false;
                    SearchPopupNavigationService.NavigateLast(_searchPopup);
                    break;
                case Key.Home:
                    if (SearchPopupNavigationService.GetCurrentLocation(_searchPopup) == null)
                        return false;
                    SearchPopupNavigationService.NavigateFirst(_searchPopup);
                    break;
                case Key.Up:
                    SearchPopupNavigationService.NavigatePrevious(_searchPopup);
                    break;
                case Key.Down:
                    SearchPopupNavigationService.NavigateNext(_searchPopup);
                    break;
                default:
                    if (!(SearchPopupNavigationService.GetCurrentLocation(_searchPopup) is ISearchControlPopupLocation
                            currentLocation)
                        || SearchPopupNavigationService.GetCurrentLocationSetMode(_searchPopup) ==
                        CurrentLocationSetMode.ByMouse)
                        return false;
                    currentLocation.OnKeyDown(e);
                    return e.Handled;
            }

            using (new TemporarilyPauseTimer(GetTimer(TimerId.ClosePopup)))
            {
            }

            return true;
        }

        private void OnSearchButtonClicked()
        {
            FocusSearchBox();
            switch (SearchStatus)
            {
                case SearchStatus.NotStarted:
                    var str = TrimSearchString(_searchBox.Text);
                    StartSearch(str);
                    AddToMruItems(str);
                    break;
                case SearchStatus.InProgress:
                    StopSearch();
                    break;
                case SearchStatus.Complete:
                    ClearSearch();
                    break;
            }
        }

        private void OnSearchTextOrOptionsChanged()
        {
            var searchText = TrimSearchString(DataSource.SearchText);
            if (SearchStatus == SearchStatus.InProgress)
                StopSearch(false);
            PopulateMruItemsList(!_prefixFilterMruItems ? string.Empty : searchText);
            if (_searchStartType == SearchStartType.Instant)
            {
                StartSearch(searchText);
            }
            else
            {
                if (HasPopup && !_searchUseMru && _searchPopupAutoDropdown)
                    IsPopupOpen = true;
                if (_searchStartType == SearchStartType.Delayed)
                {
                    StopTimer(TimerId.InitiateSearch);
                    StartTimer(TimerId.InitiateSearch);
                }
            }

            TemporarilyDisableNavigationLocationChanges();
        }

        private void PopulateMruItemsList(string searchText, bool validatePopupAutoShowState = true)
        {
            if (!_searchUseMru)
                return;
            if (!string.Equals(LastMruPopulationText, searchText, StringComparison.CurrentCulture) &&
                !string.Equals(LastMruPopulationRequestText, searchText, StringComparison.CurrentCulture))
            {
                LastMruPopulationRequestText = searchText;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) (() =>
                {
                    if (IsDataSourceAvailable && !string.Equals(LastMruPopulationText, LastMruPopulationRequestText,
                            StringComparison.CurrentCulture))
                    {
                        SearchControlDataSource.PopulateMruItem(DataSource, searchText);
                        LastMruPopulationText = LastMruPopulationRequestText;
                        ShowOrClosePopup(validatePopupAutoShowState);
                    }
                }));
            }
            else
            {
                ShowOrClosePopup(validatePopupAutoShowState);
            }
        }

        private void SeachControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SearchBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsKeyboardFocusWithin)
                return;
            IsPopupOpen = false;
            if (!IsDataSourceAvailable || SearchStatus == SearchStatus.NotStarted)
                return;
            AddToMruItems(TrimSearchString(_searchBox.Text));
        }

        private void SearchBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            ClearCurrentNavigationLocation();
            if (IsTextInputInProgress)
                IsTextChangedDuringInput = true;
            else
                OnSearchTextOrOptionsChanged();
        }

        private void SearchBox_TextInput(object sender, RoutedEventArgs e)
        {
            IsTextInputInProgress = false;
            if (!IsTextChangedDuringInput)
                return;
            IsTextChangedDuringInput = false;
            OnSearchTextOrOptionsChanged();
        }

        private void SearchBox_TextInputStart(object sender, TextCompositionEventArgs e)
        {
            IsTextChangedDuringInput = false;
            IsTextInputInProgress = true;
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OnSearchButtonClicked();
        }

        private void SearchControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void SetSearchBoxText(string text)
        {
            DataSource.SearchText = text;
            _searchBox.CaretIndex = text.Length;
        }

        private bool ShouldSearchRestart(string searchText)
        {
            return searchText != LastSearchText || _restartSearchIfUnchanged;
        }

        private void ShowOrClosePopup(bool validatePopupAutoShowState)
        {
            if (!validatePopupAutoShowState || _searchStartType == SearchStartType.Instant || !HasPopup ||
                !_searchPopupAutoDropdown)
                return;
            IsPopupOpen = !IsPopupOpen;
        }

        private void ShowProgressBar()
        {
            if (_searchProgressBar == null)
                return;
            var searchProgressType = _searchProgressType;
            switch (searchProgressType)
            {
                case SearchProgressType.None:
                    HideProgressBar();
                    break;
                case SearchProgressType.Indeterminate:
                case SearchProgressType.Determinate:
                    _searchProgressBar.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ShowProgressTimer_Tick(object sender, EventArgs e)
        {
            StopTimer(TimerId.ShowProgress);
            ShowProgressBar();
        }

        private void StartSearch(string searchText, bool fAutomaticSearch = false)
        {
            StopTimer(TimerId.InitiateSearch);
            StopTimer(TimerId.ClosePopup);
            if (fAutomaticSearch)
                StartTimer(TimerId.ClosePopup);
            else
                IsPopupOpen = false;
            if (!ShouldSearchRestart(searchText))
                return;
            LastSearchText = searchText;
            if (searchText.Length == 0 || (searchText.Length < _searchStartMinChars) & fAutomaticSearch)
            {
                ClearSearch(false);
            }
            else
            {
                LastSearchCleared = false;
                try
                {
                    InternalStatusChange = true;
                    DataSource.SearchStatus = SearchStatus.InProgress;
                }
                finally
                {
                    InternalStatusChange = false;
                }

                SearchControlDataSource.StartSearchAction(DataSource, searchText);
            }
        }

        private void StartTimer(TimerId timerId)
        {
            _timers?[(int) timerId].Start();
        }

        private void StopSearch(bool fClosePopup = true)
        {
            if (fClosePopup)
                IsPopupOpen = false;
            LastSearchText = null;
            try
            {
                InternalStatusChange = true;
                DataSource.SearchStatus = SearchStatus.Complete;
            }
            finally
            {
                InternalStatusChange = false;
            }

            SearchControlDataSource.StropSearchAction(DataSource);
        }

        private void StopTimer(TimerId timerId)
        {
            _timers?[(int) timerId].Stop();
        }

        private void TemporarilyDisableNavigationLocationChanges()
        {
            if (!SearchPopupNavigationService.GetIsNavigationEnabled(_searchPopup))
                return;
            SearchPopupNavigationService.SetIsNavigationEnabled(_searchPopup, false);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) (() =>
            {
                if (!this.IsConnectedToPresentationSource())
                    return;
                SearchPopupNavigationService.SetIsNavigationEnabled(_searchPopup, true);
            }));
        }

        private void TerminateTimers()
        {
            if (_timers == null)
                return;
            foreach (var timer in _timers)
                timer.Stop();
            _timers = null;
        }


        private string TrimSearchString(string originalText)
        {
            if (!_searchTrimsWhitespaces)
                return originalText;
            return originalText.Trim();
        }


        private class SearchControlTimer
        {
            private readonly TimeSpan _interval;
            private readonly EventHandler _tickHandler;
            private readonly DispatcherPriority _timerPriority;
            private DispatcherTimer _timer;

            public bool IsEnabled
            {
                get
                {
                    if (_timer != null)
                        return _timer.IsEnabled;
                    return false;
                }
                set
                {
                    if (_timer == null)
                        return;
                    _timer.IsEnabled = value;
                }
            }

            public SearchControlTimer(EventHandler tick, double interval,
                DispatcherPriority timerPriority = DispatcherPriority.Render)
            {
                _tickHandler = tick;
                _interval = TimeSpan.FromMilliseconds(interval);
                _timerPriority = timerPriority;
            }

            public void Start()
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer(_timerPriority) {Interval = _interval};
                    _timer.Tick += _tickHandler;
                }

                _timer.Start();
            }

            public void Stop()
            {
                if (_timer == null)
                    return;
                _timer.Stop();
                _timer.Tick -= _tickHandler;
                _timer = null;
            }
        }

        private class TemporarilyPauseTimer : DisposableObject
        {
            private SearchControlTimer Timer { get; }

            private bool TimerPaused { get; }

            public TemporarilyPauseTimer(SearchControlTimer timer)
            {
                Timer = timer;
                if (Timer == null || !Timer.IsEnabled)
                    return;
                timer.IsEnabled = false;
                TimerPaused = true;
            }

            protected override void DisposeManagedResources()
            {
                if (TimerPaused)
                    Timer.IsEnabled = true;
                base.DisposeManagedResources();
            }
        }
    }
}