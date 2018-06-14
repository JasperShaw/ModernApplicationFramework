using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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


            //if (this.DataSource == null)
            //    return;
            //InitializeSearchStatus();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    if (IsPopupOpen)
                    {
                        //OnPopupControlKeyPress(e.Key, e);
                        if (e.Handled)
                            break;
                    }
                    var str = TrimSearchString(SearchBox.Text);
                    //AddToMRUItems(str);
                    //if (this.ShouldSearchRestart(str))
                    //{
                    //    this.IsPopupOpen = false;
                    //    this.StopTimer(SearchControl.TimerId.InitiateSearch);
                    //    if (this.SearchStatus == SearchStatus.InProgress)
                    //        this.StopSearch();
                    //    this.StartSearch(str, false);
                    //    if (this.ForwardEnterKeyOnSearch)
                    //        this.NotifyKeyPress(e.Key, e);
                    //    e.Handled = true;
                    //    break;
                    //}
                    //e.Handled = NotifyKeyPress(e.Key, e);
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
                            //e.Handled = NotifyKeyPress(e.Key, e);
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
                    //e.Handled = this.NotifyKeyPress(e.SystemKey, e);
                    break;
                default:
                    //e.Handled = this.NotifyKeyPress(e.SystemKey, e);
                    break;
            }
            if (e.Handled)
                return;
            base.OnPreviewKeyDown(e);
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
        }

        private void StopSearch()
        {
            StopSearch(true);
        }

        private void StopSearch(bool fClosePopup)
        {

        }

        private void SetSearchBoxText(string text)
        {
            //Set DataSource
            SearchBox.CaretIndex = text.Length;
        }

        //private bool NotifyKeyPress(Key key, KeyEventArgs e)
        //{
        //    bool flag;
        //    if (!IsPopupOpen)
        //        flag = NotifyNavigationKeyPress(key);
        //    else
        //    {
        //        flag = OnPopupControlKeyPress(key, e);
        //        if (!flag)
        //            flag = NotifyNavigationKeyPress(key);
        //    }
        //    return flag;
        //}


        private string TrimSearchString(string originalText)
        {
            //if (!this.SearchTrimsWhitespaces)
            //    return originalText;
            return originalText.Trim();
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SearchBox_TextInput(object sender, RoutedEventArgs e)
        {
        }

        private void SearchBox_TextInputStart(object sender, TextCompositionEventArgs e)
        {
        }

        private void SearchBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void SearchBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        private void SearchControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void SeachControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
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
                    !(Native.NativeMethods.User32.GetCapture() == IntPtr.Zero))
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
    }

    public enum SearchStatus
    {
        NotStarted,
        InProgress,
        Complete,
    }
}
