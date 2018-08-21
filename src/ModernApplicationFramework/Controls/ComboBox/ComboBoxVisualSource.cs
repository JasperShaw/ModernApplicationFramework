using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ComboBox
{
    /// <inheritdoc cref="DisposableObject" />
    /// <summary>
    /// The visual data model for a <see cref="T:ModernApplicationFramework.Controls.ComboBox.ComboBox" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Utilities.DisposableObject" />
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public class ComboBoxVisualSource : DisposableObject, INotifyPropertyChanged
    {
        private bool _isFocused;
        private int _selectionBegin;
        private int _selectionEnd;
        private bool _queryForFocusChange;
        private double _dropDownWidth;
        private bool _isEditable;

        /// <summary>
        /// Indicates whether the combo box is focused.
        /// </summary>
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

        /// <summary>
        /// The width of the drop down.
        /// </summary>
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

        /// <summary>
        /// Option to make a <see cref="ComboBox"/> editable
        /// </summary>
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

        private FlagsDataSource _flagsDataSource;

        /// <summary>
        /// The flag store
        /// </summary>
        public virtual FlagsDataSource Flags => _flagsDataSource ?? (_flagsDataSource = new FlagsDataSource());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}