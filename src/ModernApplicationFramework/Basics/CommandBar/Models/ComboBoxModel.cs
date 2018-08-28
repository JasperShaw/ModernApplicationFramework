using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Models
{
    public class ComboBoxModel : INotifyPropertyChanged
    {
        private bool _isEditing;
        private IHasTextProperty _selectedItem;

        public IObservableCollection<IHasTextProperty> Items { get; }

        public IHasTextProperty SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                OnSelectionChanged(value);
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (value == _isEditing) return;
                _isEditing = value;
                OnEditingChanged(value);
                OnPropertyChanged();
            }
        }

        public ComboBoxModel()
        {
            Items = new BindableCollection<IHasTextProperty>();
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public void AddItem(IHasTextProperty item)
        {
            Validate.IsNotNull(item, nameof(item));
            if (Items.Contains(item))
                throw new ArgumentException("Cannot add the same item twice");
            Items.Add(item);
        }

        public virtual bool FilterKey(int virtualKeyCode, bool systemKey = false)
        {
            return false;
        }

        public virtual bool FilterTextInput(char input)
        {
            return false;
        }

        protected virtual void OnSelectionChanged(IHasTextProperty item)
        {
        }

        protected virtual void OnEditingChanged(bool editable)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}