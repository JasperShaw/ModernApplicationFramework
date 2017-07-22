using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ComboBox
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
}