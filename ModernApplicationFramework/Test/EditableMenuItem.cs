﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Utilities;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using TextBox = System.Windows.Controls.TextBox;

namespace ModernApplicationFramework.Test
{
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    public class EditableMenuItem : MenuItem, IStyleConnector//, IComponentConnector, IStyleConnector
    {
        private IntPtr previousHwndFocus = IntPtr.Zero;
        public static readonly DependencyProperty EditProperty = DependencyProperty.Register("Edit", typeof(object), typeof(EditableMenuItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, null));
        public static readonly DependencyProperty EditMinWidthProperty = DependencyProperty.Register("EditMinWidth", typeof(double), typeof(EditableMenuItem), new FrameworkPropertyMetadata(200.0));
        private TextBox editableTextBoxPart;
        private string textInGotFocus;
        private Collection<ValidationRule> _validationRules;
        private bool _contentLoaded;

        internal TextBox EditableTextBox => editableTextBoxPart ??
                                            (editableTextBoxPart = GetTemplateChild("PART_EditableTextBox") as TextBox);

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

        public event RoutedEventHandler AfterMenuItemTextChange;

        static EditableMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableMenuItem), new FrameworkPropertyMetadata(typeof(EditableMenuItem)));
        }

        public EditableMenuItem()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            ApplyTemplate();
            if (EditableTextBox == null)
                return;
            Binding binding = BindingOperations.GetBinding(EditableTextBox, TextBox.TextProperty);
            binding.ValidationRules.Clear();
            foreach (ValidationRule validationRule in ValidationRules)
                binding.ValidationRules.Add(validationRule);
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
            textInGotFocus = EditableTextBox.Text;
        }

        private void OnEditLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Mouse.Captured == EditableTextBox)
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
                if (string.Equals(textInGotFocus, EditableTextBox.Text, StringComparison.CurrentCulture) || AfterMenuItemTextChange == null)
                    return;
                AfterMenuItemTextChange(this, new RoutedEventArgs(e.RoutedEvent, EditableTextBox));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.OriginalSource == EditableTextBox && (NativeMethods.ModifierKeys & ModifierKeys.Alt) > ModifierKeys.None)
                e.Handled = true;
            base.OnKeyDown(e);
        }

        private void OnEditTextChanged(object sender, EventArgs e)
        {
            (EditableTextBox.Parent as FrameworkElement).BindingGroup.ValidateWithoutUpdate();
        }

        private void OnPreviewMouseDownOutsideCapturedElementHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(EditableTextBox, OnPreviewMouseDownOutsideCapturedElementHandler);
            Mouse.Capture((IInputElement)Parent, CaptureMode.SubTree);
        }

        private void OnEditPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.AcquireWin32Focus(out previousHwndFocus);
        }

        private void RestorePreviousHwndFocus()
        {
            if (!(this.previousHwndFocus != IntPtr.Zero))
                return;
            IntPtr previousHwndFocus = this.previousHwndFocus;
            this.previousHwndFocus = IntPtr.Zero;
            User32.SetFocus(previousHwndFocus);
        }


        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this, new Uri("/ModernApplicationFramework;component/Themes/Generic/EditableMenuItem.xaml", UriKind.Relative));
        }


        void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }


        [DebuggerNonUserCode]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            if (connectionId != 1)
                return;
            ((UIElement)target).GotKeyboardFocus += OnEditGotKeyboardFocus;
            ((UIElement)target).LostKeyboardFocus += OnEditLostKeyboardFocus;
            ((UIElement)target).PreviewGotKeyboardFocus += OnEditPreviewGotKeyboardFocus;
            ((UIElement)target).PreviewMouseUp += OnEditPreviewMouseUp;
            ((TextBoxBase)target).TextChanged += OnEditTextChanged;
        }
    }
}
