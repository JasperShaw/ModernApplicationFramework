using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ComboBox
{
    public class ComboBoxDataSource : DisposableObject, INotifyPropertyChanged
    {
        public ComboBoxItemsWrapper Wrapper { get; }
        private IHasTextProperty _displayedItem;

        private string _displayedText;

        private string _shortcutText;

        public IHasTextProperty DisplayedItem
        {
            get => _displayedItem;
            set
            {
                if (Equals(value, _displayedItem)) return;
                _displayedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IHasTextProperty> Items { get; set; }


        public string DisplayedText
        {
            get => _displayedText;
            set
            {
                if (value == _displayedText) return;
                _displayedText = value;
                OnPropertyChanged();
            }
        }

        public string ShortcutText
        {
            get => _shortcutText;
            set
            {
                if (value == _shortcutText) return;
                _shortcutText = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get
            {
                var displayedItem = DisplayedItem;
                if (displayedItem == null)
                    return -1;
                return Items.IndexOf(displayedItem as IHasTextProperty);
            }
        }

        public ComboBoxDataSource(ComboBoxItemsWrapper wrapper)
        {
            Wrapper = wrapper;
            Items = wrapper.Items;
        }

        public ComboBoxDataSource(IEnumerable<IHasTextProperty> collection)
        {
            Wrapper = new ComboBoxItemsWrapper(collection);
            Items = Wrapper.Items;
        }

        public void ChangeDisplayedItem(int newIndex)
        {
            if (Items.Count - 1 < newIndex || newIndex < 0)
                return;
            DisplayedItem = Items.ElementAtOrDefault(newIndex);
        }

        public void ChangeDisplayedItemRelative(int index)
        {
            var newIndex = SelectedIndex + index;

            if (newIndex < 0)
                newIndex = 0;
            if (newIndex > Items.Count - 1)
                newIndex = Items.Count - 1;
            ChangeDisplayedItem(newIndex);
        }


        protected override void DisposeManagedResources()
        {
            Items.Clear();
            base.DisposeManagedResources();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateItems()
        {
            OnPropertyChanged(nameof(Items));
        }
    }

    public class ComboBoxItemsWrapper
    {
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
