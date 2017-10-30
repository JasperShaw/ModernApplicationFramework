using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc />
    /// <summary>
    /// A custom list box control where each item has a check box
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ListBox" />
    public sealed class CheckableListBox : ListBox
    {
        /// <summary>
        /// List of keys that can toggle the item
        /// </summary>
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