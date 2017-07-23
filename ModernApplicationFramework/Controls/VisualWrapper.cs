using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A wrapper that holds a <see cref="T:System.Windows.Media.Visual" /> as its child element
    /// </summary>
    /// <seealso cref="T:System.Windows.FrameworkElement" />
    [ContentProperty("Child")]
    public class VisualWrapper : FrameworkElement
    {
        private Visual _child;

        /// <summary>
        /// The child.
        /// </summary>
        public Visual Child
        {
            get => _child;
            set
            {
                if (Equals(_child, value))
                    return;
                if (_child != null)
                    RemoveVisualChild(_child);
                _child = value;
                if (_child == null)
                    return;
                AddVisualChild(_child);
            }
        }

        protected override int VisualChildrenCount => _child == null ? 0 : 1;

        protected override Visual GetVisualChild(int index)
        {
            if (_child == null || index != 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _child;
        }
    }
}