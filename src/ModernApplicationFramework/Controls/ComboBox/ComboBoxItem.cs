using System.Windows.Input;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <inheritdoc />
    /// <summary>
    /// An item control used for a <see cref="T:ModernApplicationFramework.Controls.ComboBox.ComboBox" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ComboBoxItem" />
    public class ComboBoxItem : System.Windows.Controls.ComboBoxItem
    {
        private readonly ComboBox _parent;

        public ComboBoxItem(ComboBox parent)
        {
            _parent = parent;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            HandleSelection();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                HandleSelection();
                e.Handled = true;
            }
            else
                base.OnKeyDown(e);
        }

        private void HandleSelection()
        {
            _parent.HandleComboSelection(true, _parent.ItemContainerGenerator.IndexFromContainer(this));
        }
    }
}