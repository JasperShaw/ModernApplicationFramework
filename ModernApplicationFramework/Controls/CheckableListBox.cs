using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls
{
    public sealed class CheckableListBox : ListBox
    {
        public List<Key> ToggleKeys { get; } = new List<Key>();

        public void SelectItem(object item)
        {
            SetSelectedItems(new[]
            {
                item
            });
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckableListBoxItem();
        }
    }
}