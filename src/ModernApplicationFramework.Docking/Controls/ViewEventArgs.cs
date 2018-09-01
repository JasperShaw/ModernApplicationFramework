using System;
using System.Windows;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class ViewEventArgs : RoutedEventArgs
    {
        public ViewEventArgs(RoutedEvent evt, LayoutContent view)
            : base(evt)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
        }

        public LayoutContent View { get; }
    }
}