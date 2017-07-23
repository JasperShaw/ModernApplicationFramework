using System.Windows;
using ModernApplicationFramework.Controls.TextBoxes;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Core.Events
{
    /// <inheritdoc />
    /// <summary>
    /// Event args when <see cref="F:ModernApplicationFramework.Controls.TextBoxes.TextBox.PreviewTextChangedEvent" /> is fired
    /// </summary>
    /// <seealso cref="T:System.Windows.RoutedEventArgs" />
    public class PreviewTextChangedEventArgs : RoutedEventArgs
    {

        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, TextChangedType type, string text)
            : base(routedEvent)
        {
            Type = type;
            Text = text;
        }

        public PreviewTextChangedEventArgs(RoutedEvent routedEvent, object source, TextChangedType type, string text)
            : base(routedEvent, source)
        {
            Type = type;
            Text = text;
        }

        /// <summary>
        /// The new text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The text changed type
        /// </summary>
        public TextChangedType Type { get; }

    }
}