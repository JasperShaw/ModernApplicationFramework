using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Native.Platform;

namespace ModernApplicationFramework.Basics
{
    public class ComboBoxDataSource : DisposableObject, INotifyPropertyChanged
    {
        private object _displayedItem;
        private double _dropDownWidth;
        private bool _isEditable;
        private string _displayedText;
        private bool _isFocused;
        private int _selectionBegin;
        private int _selectionEnd;
        private bool _queryForFocusChange;
        private string _shortcutText;
        private int _selectedIndex;

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
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }


        public void ChangeDisplayedItem(int newIndex)
        {
            if (Items.Count -1 < newIndex || newIndex < 0)
                return;
            SelectedIndex = newIndex;
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
}
