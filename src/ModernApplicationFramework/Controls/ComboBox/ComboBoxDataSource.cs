using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <inheritdoc cref="DisposableObject" />
    /// <summary>
    /// The data model used by a <see cref="ComboBox"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Utilities.DisposableObject" />
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public class ComboBoxDataSource : DisposableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// The item wrapper
        /// </summary>
        public ComboBoxItemsWrapper Wrapper { get; }
        
        private IHasTextProperty _displayedItem;

        private string _displayedText;

        private string _shortcutText;

        /// <summary>
        /// The currently displayed item
        /// </summary>
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

        /// <summary>
        /// The actual item list of the data model
        /// </summary>
        public ObservableCollection<IHasTextProperty> Items { get; set; }


        /// <summary>
        /// The displayed text
        /// </summary>
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

        /// <summary>
        /// The short cut text
        /// </summary>
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

        /// <summary>
        /// The index of the selected item.
        /// </summary>
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

        /// <summary>
        /// Changes the displayed item.
        /// </summary>
        /// <param name="newIndex">The new index.</param>
        public void ChangeDisplayedItem(int newIndex)
        {
            if (Items.Count - 1 < newIndex || newIndex < 0)
                return;
            DisplayedItem = Items.ElementAtOrDefault(newIndex);
        }

        /// <summary>
        /// Changes the displayed item relative to the currently selected index.
        /// </summary>
        /// <param name="index">The index.</param>
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
