using System.Windows.Input;

namespace ModernApplicationFramework.Controls
{
    public class ComboBoxItem : System.Windows.Controls.ComboBoxItem
    {
        private readonly ComboBox _parent;

        static ComboBoxItem()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxItem),
            //    new FrameworkPropertyMetadata(typeof(ComboBoxItem)));
        }

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