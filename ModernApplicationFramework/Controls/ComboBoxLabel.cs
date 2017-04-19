using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class ComboBoxLabel : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty TargetComboBoxProperty;

        public ComboBox TargetComboBox
        {
            get => (ComboBox)GetValue(TargetComboBoxProperty);
            set => SetValue(TargetComboBoxProperty, value);
        }

        static ComboBoxLabel()
        {
            TargetComboBoxProperty = DependencyProperty.Register("TargetComboBox", typeof(ComboBox), typeof(ComboBoxLabel));
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
