using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Core.Utilities;
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

        /// <summary>
        /// The displayed item
        /// </summary>
        public static readonly DependencyProperty DisplayedItemProperty;

        /// <summary>
        /// Indication whether the combo box is inside a menu item
        /// </summary>
        public static readonly DependencyProperty IsEmbeddedInMenuProperty;


        /// <summary>
        /// The visual data source property
        /// </summary>
        public static readonly DependencyProperty VisualDataSourceProperty;
        
        /// <summary>
        /// The data source property
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty;

        private static readonly string[] PropertiesToObserve;

        private TextBox _editableTextBoxPart;
        private ToggleButton _toggleButton;

        private WeakReference _rememberedFocus;
        private IntPtr _rememberedHwndFocus;
        private IntPtr _previousHwndFocus;
        private bool _inKeyboardNavigationMode;

        private double _width;

        private bool _isStretchingHorizontally;

        private bool _supressQueryForFocusChangeListener;
        private bool _supressSelectionStartChange;
        private bool _supressSelectionEndChange;
        private bool _supressUpdateOnLostFocus;
        private bool _supressKillFocusFilterNotification;

        private ComboBoxDataSource _controllingDataSource;
        private ComboBoxVisualSource _controllingVisualSource;

        private ComboBoxAutomationPeer _peer;

        public MenuItem ParentMenuItem => this.FindAncestor<MenuItem>();

        public object DisplayedItem
        {
            get => GetValue(DisplayedItemProperty);
            set => SetValue(DisplayedItemProperty, value);
        }

        public ComboBoxVisualSource VisualDataSource
        {
            get => (ComboBoxVisualSource)GetValue(VisualDataSourceProperty);
            set => SetValue(VisualDataSourceProperty, value);
        }

        public ComboBoxDataSource DataSource
        {
            get => (ComboBoxDataSource)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
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
            DisplayedItemProperty = DependencyProperty.Register("DisplayedItem", typeof(object), typeof(ComboBox),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender, OnDisplayedItemChanged));
            IsEmbeddedInMenuProperty = DependencyProperty.Register("IsEmbeddedInMenu", typeof(bool), typeof(ComboBox),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            VisualDataSourceProperty = DependencyProperty.Register("VisualDataSource", typeof(ComboBoxVisualSource), typeof(ComboBox),
                new FrameworkPropertyMetadata(OnVisualDataSourceChanged));
            DataSourceProperty = DependencyProperty.Register("DataSource", typeof(ComboBoxDataSource), typeof(ComboBox),
                new FrameworkPropertyMetadata(OnDataSourceChanged));
            PropertiesToObserve = new[]
            {
                "DropDownWidth",
                "IsFocused",
                "SelectionBegin",
                "SelectionEnd",
                "QueryForFocusChange"
            };
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).OnDataSourceChanged(e);
        }

        private static void OnVisualDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).OnVisualDataSourceChanged(e);
        }

        public ComboBox()
        {
            _rememberedHwndFocus = IntPtr.Zero;
            _previousHwndFocus = IntPtr.Zero;

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

        private void OnEditableTextBoxPartLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            RestorePreviousHwndFocus();
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

            if (_controllingDataSource == null)
                return;
            UpdateTextSelectionProperties();
        }

        private void TextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_controllingDataSource == null)
                return;
            UpdateTextSelectionProperties();
        }

        private void UpdateTextSelectionProperties()
        {
            try
            {
                _supressSelectionStartChange = true;
                _supressSelectionEndChange = true;
                _controllingVisualSource.SelectionBegin = _editableTextBoxPart.SelectionStart;
                _controllingVisualSource.SelectionEnd = _editableTextBoxPart.SelectionLength + _editableTextBoxPart.SelectionStart;
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
            if (e.Handled || !ShouldPassPreviewKeyDownToBase(e))
                return;
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
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
                    if (!_inKeyboardNavigationMode)
                        _controllingDataSource.UpdateItems();
                    _inKeyboardNavigationMode = true;
                    _controllingDataSource.ChangeDisplayedItemRelative(e.Key == Key.Down ? 1 : -1);
                    e.Handled = true;
                }
                else if (e.Key == Key.Next || e.Key == Key.Prior)
                {
                    if (!_inKeyboardNavigationMode)
                        _controllingDataSource.UpdateItems();
                    _inKeyboardNavigationMode = true;
                    _controllingDataSource.ChangeDisplayedItemRelative(e.Key == Key.Next ? 15 : -15);
                    e.Handled = true;
                }
                else if (e.Key == Key.Home || e.Key == Key.End)
                {
                    if (!_inKeyboardNavigationMode)
                        _controllingDataSource.UpdateItems();
                    _inKeyboardNavigationMode = true;
                    _controllingDataSource.ChangeDisplayedItem(e.Key == Key.Home
                        ? 0
                        : _controllingDataSource.Items.Count - 1);
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

        protected override void OnDropDownOpened(EventArgs e)
        {
            if (_controllingDataSource != null)
                if (IsEditable)
                    _editableTextBoxPart?.SelectAll();
            base.OnDropDownOpened(e);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (IsEditable && IsEmbeddedInMenu)
            {
                if (!IsDropDownOpen)
                    CaptureMouseInEmbeddedMode();
                DropDownClosed += SetCaptureOnDropDownClosed;
            }
            base.OnGotKeyboardFocus(e);
            if (_controllingDataSource == null || _controllingVisualSource == null)
                return;
            var keyboardFocusWithin = IsKeyboardFocusWithin;
            _controllingVisualSource.IsFocused = keyboardFocusWithin;
            if (!keyboardFocusWithin || !IsEditable)
                return;
            _controllingDataSource.UpdateItems();
            _editableTextBoxPart?.SelectAll();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _inKeyboardNavigationMode = false;
            if (_controllingDataSource != null && _controllingDataSource.IsDisposed)
                return;
            base.OnLostKeyboardFocus(e);
            if (_controllingDataSource != null && _controllingVisualSource != null)
            {
                _controllingVisualSource.IsFocused = IsKeyboardFocusWithin;
                try
                {
                    _supressSelectionStartChange = true;
                    _controllingVisualSource.SelectionBegin = 0;
                }
                finally
                {
                    _supressSelectionStartChange = false;
                }
                if (!IsKeyboardFocusWithin)
                    if (!_supressUpdateOnLostFocus)
                        try
                        {
                            _controllingDataSource.UpdateItems();
                        }
                        catch (COMException)
                        {
                        }
            }
            if (!IsEditable || !IsEmbeddedInMenu || !Equals(Mouse.Captured, _editableTextBoxPart))
                return;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_editableTextBoxPart,
                OnPreviewMouseDownOutsideCapturedElementHandler);
            FocusManager.SetFocusedElement(ParentMenu, null);
            Mouse.Capture(ParentMenu, CaptureMode.SubTree);
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
                {
                    constraint.Width = availableSize.Width;
                }
            }
            return constraint;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem(this);
        }


        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new ComboBoxAutomationPeer(this);
        }

        internal void HandleComboSelection(bool isSelectionFromList, int selectionIndex)
        {
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
                {
                }
                else
                {
                    _controllingDataSource.ChangeDisplayedItem(selectionIndex);
                }
                if (isSelectionFromList)
                    IsDropDownOpen = false;
                if (IsKeyboardFocusWithin)
                    return;
                _controllingDataSource.UpdateItems();
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


        private static void OnDisplayedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).OnDisplayedItemChanged(e);
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

        private void OnDataSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_controllingDataSource != null)
            {
                _controllingDataSource.Dispose();
                _controllingDataSource = null;
            }
            var newValue = e.NewValue as ComboBoxDataSource;
            if (newValue == null)
                return;
            _controllingDataSource = newValue;
        }

        private void OnVisualDataSourceChanged(DependencyPropertyChangedEventArgs e)
        {

            if (_controllingVisualSource != null)
            {
                _controllingVisualSource.Dispose();
                _controllingVisualSource = null;
            }
            var newValue = e.NewValue as ComboBoxVisualSource;
            if (newValue == null)
                return;
            _controllingVisualSource = newValue;


            SubscribeToPropertyChanges(_controllingVisualSource);
            _controllingVisualSource.Disposing += OnControllingDataSourceDisposing;
            _isStretchingHorizontally = _controllingVisualSource.Flags.StretchHorizontally;
            CacheDropDownWidth();
        }

        private void OnControllingDataSourceDisposing(object sender, EventArgs e)
        {
            UnsubscribeFromPropertyChanges(_controllingDataSource);
        }

        private void UnsubscribeFromPropertyChanges(ComboBoxDataSource dataSource)
        {
            var ds = dataSource as INotifyPropertyChanged;
            if (ds == null)
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
            _width = _controllingVisualSource.DropDownWidth > 0.0 ? _controllingVisualSource.DropDownWidth : 90.0;
        }

        private void SubscribeToPropertyChanges(ComboBoxVisualSource visualSource)
        {
            var vs = visualSource as INotifyPropertyChanged;
            if (vs == null)
                return;
            foreach (var propertyName in PropertiesToObserve)
                PropertyChangedEventManager.AddListener(vs, this, propertyName);
        }

        private void OnDisplayedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            _peer = AutomationPeerHelper.CreatePeerFromElement<ComboBoxAutomationPeer>(this);
            var oldValue1 = e.OldValue;

            if (AutomationPeerHelper.SelectionListenersExist())
            {
                var getPeer =
                    new Func<object, System.Windows.Automation.Peers.AutomationPeer>(
                        item => _peer.GetItemAutomationPeer(item));
                var childListenersMightExists = HasGeneratedContainers() || _peer.HasHandedOutChildPeers;
                if (e.NewValue != null)
                {
                    AutomationPeerHelper.RaiseSelectionEvents(_peer, null, e.NewValue, childListenersMightExists,
                        getPeer);
                }
                else
                {
                    if (e.OldValue == null)
                        return;
                    var sourceArray1 = new object[0];
                    var sourceArray2 = new[]
                    {
                        e.OldValue
                    };
                    AutomationPeerHelper.RaiseSelectionEvents(_peer,
                        new SelectionChangedEventArgs(SelectionChangedEvent, sourceArray2, sourceArray1), null,
                        childListenersMightExists, getPeer);
                }
                if (e.NewValue != null)
                    _peer.RaiseIsSelectedChanged(e.NewValue, false, true);
                if (oldValue1 != null)
                    _peer.RaiseIsSelectedChanged(oldValue1, false, true);
            }
            if (oldValue1 == null)
                return;
            _peer.RemoveItemFromPeerCache(oldValue1);
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
            if (e.PropertyName == "DropDownWidth" && _controllingVisualSource != null)
            {
                CacheDropDownWidth();
            }
            else if (e.PropertyName == "IsFocused" && _controllingVisualSource != null)
            {
                if (!_controllingVisualSource.IsFocused || IsKeyboardFocusWithin)
                    return;
                Keyboard.Focus(this);
                _controllingVisualSource.IsFocused = IsKeyboardFocusWithin;
            }
            else if (e.PropertyName == "SelectionBegin" && _controllingVisualSource != null)
            {
                if (_editableTextBoxPart == null || _supressSelectionStartChange)
                    return;
                _editableTextBoxPart.SelectionStart = _controllingVisualSource.SelectionBegin;
            }
            else if (e.PropertyName == "SelectionEnd" && _controllingVisualSource != null)
            {
                if (_editableTextBoxPart == null || _supressSelectionEndChange)
                    return;
                _editableTextBoxPart.SelectionLength = _controllingVisualSource.SelectionEnd -
                                                       _editableTextBoxPart.SelectionStart;
            }
            else
            {
                if (e.PropertyName != "QueryForFocusChange" || _controllingVisualSource == null ||
                    _supressQueryForFocusChangeListener || !_controllingVisualSource.QueryForFocusChange)
                    return;
                _supressQueryForFocusChangeListener = true;
                _controllingVisualSource.QueryForFocusChange =
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
    }
}