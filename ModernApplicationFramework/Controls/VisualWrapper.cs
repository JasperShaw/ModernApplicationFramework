using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    [ContentProperty("Child")]
    public class VisualWrapper : FrameworkElement
    {
        private Visual _child;

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