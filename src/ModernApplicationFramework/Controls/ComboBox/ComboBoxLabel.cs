using System.Windows;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <inheritdoc />
    /// <summary>
    /// A special button control representing the displayed combo box item when the combo box is not editable.
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    public class ComboBoxLabel : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty TargetComboBoxProperty =
            DependencyProperty.Register(nameof(TargetComboBox), typeof(ComboBox), typeof(ComboBoxLabel));

        public System.Windows.Controls.ComboBox TargetComboBox
        {
            get => (System.Windows.Controls.ComboBox)GetValue(TargetComboBoxProperty);
            set => SetValue(TargetComboBoxProperty, value);
        }

        static ComboBoxLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxLabel), new FrameworkPropertyMetadata(typeof(ComboBoxLabel)));
        }

        protected override void OnClick()
        {
            var targetComboBox = TargetComboBox;
            if (targetComboBox == null)
                return;
            targetComboBox.Focus();
            if (targetComboBox.IsEditable)
                return;
            targetComboBox.IsDropDownOpen = !targetComboBox.IsDropDownOpen;
        }
    }
}
