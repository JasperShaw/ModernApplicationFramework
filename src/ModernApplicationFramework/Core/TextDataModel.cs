using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Core
{
    /// <inheritdoc cref="IHasTextProperty" />
    /// <summary>
    /// Simple text command bar item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasTextProperty" />
    public class TextDataModel : ViewModelBase, IHasTextProperty
    {
        private string _text;

        public TextDataModel(string text)
        {
            _text = text;
        }

        /// <inheritdoc />
        /// <summary>
        /// The localized object text
        /// </summary>
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
    }
}