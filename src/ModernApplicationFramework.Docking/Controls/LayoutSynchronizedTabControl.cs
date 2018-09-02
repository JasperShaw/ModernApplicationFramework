using System.Collections.Specialized;
using System.Windows.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutSynchronizedTabControl : TabControl
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            LayoutSynchronizer.Update(this);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            LayoutSynchronizer.Update(this);
        }
    }
}