using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ComboBox
{
    public class ComboBoxDataSourceEx : CommandBarItemDataSource
    {
        private string _displayedText;
        private string _shortcutText;
        private bool _isEditable = true;
        private bool _isFocused;
        private int _selectionBegin;
        private int _selectionEnd;
        private bool _queryForFocusChange;
        private double _dropDownWidth;

        private ComboBoxModel _model;
        private IHasTextProperty _displayedItem;
        private IObservableCollection<IHasTextProperty> _items;

        private bool _isUpdating;

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

        public ComboBoxModel Model
        {
            get => _model;
            set
            {
                if (Equals(value, _model)) return;
                _model = value;
                OnPropertyChanged();
            }
        }

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

        public IObservableCollection<IHasTextProperty> Items =>
            _items ?? (_items = Model.Items);

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

        public override Guid Id { get; }

        public ComboBoxDataSourceEx(Guid id, string text, uint sortOrder, CommandBarGroupDefinition group, CommandDefinitionBase definition,
            bool visible, bool isChecked, bool isCustom, bool isCustomizable, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) :
            base(text, sortOrder, group, definition, visible, isChecked, isCustom, isCustomizable, flags)
        {
            if (definition is CommandComboBoxDefinition comboBoxDefinition)
            {
                Model = comboBoxDefinition.Model;
                Model.PropertyChanged += ModelOnPropertyChanged;
            }

            Id = id;

            Flags.FilterKeys = true;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isUpdating)
                return;
            switch (e.PropertyName)
            {
                case nameof(Model.SelectedItem):
                    DisplayedItem = Model.SelectedItem;
                    DisplayedText = Model.SelectedItem.Text;
                    break;
                case nameof(Model.IsEditing):
                    IsEditable = Model.IsEditing;
                    break;
            }
        }

        public void ExecuteItem(int index)
        {
            InvokeSetDisplayedItemByIndex(index);
        }

        public void ExecuteItem(string text)
        {
            ExecuteItem(Items.IndexOf(x => x.Text.Equals(text)));
        }

        public void InvokeSetDisplayedItemRelative(int amount)
        {
            var newIndex = SelectedIndex + amount;
            if (newIndex <= 0 && Items.Count > 0)
            {
                InvokeSetDisplayedItemByIndex(0);
                return;
            }
            if (newIndex > Items.Count - 1 && Items.Any())
                InvokeSetDisplayedItemByIndex(Items.Count - 1);
            else InvokeSetDisplayedItemByIndex(newIndex);

        }

        public void Update()
        {
            _isUpdating = true;
            Model.SelectedItem = DisplayedItem;
            Model.IsEditing = IsEditable;
            _isUpdating = false;
        }

        public void InvokeSetDisplayedItemByIndex(int index)
        {
            if (Items.Count < 0 || index < 0)
            {
                DisplayedText = DisplayedItem == null ? string.Empty : DisplayedItem.Text;
                return;
            }

            var item = Items.ElementAtOrDefault(index);
            if (item == null)
            {
                DisplayedItem = null;
                DisplayedText = string.Empty;
            }
            else
            {
                DisplayedItem = item;
                DisplayedText = item.Text;
            }
        }

        private void AssureSelection()
        {
            if (DisplayedItem == null)
            {
                DisplayedText = string.Empty;
                return;
            }
            var item = Items.FirstOrDefault(x => x.Text == DisplayedItem.Text);
            DisplayedItem = item;
            DisplayedText = item?.Text;
        }

        internal bool InvokeFilterEvent(FilterKeyMessages filterMessage, string text, int virtualKeyCode = 0, char input = char.MinValue)
        {
            switch (filterMessage)
            {
                case FilterKeyMessages.GotFocus:
                case FilterKeyMessages.LostFocus:
                    AssureSelection();
                    return false;
                case FilterKeyMessages.CharPressed:
                    return Model.FilterTextInput(input);
                case FilterKeyMessages.KeyDown:
                    return Model.FilterKey(virtualKeyCode);
                case FilterKeyMessages.SystemKeyDown:
                    return Model.FilterKey(virtualKeyCode, true);
            }

            return false;
        }

        internal void InvokeTextChangedEvent(FilterKeyMessages filterMessage, string text)
        {
            switch (filterMessage)
            {
                case FilterKeyMessages.DragDrop:
                case FilterKeyMessages.TextChanged:
                    DisplayedText = text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterMessage), filterMessage, null);
            }
        }
    }

    public class ComboBoxModel : INotifyPropertyChanged
    {
        public static ComboBoxModel Instance = new ComboBoxModel();
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
            Items = new BindableCollection<IHasTextProperty>
            {
                new ComboBoxItemModel("123"),
                new ComboBoxItemModel("456"),
                new ComboBoxItemModel("789"),
            };
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public void AddItem(ComboBoxItemModel item)
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

    public class ComboBoxItemModel : IHasTextProperty
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxItemModel(string text)
        {
            _text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}