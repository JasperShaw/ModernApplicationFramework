using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <summary>
    /// A wrapper that holds the items for a <see cref="ComboBoxDataSource"/>. Required to track item changes
    /// </summary>
    public class ComboBoxItemsWrapper
    {
        /// <summary>
        /// Collections of the items
        /// </summary>
        public ObservableCollection<IHasTextProperty> Items { get; set; }

        public ComboBoxItemsWrapper(IEnumerable<IHasTextProperty> colection)
        {
            Items = new ObservableCollection<IHasTextProperty>();
            if (colection == null)
                return;
            foreach (var item in colection)
                Items.Add(item);
            if (colection is INotifyCollectionChanged observable)
                observable.CollectionChanged += Observable_CollectionChanged;
        }

        private void Observable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var eNewItem in e.NewItems)
                {
                    Items.Add(eNewItem as IHasTextProperty);
                }
            if (e.OldItems != null)
                foreach (var eNewItem in e.OldItems)
                {
                    Items.Remove(eNewItem as IHasTextProperty);
                }
        }
    }
}