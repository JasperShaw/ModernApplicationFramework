using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Core.ValidationRules;

namespace ModernApplicationFramework.Controls.Dialogs
{
    public partial class GotoDialog
    {
        public GotoDialog()
        {
            InitializeComponent();
            var rangeValidationRule = new RangeValidationRule();
            BindingOperations.SetBinding(rangeValidationRule.BindingTarget, RangeValidationRule.MinimumProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("DataContext.MinimumLine", Array.Empty<object>())
            });
            BindingOperations.SetBinding(rangeValidationRule.BindingTarget, RangeValidationRule.MaximumProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("DataContext.MaximumLine", Array.Empty<object>())
            });
            LineNumberTextBox.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath("CurrentLine", Array.Empty<object>()),
                Mode = BindingMode.TwoWay,
                ValidationRules = {
                     rangeValidationRule
                },
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                NotifyOnTargetUpdated = true
            });
            LineNumberTextBox.TargetUpdated += (param1, param2) =>
            {
                InputMethod.SetIsInputMethodEnabled(LineNumberTextBox, false);
                LineNumberTextBox.Focus();
            };
        }

        private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            LineNumberTextBox.SelectAll();
        }

        private void HandleOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void HandleCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
