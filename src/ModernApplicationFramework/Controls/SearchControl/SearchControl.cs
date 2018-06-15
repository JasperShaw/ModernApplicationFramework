using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
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
        private SearchTextBox SearchBox;
        private Button SearchButton;
        private ToggleButton SearchDropdownButton;
        private Popup SearchPopup;
        private SmoothProgressBar SearchProgressBar;
        private LiveTextBlock LiveSearchTextBlock;
        private SearchControlTimer[] Timers;

        private SearchStartType SearchStartType = DefaultSettings.SearchStartType;
        private uint DelayedSearchStartMilliseconds = DefaultSettings.SearchStartDelay;
        private bool RestartSearchIfUnchanged = DefaultSettings.RestartSearchIfUnchanged;
        private bool SearchTrimsWhitespaces = DefaultSettings.SearchTrimsWhitespaces;
        private uint SearchStartMinChars = DefaultSettings.SearchStartMinChars;
        private SearchProgressType SearchProgressType = DefaultSettings.SearchProgressType;
        private uint DelayedShowProgressMilliseconds = DefaultSettings.SearchProgressShowDelay;
        private bool ForwardEnterKeyOnSearch = DefaultSettings.ForwardEnterKeyOnSearchStart;

        private static SearchSettingsDataSource DefaultSettings = new SearchSettingsDataSource();


        public static readonly DependencyProperty HasPopupProperty = DependencyProperty.Register(
            "HasPopup", typeof(bool), typeof(SearchControl), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(SearchControl), new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsPopupOpenChanged));

        public bool IsPopupOpen
        {
            get => (bool) GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        public bool HasPopup
        {
            get => (bool) GetValue(HasPopupProperty);
            set => SetValue(HasPopupProperty, value);
        }

        private SearchStatus SearchStatus { get; set; }

        private bool IsTextInputInProgress { get; set; }

        private bool IsTextChangedDuringInput { get; set; }

        private bool InternalStatusChange { get; set; }

        private string LastSearchText { get; set; }

        private bool LastSearchCleared { get; set; }

        private SearchControlDataSource DataSource => DataContext as SearchControlDataSource;

        private bool IsDataSourceAvailable => DataSource != null;

        static SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl), new FrameworkPropertyMetadata(typeof(SearchControl)));
            EventManager.RegisterClassHandler(typeof(SearchControl), PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseButtonDown));
            EventManager.RegisterClassHandler(typeof(SearchControl), GotMouseCaptureEvent, new MouseEventHandler(OnGotMouseCapture));
            EventManager.RegisterClassHandler(typeof(SearchControl), LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture), true);
        }

        public SearchControl()
        {
            DataContextChanged += OnDataContextChanged;
            //Loaded += SeachControl_Loaded;
            //Unloaded += SearchControl_Unloaded;
        }

        public void FocusEnd()
        {
            if (SearchBox.IsKeyboardFocused)
                return;
            SearchBox.Focus();
            var seachBox = SearchBox;
            var text = SearchBox.Text;
            var num = text.Length;
            seachBox.CaretIndex = num;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SearchBox = GetTemplateChild("PART_SearchBox") as SearchTextBox;
            SearchBox.LostKeyboardFocus += SearchBox_LostKeyboardFocus;
            SearchBox.AddHandler(Binding.SourceUpdatedEvent, new EventHandler<DataTransferEventArgs>(SearchBox_SourceUpdated), true);
            TextCompositionManager.AddTextInputStartHandler(SearchBox, SearchBox_TextInputStart);
            SearchBox.AddHandler(TextCompositionManager.TextInputEvent, new RoutedEventHandler(SearchBox_TextInput), true);
            SearchButton = GetTemplateChild("PART_SearchButton") as Button;
            SearchButton.Click += SearchButton_Click;
            SearchDropdownButton = GetTemplateChild("PART_SearchDropdownButton") as ToggleButton;
            SearchPopup = GetTemplateChild("PART_SearchPopup") as Popup;
            SearchProgressBar = GetTemplateChild("PART_SearchProgressBar") as SmoothProgressBar;
            LiveSearchTextBlock = GetTemplateChild("PART_LiveSearchTextBlock") as LiveTextBlock;

            if (DataSource == null)
                return;
            InitializeSearchStatus();
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
                    var str = TrimSearchString(SearchBox.Text);
                    //AddToMRUItems(str);
                    if (ShouldSearchRestart(str))
                    {
                        IsPopupOpen = false;
                        StopTimer(TimerId.InitiateSearch);
                        if (SearchStatus == SearchStatus.InProgress)
                            StopSearch();
                        StartSearch(str);
                        if (ForwardEnterKeyOnSearch)
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
                            if (!string.IsNullOrEmpty(SearchBox.Text))
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
                    e.Handled = NotifyKeyPress(e.SystemKey, e);
                    break;
            }
            if (e.Handled)
                return;
            base.OnPreviewKeyDown(e);
        }

        private bool ShouldSearchRestart(string searchText)
        {
            return searchText != LastSearchText || RestartSearchIfUnchanged;
        }


        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!e.Handled && e.OriginalSource == this)
            {
                FocusSearchBox();
                e.Handled = true;
            }
            base.OnGotKeyboardFocus(e);
        }

        private void FocusSearchBox()
        {
            if (SearchBox.IsKeyboardFocused)
                return;
            SearchBox.Focus();
        }


        private void ClearSearch()
        {
            ClearSearch(true);
        }

        private void ClearSearch(bool fClearSearchText)
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
            // TODO: Invoke Clear Search on DataSource
        }

        private void StopSearch()
        {
            StopSearch(true);
        }

        private void StopSearch(bool fClosePopup)
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
            // TODO: Invoke Stop Search on DataSource
        }

        private void StartSearch(string searchText, bool fAutomaticSearch = false)
        {
            StopTimer(TimerId.InitiateSearch);
            //StopTimer(ClosePopup);
            if (fAutomaticSearch)
            {
                //StartTimer(ClosePopup);
            }
            else
                IsPopupOpen = false;
            if (!ShouldSearchRestart(searchText))
                return;
            LastSearchText = searchText;
            if (searchText.Length == 0 || searchText.Length < SearchStartMinChars & fAutomaticSearch)
                ClearSearch(false);
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
                // TODO: Invoke StartSearch on DataSource
            }
        }

        private void OnSearchTextOrOptionsChanged()
        {
            var searchText = TrimSearchString(DataSource.SearchText);
            if (SearchStatus == SearchStatus.InProgress)
                StopSearch(false);
            //PopulateMRUItemsList(!this.PrefixFilterMRUItems ? string.Empty : searchText, true);
            if (SearchStartType == SearchStartType.Instant)
                StartSearch(searchText);
            else
            {
                //if (HasPopup && !SearchUseMRU && SearchPopupAutoDropdown)
                //    IsPopupOpen = true;
                if (SearchStartType == SearchStartType.Delayed)
                {
                    StopTimer(TimerId.InitiateSearch);
                    StartTimer(TimerId.InitiateSearch);
                }
            }
            TemporarilyDisableNavigationLocationChanges();
        }

        private void TemporarilyDisableNavigationLocationChanges()
        {
            
        }

        private void SetSearchBoxText(string text)
        {
            DataSource.SearchText = text;
            SearchBox.CaretIndex = text.Length;
        }

        private void ClearCurrentNavigationLocation()
        {
            if (!HasPopup)
                return;
            SearchPopupNavigationService.ClearCurrentLocation(SearchPopup);
        }

        private bool NotifyKeyPress(Key key, KeyEventArgs e)
        {
            bool flag;
            if (!IsPopupOpen)
                flag = NotifyNavigationKeyPress(key);
            else
            {
                flag = OnPopupControlKeyPress(key, e);
                if (!flag)
                    flag = NotifyNavigationKeyPress(key);
            }
            return flag;
        }

        private bool OnPopupControlKeyPress(Key key, KeyEventArgs e)
        {
            switch (key)
            {
                case Key.Prior:
                    SearchPopupNavigationService.NavigateFirst(SearchPopup);
                    break;
                case Key.Next:
                    SearchPopupNavigationService.NavigateLast(SearchPopup);
                    break;
                case Key.End:
                    if (SearchPopupNavigationService.GetCurrentLocation(SearchPopup) == null)
                        return false;
                    SearchPopupNavigationService.NavigateLast(SearchPopup);
                    break;
                case Key.Home:
                    if (SearchPopupNavigationService.GetCurrentLocation(SearchPopup) == null)
                        return false;
                    SearchPopupNavigationService.NavigateFirst(SearchPopup);
                    break;
                case Key.Up:
                    SearchPopupNavigationService.NavigatePrevious(SearchPopup);
                    break;
                case Key.Down:
                    SearchPopupNavigationService.NavigateNext(SearchPopup);
                    break;
                default:
                    //var currentLocation = SearchPopupNavigationService.GetCurrentLocation(SearchPopup) as ISearchControlPopupLocation;
                    //if (currentLocation == null || SearchPopupNavigationService.GetCurrentLocationSetMode(SearchPopup) == CurrentLocationSetMode.ByMouse)
                    //    return false;
                    //currentLocation.OnKeyDown(e);
                    return e.Handled;
            }
            //using (new TemporarilyPauseTimer(GetTimer(TimerId.ClosePopup)))
            //    ;
            return true;
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
            return false;

            //TODO: Invoke Data Source
            //return (bool)Utilities.Invoke(DataSource, SearchControlDataSource.VerbNames.NotifyNavigationKey, new[]
            //{
            //    searchNavigationKeys,
            //    uiAccelModifiers
            //});
        }

        private static UIAccelModifiers GetUiAccelModifiers(ModifierKeys modifiers)
        {
            var uiAccelModifiers = UIAccelModifiers.None;
            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                uiAccelModifiers |= UIAccelModifiers.Control;
            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                uiAccelModifiers |= UIAccelModifiers.Shift;
            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                uiAccelModifiers |= UIAccelModifiers.Alt;
            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                uiAccelModifiers |= UIAccelModifiers.Windows;
            return uiAccelModifiers;
        }


        private string TrimSearchString(string originalText)
        {
            if (!SearchTrimsWhitespaces)
                return originalText;
            return originalText.Trim();
        }


        private void InitializeSearchStatus()
        {
            SearchStatus = DataSource.SearchStatus;
            if (SearchProgressBar != null)
            {
                StopTimer(TimerId.ShowProgress);
                if (SearchStatus != SearchStatus.InProgress)
                    HideProgressBar();
                else
                {
                    SearchProgressBar.InitializeProgress();
                    StartTimer(TimerId.ShowProgress);
                }
            }
            if(InternalStatusChange)
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
            Timers = new[]
            {
                new SearchControlTimer(InitiateSearchTimer_Tick,DelayedSearchStartMilliseconds, DispatcherPriority.Background),
                new SearchControlTimer(ShowProgressTimer_Tick, DelayedShowProgressMilliseconds),
            };
        }

        private void TerminateTimers()
        {
            if (Timers == null)
                return;
            foreach (var timer in Timers)
                timer.Stop();
            Timers = null;
        }

        private SearchControlTimer GetTimer(TimerId timerId)
        {
            if ((int) timerId == 2)
                return null;
            return Timers?[(int) timerId];
        }

        private void StartTimer(TimerId timerId)
        {
            if ((int)timerId == 2)
                return;
            Timers?[(int)timerId].Start();
        }

        private void StopTimer(TimerId timerId)
        {
            if ((int)timerId == 2)
                return;
            Timers?[(int)timerId].Stop();
        }

        private void ShowProgressBar()
        {
            if (SearchProgressBar == null)
                return;
            var searchProgressType = SearchProgressType;
            switch (searchProgressType)
            {
                case SearchProgressType.None:
                    HideProgressBar();
                    break;
                case SearchProgressType.Indeterminate:
                case SearchProgressType.Determinate:
                    SearchProgressBar.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void HideProgressBar()
        {
            if (SearchProgressBar == null)
                return;
            SearchProgressBar.Visibility = Visibility.Collapsed;
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OnSearchButtonClicked();
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

        private void SearchBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            ClearCurrentNavigationLocation();
            if (IsTextInputInProgress)
                IsTextChangedDuringInput = true;
            else
                OnSearchTextOrOptionsChanged();
        }

        private void SearchBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsKeyboardFocusWithin)
                return;
            IsPopupOpen = false;
            if (!IsDataSourceAvailable || SearchStatus == SearchStatus.NotStarted)
                return;
            //this.AddToMRUItems(TrimSearchString(SearchBox.Text));
        }

        private void SearchControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void SeachControl_Loaded(object sender, RoutedEventArgs e)
        {
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
                SearchStartType = searchSettings.SearchStartType;
                DelayedSearchStartMilliseconds = searchSettings.SearchStartDelay;
                RestartSearchIfUnchanged = searchSettings.RestartSearchIfUnchanged;
                SearchTrimsWhitespaces = searchSettings.SearchTrimsWhitespaces;
                SearchStartMinChars = searchSettings.SearchStartMinChars;
                SearchProgressType = searchSettings.SearchProgressType;
                DelayedShowProgressMilliseconds = searchSettings.SearchProgressShowDelay;
                ForwardEnterKeyOnSearch = searchSettings.ForwardEnterKeyOnSearchStart;
                InitializeTimers();
            }
            else
                TerminateTimers();
        }

        private void OnDataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchControlDataSource.SearchStatus))
                InitializeSearchStatus();
        }

        private void OnSearchButtonClicked()
        {
            FocusSearchBox();
            switch (SearchStatus)
            {
                case SearchStatus.NotStarted:
                    var str = TrimSearchString(SearchBox.Text);
                    StartSearch(str);
                    //AddToMRUItems(str);
                    break;
                case SearchStatus.InProgress:
                    StopSearch();
                    break;
                case SearchStatus.Complete:
                    ClearSearch();
                    break;
            }
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var element = (SearchControl)sender;
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
                element.IsPopupOpen = false;
        }

        private static void OnGotMouseCapture(object sender, MouseEventArgs e)
        {
            if (!(sender is SearchControl searchControl) || !(e.OriginalSource is FrameworkElement frameworkElement))
                return;
            if (frameworkElement.Parent != searchControl.SearchPopup)
                return;
            searchControl.SearchPopup.StaysOpen = true;
            Mouse.Capture(searchControl, CaptureMode.SubTree);
            searchControl.SearchPopup.StaysOpen = false;
            e.Handled = true;
        }

        private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var searchControl = sender as SearchControl;

            if (!(e.OriginalSource is Visual originalSource) || !searchControl.IsPopupOpen || (searchControl.SearchPopup == originalSource || searchControl.SearchPopup.IsLogicalAncestorOf(originalSource as DependencyObject)))
                return;
            searchControl.IsPopupOpen = false;
            if (searchControl.SearchDropdownButton != originalSource && !searchControl.SearchDropdownButton.IsLogicalAncestorOf(originalSource))
                return;
            e.Handled = true;
        }

        private static void OnIsPopupOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void InitiateSearchTimer_Tick(object sender, EventArgs e)
        {
            StartSearch(TrimSearchString(SearchBox.Text), true);
        }

        private void ShowProgressTimer_Tick(object sender, EventArgs e)
        {
            StopTimer(TimerId.ShowProgress);
            ShowProgressBar();
        }



        private class SearchControlTimer
        {
            private DispatcherTimer _timer;
            private readonly DispatcherPriority _timerPriority;
            private readonly EventHandler _tickHandler;
            private readonly TimeSpan _interval;

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

            public SearchControlTimer(EventHandler tick, double interval, DispatcherPriority timerPriority = DispatcherPriority.Render)
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

            private bool TimerPaused { get; set; }

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

        private enum TimerId
        {
            InitiateSearch,
            ShowProgress,
            //ClosePopup,
        }
    }

    public enum SearchStatus
    {
        NotStarted,
        InProgress,
        Complete,
    }

    public enum SearchNavigationKeys : uint
    {
        Enter,
        Down,
        Up,
        PageDown,
        PageUp,
        Home,
        End,
        Escape
    }

    [Flags]
    public enum UIAccelModifiers : uint
    {
        None = 0,
        Shift = 1,
        Control = 2,
        Alt = 4,
        Windows = 8,
    }
}
