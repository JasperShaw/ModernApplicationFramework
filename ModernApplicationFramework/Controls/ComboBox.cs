using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls
{
    public class ComboBox : System.Windows.Controls.ComboBox, IWeakEventListener
    {
        public static readonly DependencyProperty DisplayedItemProperty;
        public static readonly DependencyProperty IsEmbeddedInMenuProperty;

        private TextBox _editableTextBoxPart;
        private ToggleButton _toggleButton;

        private WeakReference _rememberedFocus;
        private IntPtr _rememberedHwndFocus;
        private IntPtr _previousHwndFocus;
        private bool _inKeyboardNavigationMode;

        private double _width;
        internal const double DefaultWidth = 90.0;
        private bool _supressQueryForFocusChangeListener;
        private bool _supressSelectionStartChange;
        private bool _supressSelectionEndChange;
        private bool _supressUpdateOnLostFocus;
        private bool _supressKillFocusFilterNotification;

        private static readonly string[] PropertiesToObserve;

        private ComboBoxDataSource _controllingDataSource;

        public object DisplayedItem
        {
            get => GetValue(DisplayedItemProperty);
            set => SetValue(DisplayedItemProperty, value);
        }

        public bool IsEmbeddedInMenu
        {
            get => (bool)GetValue(IsEmbeddedInMenuProperty);
            set => SetValue(IsEmbeddedInMenuProperty, Boxes.Box(value));
        }

        static ComboBox()
        {
            DisplayedItemProperty = DependencyProperty.Register("DisplayedItem", typeof(object), typeof(ComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnDisplayedItemChanged));
            IsEmbeddedInMenuProperty = DependencyProperty.Register("IsEmbeddedInMenu", typeof(bool), typeof(ComboBox), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
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

        public ComboBox()
        {
            _rememberedHwndFocus = IntPtr.Zero;
            _previousHwndFocus = IntPtr.Zero;
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
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (IsEditable && e.Key == Key.Return)
            {
                if (IsEmbeddedInMenu)
                {
                    
                }
                HandleComboSelection(IsDropDownOpen, -1);
                e.Handled = true;
            }
            else
            {
                if (IsEditable || IsDropDownOpen || _controllingDataSource == null)
                    return;
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                }
                else if (e.Key == Key.Next || e.Key == Key.Prior)
                {
                    
                }
                else if (e.Key == Key.Home || e.Key == Key.End)
                {

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

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size constraint = base.MeasureOverride(availableSize);
            if (availableSize.Width == double.PositiveInfinity && availableSize.Height == double.PositiveInfinity)
            {
                var width = constraint.Width;
                var val1 = _toggleButton?.DesiredSize.Width ?? 0.0;
                constraint.Width = Math.Max(val1, _width);
                if (constraint.Width < width)
                    constraint = base.MeasureOverride(constraint);

            }
            else
                constraint.Width = availableSize.Width;
            return constraint;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem(this);
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            
        }

        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            if (IsEditable)
            {
                
            }
            base.OnPreviewDrop(e);
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
                    _controllingDataSource.ChangeDisplayedItem(selectionIndex);
                if (isSelectionFromList)
                    IsDropDownOpen = false;
            }
            finally
            {
                _supressKillFocusFilterNotification = false;
                _supressUpdateOnLostFocus = false;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _controllingDataSource = null;
            var newValue = e.NewValue as ComboBoxDataSource;
            if (newValue == null)
                return;
            _controllingDataSource = newValue;
            SubscribeToPropertyChanges(_controllingDataSource);
            //this.isStretchingHorizontally = this.controllingDataSource.Flags.StretchHorizontally;
            CacheDropDownWidth();
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

        private void SubscribeToPropertyChanges(ComboBoxDataSource dataSource)
        {
            var ds = dataSource as INotifyPropertyChanged;
            if (ds == null)
                return;
            foreach (string propertyName in PropertiesToObserve)
                PropertyChangedEventManager.AddListener(ds, this, propertyName);
        }


        private static void OnDisplayedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ComboBox)d).OnDisplayedItemChanged(e);
        }

        private void OnDisplayedItemChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(PropertyChangedEventManager)) || !(e is PropertyChangedEventArgs))
                return false;
            OnDataContextPropertyChanged((PropertyChangedEventArgs) e);
            return true;
        }

        private void OnDataContextPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DropDownWidth" && _controllingDataSource != null)
                CacheDropDownWidth();
            else if (e.PropertyName == "IsFocused" && _controllingDataSource != null)
            {
                if (!_controllingDataSource.IsFocused || IsKeyboardFocusWithin)
                    return;
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
                _editableTextBoxPart.SelectionLength = _controllingDataSource.SelectionEnd - _editableTextBoxPart.SelectionStart;
            }
            else
            {
                if (e.PropertyName != "QueryForFocusChange" || _controllingDataSource == null || _supressQueryForFocusChangeListener || !_controllingDataSource.QueryForFocusChange)
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
    }
}