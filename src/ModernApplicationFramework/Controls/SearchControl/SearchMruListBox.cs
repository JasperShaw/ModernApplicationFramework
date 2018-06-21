using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchMruListBox : ListBox
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SearchMruListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SearchMruListBoxItem;
        }
    }
}
