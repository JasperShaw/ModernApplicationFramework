using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Localization;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <summary>
    /// Custom combo box control
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ComboBox" />
    /// <seealso cref="System.Windows.IWeakEventListener" />
    /// <inheritdoc cref="System.Windows.Controls.ComboBox" />
    /// <seealso cref="T:System.Windows.Controls.ComboBox" />
    /// <seealso cref="T:System.Windows.IWeakEventListener" />
    public class ComboBox : System.Windows.Controls.ComboBox, IWeakEventListener
    {
        internal const double DefaultWidth = 90.0;

        public static readonly DependencyProperty DisplayedItemProperty = DependencyProperty.Register(
            nameof(DisplayedItem), typeof(IHasTextProperty), typeof(ComboBox),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender, OnDisplayedItemChanged));

        private static void OnDisplayedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).OnDisplayedItemChanged(e);
        }

        private void OnDisplayedItemChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public static readonly DependencyProperty IsEmbeddedInMenuProperty =
            DependencyProperty.Register(nameof(IsEmbeddedInMenu), typeof(bool), typeof(ComboBox),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly string[] PropertiesToObserve = {
            "DropDownWidth",
            "IsFocused",
            "SelectionBegin",
            "SelectionEnd",
            "QueryForFocusChange"
        };

        private TextBox _editableTextBoxPart;
        private ToggleButton _toggleButton;

        private WeakReference _rememberedFocus;
        private IntPtr _rememberedHwndFocus = IntPtr.Zero;
        private IntPtr _previousHwndFocus = IntPtr.Zero;
        private int _indexOfDisplayedItemOnFocus = -1;

        private bool _inKeyboardNavigationMode;

        private double _width;

        private bool _isStretchingHorizontally;

        private bool _supressQueryForFocusChangeListener;
        private bool _supressSelectionStartChange;
        private bool _supressSelectionEndChange;
        private bool _supressUpdateOnLostFocus;
        private bool _supressKillFocusFilterNotification;

        private string _displayedTextOnFocus;

        private ComboBoxDataSource _controllingDataSource;

        public MenuItem ParentMenuItem => this.FindAncestor<MenuItem>();

        private bool IsFilterKeysEnabled
        {
            get
            {
                if (!(DataContext is ComboBoxDataSource data))
                    return false;
                return data.Flags.FilterKeys;
            }
        }

        private bool ComboCommitsOnDrop
        {
            get
            {
                if (!(DataContext is ComboBoxDataSource data))
                    return false;
                return data.Flags.ComboCommitsOnDrop;
            }
        }


        //TODO type:
        public IHasTextProperty DisplayedItem
        {
            get => (IHasTextProperty)GetValue(DisplayedItemProperty);
            set => SetValue(DisplayedItemProperty, value);
        }

        public bool IsEmbeddedInMenu
        {
            get => (bool)GetValue(IsEmbeddedInMenuProperty);
            set => SetValue(IsEmbeddedInMenuProperty, Boxes.Box(value));
        }

        private System.Windows.Controls.Menu ParentMenu
        {
            get
            {
                if (!IsEmbeddedInMenu)
                    return null;
                return this.FindAncestor<Menu.Menu>();
            }
        }

        static ComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

        public ComboBox()
        {
            DataContextChanged += OnDataContextChanged;

            DteFocusHelper.HookAcquireFocus(this);
            AddHandler(MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _toggleButton = GetTemplateChild("ToggleButton") as ToggleButton;
            if (!IsEditable)
                return;
            _editableTextBoxPart = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (_editableTextBoxPart == null)
                return;
            _editableTextBoxPart.SelectionChanged += TextBoxSelectionChanged;
            _editableTextBoxPart.TextChanged += TextBoxTextChanged;
            _editableTextBoxPart.PreviewMouseUp += TextBoxPreviewMouseUp;
            _editableTextBoxPart.PreviewGotKeyboardFocus += OnEditableTextBoxPartPreviewGotKeyboardFocus;
            _editableTextBoxPart.PreviewLostKeyboardFocus += OnEditableTextBoxPartPreviewLostKeyboardFocus;
            _editableTextBoxPart.LostKeyboardFocus += OnEditableTextBoxPartLostKeyboardFocus;
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            if (IsEditable && DataObjectHelper.DataObjectHasString(e.Data))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            base.OnPreviewDragEnter(e);
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            if (IsEditable && DataObjectHelper.DataObjectHasString(e.Data))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            base.OnPreviewDragOver(e);
        }

        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            if (IsEditable && DataObjectHelper.DataObjectHasString(e.Data))
                e.Handled = true;
            base.OnPreviewDragLeave(e);
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            if (IsEditable)
            {
                var stringFromDataObject = DataObjectHelper.GetStringFromDataObject(e.Data);
                if (stringFromDataObject != null)
                {            
                    Keyboard.Focus(this);
                    Text = stringFromDataObject;
                    _controllingDataSource.SelectionBegin = _controllingDataSource.SelectionEnd = 0;
                    _editableTextBoxPart.CaretIndex = Text.Length;
                    if (ComboCommitsOnDrop)
                        HandleComboSelection(IsDropDownOpen, -1);
                    //TODO:
                    if (_controllingDataSource != null && IsFilterKeysEnabled)
                        _controllingDataSource.InvokeTextChangedEvent(FilterKeyMessages.DragDrop, Text);
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
            }
            base.OnPreviewDrop(e);
        }

        private void OnEditableTextBoxPartLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!ShouldRestoreFocus((DependencyObject)e.NewFocus))
                return;
            RestorePreviousHwndFocus();
        }

        private static bool ShouldRestoreFocus(DependencyObject newFocus)
        {
            return newFocus == null;
        }

        private void RestorePreviousHwndFocus()
        {
            if (!(_previousHwndFocus != IntPtr.Zero))
                return;
            var previousHwndFocus = _previousHwndFocus;
            _previousHwndFocus = IntPtr.Zero;
            User32.SetFocus(previousHwndFocus);
        }

        private void OnEditableTextBoxPartPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!ShouldRestoreFocus((DependencyObject)e.NewFocus))
                return;
            RestorePreviousHwndFocus();
        }

        private void OnEditableTextBoxPartPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SavePreviousHwndFocus();
        }

        private void SavePreviousHwndFocus()
        {
            this.AcquireWin32Focus(out _previousHwndFocus);
        }

        private void TextBoxPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsEmbeddedInMenu || !Equals(Mouse.Captured, _editableTextBoxPart))
                return;
            e.Handled = true;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_controllingDataSource == null || !IsFilterKeysEnabled)
                return;
            UpdateTextSelectionProperties();
            _controllingDataSource.InvokeTextChangedEvent(FilterKeyMessages.TextChanged, Text);
        }

        private void TextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_controllingDataSource == null || !IsFilterKeysEnabled)
                return;
            UpdateTextSelectionProperties();
        }

        private void UpdateTextSelectionProperties()
        {
            try
            {
                _supressSelectionStartChange = true;
                _supressSelectionEndChange = true;
                _controllingDataSource.SelectionBegin = _editableTextBoxPart.SelectionStart;
                _controllingDataSource.SelectionEnd = _editableTextBoxPart.SelectionLength + _editableTextBoxPart.SelectionStart;
            }
            finally
            {
                _supressSelectionStartChange = false;
                _supressSelectionEndChange = false;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(PropertyChangedEventManager)) || !(e is PropertyChangedEventArgs))
                return false;
            OnDataContextPropertyChanged((PropertyChangedEventArgs)e);
            return true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (_controllingDataSource != null || IsEditable && IsFilterKeysEnabled)
            {
                Trace.WriteLine(e.Key);
                int keyCodeFromWpfKey;
                var message = FilterKeyMessages.KeyDown;
                if (e.SystemKey != Key.None)
                {
                    keyCodeFromWpfKey = GetVirtualKeyCodeFromWpfKey(e.SystemKey);
                    message = FilterKeyMessages.SystemKeyDown;
                }
                else
                    keyCodeFromWpfKey = GetVirtualKeyCodeFromWpfKey(e.Key);
                e.Handled = _controllingDataSource.InvokeFilterEvent(message, Text, keyCodeFromWpfKey);
            }


            if (e.Handled || !ShouldPassPreviewKeyDownToBase(e))
                return;
            base.OnPreviewKeyDown(e);
        }

        private static int GetVirtualKeyCodeFromWpfKey(Key key)
        {
            var num = KeyInterop.VirtualKeyFromKey(key);
            switch (num)
            {
                case 160:
                case 161:
                    num = 16;
                    break;
                case 162:
                case 163:
                    num = 17;
                    break;
                case 164:
                case 165:
                    num = 18;
                    break;
            }
            return num;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.System && e.KeyboardDevice.Modifiers == ModifierKeys.Alt &&
                e.SystemKey == Key.Up)
            {
                var flag1 = e.Key == Key.Tab;
                var isSelectionFromList = IsDropDownOpen;
                var flag2 = false;
                var selectionIndex = -1;
                if (IsEditable)
                {
                    if (!string.Equals(_editableTextBoxPart.Text, _displayedTextOnFocus))
                    {
                        var num = 0;
                        foreach (var item in _controllingDataSource.Items)
                        {
                            if (string.Equals(item.Text, _editableTextBoxPart.Text))
                            {
                                selectionIndex = num;
                                break;
                            }
                            ++num;
                        }
                        flag2 = true;
                    }
                }
                else if (IsDropDownOpen)
                {
                    if (Keyboard.FocusedElement is ComboBoxItem focusedElement)
                    {
                        var dataSource = focusedElement.DataContext as IHasTextProperty;
                        if (dataSource == _controllingDataSource.DisplayedItem)
                            selectionIndex = _controllingDataSource.Items.IndexOf(dataSource);
                    }
                }
                else if (_controllingDataSource.SelectedIndex != -1 && _controllingDataSource.SelectedIndex != _indexOfDisplayedItemOnFocus)
                    selectionIndex = _controllingDataSource.SelectedIndex;
                if (IsEmbeddedInMenu)
                    ReleaseMouseInEmbeddedMode();
                if (selectionIndex != -1 | flag2)
                {
                    HandleComboSelection(isSelectionFromList, selectionIndex);
                    isSelectionFromList = false;
                }
                if (isSelectionFromList)
                    IsDropDownOpen = false;
                if (!IsKeyboardFocusWithin)
                    Keyboard.Focus(this);
                if (flag1)
                    (IsEditable ? (UIElement)e.OriginalSource : this).MoveFocus(
                        new TraversalRequest(e.KeyboardDevice.Modifiers == ModifierKeys.Shift
                            ? FocusNavigationDirection.Previous
                            : FocusNavigationDirection.Next));
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
                if (IsEditable && e.Key == Key.Return)
                {
                    if (IsEmbeddedInMenu)
                        ReleaseMouseInEmbeddedMode();
                    HandleComboSelection(IsDropDownOpen, -1);
                    e.Handled = true;
                }
                else
                {
                    if (IsEditable || IsDropDownOpen || _controllingDataSource == null)
                        return;
                    if (e.Key == Key.Down || e.Key == Key.Up)
                    {
                        //if (!_inKeyboardNavigationMode)
                        //{
                        //    _controllingDataSource.UpdateItems();
                        //}
                        _inKeyboardNavigationMode = true;
                        _controllingDataSource.InvokeSetDisplayedItemRelative(e.Key == Key.Down ? 1 : -1);
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Next || e.Key == Key.Prior)
                    {
                        //if (!_inKeyboardNavigationMode)
                        //{
                        //    _controllingDataSource.UpdateItems();
                        //}
                        _inKeyboardNavigationMode = true;
                        _controllingDataSource.InvokeSetDisplayedItemRelative(e.Key == Key.Next ? 15 : -15);
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Home || e.Key == Key.End)
                    {
                        //if (!_inKeyboardNavigationMode)
                        //{
                        //    _controllingDataSource.UpdateItems();
                        //}
                        _inKeyboardNavigationMode = true;
                        _controllingDataSource.InvokeSetDisplayedItemByIndex(e.Key == Key.Home ? 0 : _controllingDataSource.Items.Count - 1);
                        e.Handled = true;
                    }
                    else
                    {
                        if (e.Key != Key.Return || !_inKeyboardNavigationMode)
                            return;
                        _inKeyboardNavigationMode = false;
                        HandleComboSelection(true, _controllingDataSource.SelectedIndex);
                        e.Handled = true;
                    }
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (_controllingDataSource != null && IsFilterKeysEnabled && IsEditable &&
                (e.Text.Length > 0 || e.SystemText.Length > 0))
            {
                var ch = e.Text.Length != 0 ? e.Text[0] : e.SystemText[0];
                e.Handled = _controllingDataSource.InvokeFilterEvent(FilterKeyMessages.CharPressed, Text, 0, ch);
            }
            base.OnPreviewTextInput(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            if (_controllingDataSource != null)
            {
                //_controllingDataSource.UpdateItems();
                if (IsEditable)
                    _editableTextBoxPart?.SelectAll();
            }
            base.OnDropDownOpened(e);
            if (!IsFilterKeysEnabled || _controllingDataSource == null)
                return;
            _controllingDataSource.InvokeFilterEvent(FilterKeyMessages.GotFocus, Text);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (IsEditable)
            {
                if (string.IsNullOrEmpty(_displayedTextOnFocus))
                    _displayedTextOnFocus = _editableTextBoxPart.Text;
            }

            else if (_indexOfDisplayedItemOnFocus == -1)
                _indexOfDisplayedItemOnFocus = _controllingDataSource.SelectedIndex;

            if (_controllingDataSource != null && IsFilterKeysEnabled &&
                (e.Source == _editableTextBoxPart || e.OriginalSource == _editableTextBoxPart))
            {
                _controllingDataSource.InvokeFilterEvent(FilterKeyMessages.GotFocus, Text);
            }

            if (IsEditable && IsEmbeddedInMenu)
            {
                if (!IsDropDownOpen)
                    CaptureMouseInEmbeddedMode();
                DropDownClosed += SetCaptureOnDropDownClosed;
            }
            base.OnGotKeyboardFocus(e);
            if (_controllingDataSource == null)
                return;
            var keyboardFocusWithin = IsKeyboardFocusWithin;
            _controllingDataSource.IsFocused = keyboardFocusWithin;
            if (!keyboardFocusWithin || !IsEditable)
                return;
            //_controllingDataSource.UpdateItems();
            _editableTextBoxPart?.SelectAll();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _inKeyboardNavigationMode = false;
            if (_controllingDataSource != null && _controllingDataSource.IsDisposed)
                return;
            if (_controllingDataSource != null && IsFilterKeysEnabled && !_supressKillFocusFilterNotification &&
                (e.Source == _editableTextBoxPart || e.OriginalSource == _editableTextBoxPart))
            {
                _controllingDataSource.InvokeFilterEvent(FilterKeyMessages.LostFocus, Text);
            }
            base.OnLostKeyboardFocus(e);
            if (_controllingDataSource != null)
            {
                _controllingDataSource.IsFocused = IsKeyboardFocusWithin;
                try
                {
                    _supressSelectionStartChange = true;
                    _controllingDataSource.SelectionBegin = 0;
                }
                finally
                {
                    _supressSelectionStartChange = false;
                }

                if (!IsKeyboardFocusWithin)
                {
                    if (!_supressUpdateOnLostFocus)
                    {
                        try
                        {
                            _controllingDataSource.Update();
                        }
                        catch (COMException)
                        {
                        }
                    }
                }
                if (!IsEditable || !IsEmbeddedInMenu || Mouse.Captured != _editableTextBoxPart)
                    return;
                Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_editableTextBoxPart, OnPreviewMouseDownOutsideCapturedElementHandler);
                FocusManager.SetFocusedElement(ParentMenu, null);
                Mouse.Capture(ParentMenu, CaptureMode.SubTree);
            }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var constraint = base.MeasureOverride(availableSize);
            if (_isStretchingHorizontally)
            {
                if (double.IsPositiveInfinity(availableSize.Width) && double.IsPositiveInfinity(availableSize.Height))
                {
                    var width = constraint.Width;
                    var val1 = _toggleButton?.DesiredSize.Width ?? 0.0;
                    constraint.Width = Math.Max(val1, _width);
                    if (constraint.Width < width)
                        constraint = base.MeasureOverride(constraint);
                }
                else
                    constraint.Width = availableSize.Width;
            }
            return constraint;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem(this);
        }

        internal void HandleComboSelection(bool isSelectionFromList, int selectionIndex)
        {
            _indexOfDisplayedItemOnFocus = -1;
            _displayedTextOnFocus = string.Empty;
            if (_controllingDataSource == null)
                return;
            try
            {
                _supressKillFocusFilterNotification = true;
                _supressUpdateOnLostFocus = true;
                Keyboard.Focus(null);
                var focusedElement = Keyboard.FocusedElement;
                _rememberedFocus = focusedElement != null ? new WeakReference(focusedElement) : null;
                _rememberedHwndFocus = User32.GetFocus();
                if (selectionIndex == -1)
                    _controllingDataSource.ExecuteItem(Text);
                else
                    _controllingDataSource.ExecuteItem(selectionIndex);
                if (isSelectionFromList)
                    IsDropDownOpen = false;
                if (IsKeyboardFocusWithin)
                    return;
                _controllingDataSource.Update();
            }
            finally
            {
                _supressKillFocusFilterNotification = false;
                _supressUpdateOnLostFocus = false;
            }
        }

        internal bool HasGeneratedContainers()
        {
            return ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated;
        }

        private static uint ConstructKeyboardEventLParam(uint repeatCount, uint scanCode)
        {
            return (uint)((int)repeatCount & ushort.MaxValue | ((int)scanCode & byte.MaxValue) << 16);
        }


        private bool ShouldPassPreviewKeyDownToBase(KeyEventArgs e)
        {
            if (IsEditable && Equals(e.OriginalSource, _editableTextBoxPart) &&
                (e.Key == Key.Return || e.Key == Key.Escape))
                return !IsDropDownOpen;
            return true;
        }

        private void SetCaptureOnDropDownClosed(object sender, EventArgs e)
        {
            DropDownClosed -= SetCaptureOnDropDownClosed;
            if (!IsEmbeddedInMenu || !IsKeyboardFocusWithin || !Mouse.Captured.Equals(_editableTextBoxPart))
                return;
            CaptureMouseInEmbeddedMode();
        }

        private void OnControllingDataSourceDisposing(object sender, EventArgs e)
        {
            UnsubscribeFromPropertyChanges(_controllingDataSource);
        }

        private void UnsubscribeFromPropertyChanges(ComboBoxDataSource dataSource)
        {
            if (!(dataSource is INotifyPropertyChanged ds))
                return;
            foreach (var propertyName in PropertiesToObserve)
                PropertyChangedEventManager.RemoveListener(ds, this, propertyName);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (!IsKeyboardFocusWithin || IsEditable || IsDropDownOpen)
                return;
            Keyboard.Focus(null);
            args.Handled = true;
        }

        private void CacheDropDownWidth()
        {
            _width = _controllingDataSource.DropDownWidth > 0.0 ? _controllingDataSource.DropDownWidth : 90.0;
        }

        private void SubscribeToPropertyChanges(INotifyPropertyChanged dataSource)
        {
            if (!(dataSource is INotifyPropertyChanged notify))
                return;
            foreach (var propertyName in PropertiesToObserve)
                PropertyChangedEventManager.AddListener(notify, this, propertyName);
        }

        private void CaptureMouseInEmbeddedMode()
        {
            if (!IsEmbeddedInMenu || !IsEditable || !Mouse.Capture(_editableTextBoxPart, CaptureMode.SubTree))
                return;
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(_editableTextBoxPart,
                OnPreviewMouseDownOutsideCapturedElementHandler);
        }

        private void ReleaseMouseInEmbeddedMode()
        {
            if (!IsEmbeddedInMenu || !IsEditable || !Equals(Mouse.Captured, _editableTextBoxPart))
                return;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_editableTextBoxPart,
                OnPreviewMouseDownOutsideCapturedElementHandler);
            Mouse.Capture(null);
        }

        private void OnPreviewMouseDownOutsideCapturedElementHandler(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseInEmbeddedMode();
        }

        private void OnDataContextPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DropDownWidth" && _controllingDataSource != null)
            {
                CacheDropDownWidth();
            }
            else if (e.PropertyName == "IsFocused" && _controllingDataSource != null)
            {
                if (!_controllingDataSource.IsFocused || IsKeyboardFocusWithin)
                    return;
                //TODO: Test what happens if model get's changed outside this control
                Keyboard.Focus(this);
                _controllingDataSource.IsFocused = IsKeyboardFocusWithin;
            }
            else if (e.PropertyName == "SelectionBegin" && _controllingDataSource != null)
            {
                if (_editableTextBoxPart == null || _supressSelectionStartChange)
                    return;
                _editableTextBoxPart.SelectionStart = _controllingDataSource.SelectionBegin;
            }
            else if (e.PropertyName == "SelectionEnd" && _controllingDataSource != null)
            {
                if (_editableTextBoxPart == null || _supressSelectionEndChange)
                    return;
                _editableTextBoxPart.SelectionLength = _controllingDataSource.SelectionEnd -
                                                       _editableTextBoxPart.SelectionStart;
            }
            else
            {
                if (e.PropertyName != "QueryForFocusChange" || _controllingDataSource == null ||
                    _supressQueryForFocusChangeListener || !_controllingDataSource.QueryForFocusChange)
                    return;
                _supressQueryForFocusChangeListener = true;
                _controllingDataSource.QueryForFocusChange =
                    _rememberedFocus != null && !_rememberedFocus.IsAlive ||
                    (_rememberedFocus == null || !_rememberedFocus.IsAlive
                        ? null
                        : _rememberedFocus.Target as IInputElement) != Keyboard.FocusedElement ||
                    _rememberedHwndFocus != User32.GetFocus();
                _rememberedFocus = null;
                _rememberedHwndFocus = IntPtr.Zero;
                _supressQueryForFocusChangeListener = false;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_controllingDataSource != null)
            {
                _controllingDataSource.Dispose();
                _controllingDataSource = null;
            }

            if (!(e.NewValue is ComboBoxDataSource dataSource))
                return;
            _controllingDataSource = dataSource;
            SubscribeToPropertyChanges(_controllingDataSource);
            _controllingDataSource.Disposing += OnControllingDataSourceDisposing;
            _isStretchingHorizontally = _controllingDataSource.Flags.StretchHorizontally;
            CacheDropDownWidth();
        }
    }

    internal enum FilterKeyMessages
    {
        GotFocus,
        LostFocus,
        TextChanged,
        DragDrop,
        CharPressed,
        SystemKeyDown,
        KeyDown
    }
}