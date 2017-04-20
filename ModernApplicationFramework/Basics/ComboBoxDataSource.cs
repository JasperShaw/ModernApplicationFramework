using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Native.Platform;

namespace ModernApplicationFramework.Basics
{
    public class ComboBoxVisualSource : DisposableObject, INotifyPropertyChanged
    {
        private bool _isFocused;
        private int _selectionBegin;
        private int _selectionEnd;
        private bool _queryForFocusChange;
        private double _dropDownWidth;
        private bool _isEditable;

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (value == _isFocused) return;
                _isFocused = value;
                OnPropertyChanged();
            }
        }

        public int SelectionBegin
        {
            get => _selectionBegin;
            set
            {
                if (value == _selectionBegin) return;
                _selectionBegin = value;
                OnPropertyChanged();
            }
        }

        public int SelectionEnd
        {
            get => _selectionEnd;
            set
            {
                if (value == _selectionEnd) return;
                _selectionEnd = value;
                OnPropertyChanged();
            }
        }

        public bool QueryForFocusChange
        {
            get => _queryForFocusChange;
            set
            {
                if (value == _queryForFocusChange) return;
                _queryForFocusChange = value;
                OnPropertyChanged();
            }
        }

        public double DropDownWidth
        {
            get => _dropDownWidth;
            set
            {
                if (value.Equals(_dropDownWidth)) return;
                _dropDownWidth = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                if (value == _isEditable) return;
                _isEditable = value;
                OnPropertyChanged();
            }
        }

        private FlagStorage _flagStorage;

        public virtual FlagStorage Flags => _flagStorage ?? (_flagStorage = new FlagStorage());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ComboBoxDataSource : DisposableObject, INotifyPropertyChanged
    {
        public ComboBoxItemsWrapper Wrapper { get; }
        private object _displayedItem;

        private string _displayedText;

        private string _shortcutText;

        public object DisplayedItem
        {
            get => _displayedItem;
            set
            {
                if (Equals(value, _displayedItem)) return;
                _displayedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<object> Items { get; set; }


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
                return Items.IndexOf(displayedItem);
            }
        }

        public ComboBoxDataSource(ComboBoxItemsWrapper wrapper)
        {
            Wrapper = wrapper;
            Items = wrapper.Items;
        }

        public ComboBoxDataSource(IEnumerable collection)
        {
            Wrapper = new ComboBoxItemsWrapper(collection);
            Items = Wrapper.Items;
        }

        public void ChangeDisplayedItem(int newIndex)
        {
            if (Items.Count -1 < newIndex || newIndex < 0)
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
        public ObservableCollection<object> Items { get; set; }

        public ComboBoxItemsWrapper(IEnumerable colection)
        {
            Items = new ObservableCollection<object>();
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
                    Items.Add(eNewItem);
                }
            if (e.OldItems != null)
                foreach (var eNewItem in e.OldItems)
                {
                    Items.Remove(eNewItem);
                }
        }
    }
}
