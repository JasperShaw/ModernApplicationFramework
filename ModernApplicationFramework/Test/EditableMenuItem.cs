using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Utilities;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using TextBox = System.Windows.Controls.TextBox;

namespace ModernApplicationFramework.Test
{
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    public class EditableMenuItem : MenuItem //, IComponentConnector, IStyleConnector
    {
        private IntPtr _previousHwndFocus;
        public static readonly DependencyProperty EditProperty;
        public static readonly DependencyProperty EditMinWidthProperty;
        private TextBox _editableTextBoxPart;
        private RoutedEventHandler _afterMenuItemTextChange;
        private string _textInGotFocus;
        private Collection<ValidationRule> _validationRules;
        private bool _contentLoaded;

        internal TextBox EditableTextBox => _editableTextBoxPart ??
                                            (_editableTextBoxPart = GetTemplateChild("PART_EditableTextBox") as TextBox);

        public Collection<ValidationRule> ValidationRules => _validationRules ?? (_validationRules = new Collection<ValidationRule>());

        public object Edit
        {
            get => GetValue(EditProperty);
            set => SetValue(EditProperty, value);
        }

        public double EditMinWidth
        {
            get => (double)GetValue(EditMinWidthProperty);
            set => SetValue(EditMinWidthProperty, value);
        }

        public event RoutedEventHandler AfterMenuItemTextChange
        {
            add
            {
                RoutedEventHandler routedEventHandler = _afterMenuItemTextChange;
                RoutedEventHandler comparand;
                do
                {
                    comparand = routedEventHandler;
                    routedEventHandler = Interlocked.CompareExchange(ref _afterMenuItemTextChange, (RoutedEventHandler)Delegate.Combine(comparand, value), comparand);
                }
                while (routedEventHandler != comparand);
            }
            remove
            {
                RoutedEventHandler routedEventHandler = _afterMenuItemTextChange;
                RoutedEventHandler comparand;
                do
                {
                    comparand = routedEventHandler;
                    routedEventHandler = Interlocked.CompareExchange(ref _afterMenuItemTextChange, (RoutedEventHandler)Delegate.Remove(comparand, value), comparand);
                }
                while (routedEventHandler != comparand);
            }
        }

        static EditableMenuItem()
        {
            EditProperty = DependencyProperty.Register("Edit", typeof(object), typeof(EditableMenuItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null));
            EditMinWidthProperty = DependencyProperty.Register("EditMinWidth", typeof(double), typeof(EditableMenuItem), new FrameworkPropertyMetadata(200.0));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableMenuItem), new FrameworkPropertyMetadata(typeof(EditableMenuItem)));
        }

        public EditableMenuItem()
        {
            _previousHwndFocus = IntPtr.Zero;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            ApplyTemplate();
            if (EditableTextBox == null)
                return;
            var binding = BindingOperations.GetBinding(EditableTextBox, TextBox.TextProperty);
            if (binding != null)
            {
                binding.ValidationRules.Clear();
                foreach (ValidationRule validationRule in ValidationRules)
                    binding.ValidationRules.Add(validationRule);
            }

            EditableTextBox.GotKeyboardFocus += OnEditGotKeyboardFocus;
            EditableTextBox.LostKeyboardFocus += OnEditLostKeyboardFocus;
            EditableTextBox.PreviewGotKeyboardFocus += OnEditPreviewGotKeyboardFocus;
            EditableTextBox.PreviewMouseUp += OnEditPreviewMouseUp;
            EditableTextBox.TextChanged += OnEditTextChanged;
        }

        private void FocusEditBox()
        {
            Keyboard.Focus(EditableTextBox);
            EditableTextBox.SelectionStart = 0;
            EditableTextBox.SelectionLength = EditableTextBox.Text.Length;
        }

        protected override void OnClick()
        {
            if (!EditableTextBox.IsKeyboardFocused)
                FocusEditBox();
            else
                base.OnClick();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!EditableTextBox.IsKeyboardFocused)
            {
                FocusEditBox();
                e.Handled = true;
            }
            else
            {
                Keyboard.Focus(this);
                e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        private void OnEditPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OnEditGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Mouse.Capture(EditableTextBox, CaptureMode.SubTree);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(EditableTextBox, OnPreviewMouseDownOutsideCapturedElementHandler);
            _textInGotFocus = EditableTextBox.Text;
        }

        private void OnEditLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Equals(Mouse.Captured, EditableTextBox))
            {
                Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(EditableTextBox, OnPreviewMouseDownOutsideCapturedElementHandler);
                FocusManager.SetFocusedElement(Parent, null);
                Mouse.Capture((IInputElement)Parent, CaptureMode.SubTree);
            }
            RestorePreviousHwndFocus();
            IList<ValidationError> errors = Validation.GetErrors(EditableTextBox);
            if (errors.Count != 0)
            {
                
            }
            else
            {
                if (string.Equals(_textInGotFocus, EditableTextBox.Text, StringComparison.CurrentCulture) || _afterMenuItemTextChange == null)
                    return;
               _afterMenuItemTextChange(this, new RoutedEventArgs(e.RoutedEvent, EditableTextBox));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Equals(e.OriginalSource, EditableTextBox) && (NativeMethods.ModifierKeys & ModifierKeys.Alt) > ModifierKeys.None)
                e.Handled = true;
            base.OnKeyDown(e);
        }

        private void OnEditTextChanged(object sender, EventArgs e)
        {
            var frameworkElement = EditableTextBox.Parent as FrameworkElement;
            frameworkElement?.BindingGroup.ValidateWithoutUpdate();
        }

        private void OnPreviewMouseDownOutsideCapturedElementHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(EditableTextBox, OnPreviewMouseDownOutsideCapturedElementHandler);
            Mouse.Capture((IInputElement)Parent, CaptureMode.SubTree);
        }

        private void OnEditPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.AcquireWin32Focus(out _previousHwndFocus);
        }

        private void RestorePreviousHwndFocus()
        {
            if (!(_previousHwndFocus != IntPtr.Zero))
                return;
            var previousFocus = _previousHwndFocus;
            _previousHwndFocus = IntPtr.Zero;
            User32.SetFocus(previousFocus);
        }


        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this, new Uri("/ModernApplicationFramework;component/Themes/Generic/EditableMenuItem.xaml", UriKind.Relative));
        }
    }
}
