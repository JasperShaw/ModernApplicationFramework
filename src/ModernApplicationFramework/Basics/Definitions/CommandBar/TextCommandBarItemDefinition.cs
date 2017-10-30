using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple text command bar item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasTextProperty" />
    public class TextCommandBarItemDefinition : IHasTextProperty
    {
        private string _text;

        public TextCommandBarItemDefinition(string text)
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}