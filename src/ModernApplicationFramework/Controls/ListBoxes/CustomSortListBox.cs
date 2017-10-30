using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc cref="ListBox" />
    /// <summary>
    /// Special list box control with a custom <see cref="T:System.Collections.IComparer" /> for sorting the items
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ListBox" />
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public class CustomSortListBox : ListBox, INotifyPropertyChanged
    {

        private PropertyChangedEventHandler _propertyChanged;

        /// <summary>
        /// The Instance handling the sorting
        /// </summary>
        public IComparer CustomSort { get; set; }

        public UIElement FocusableElement
        {
            get
            {
                UIElement uiElement = null;
                if (SelectedIndex != -1)
                    uiElement = ItemContainerGenerator.ContainerFromItem(SelectedItem) as ListBoxItem;
                return uiElement ?? this;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                var changedEventHandler = _propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref _propertyChanged, (PropertyChangedEventHandler)Delegate.Combine(comparand, value), comparand);
                } while (changedEventHandler != comparand);
            }
            remove
            {
                var changedEventHandler = _propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange(ref _propertyChanged, (PropertyChangedEventHandler)Delegate.Remove(comparand, value), comparand);
                } while (changedEventHandler != comparand);
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (CustomSort != null)
            {
                var defaultView = CollectionViewSource.GetDefaultView(newValue) as ListCollectionView;
                if (defaultView != null && !Equals(defaultView.CustomSort, CustomSort))
                    defaultView.CustomSort = CustomSort;
            }
            if (HasItems)
            {
                SelectedIndex = 0;
                ScrollIntoView(SelectedItem);
            }
            OnPropertyChanged(nameof(FocusableElement));
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        public CustomSortListBox()
        {
            Loaded += CustomSortListBox_Loaded;
        }

        private void CustomSortListBox_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(FocusableElement));
            Loaded -= CustomSortListBox_Loaded;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            OnPropertyChanged(nameof(FocusableElement));
        }

        protected void OnPropertyChanged(string name)
        {
            _propertyChanged.RaiseEvent(this, name);
        }
    }
}
