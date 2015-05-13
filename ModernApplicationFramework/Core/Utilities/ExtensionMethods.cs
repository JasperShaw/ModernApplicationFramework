using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class ExtensionMethods
    {
        public static void RaiseEvent(this EventHandler eventHandler, object source)
        {
            RaiseEvent(eventHandler, source, EventArgs.Empty);
        }
        public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
        {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
        }

        public static bool IsConnectedToPresentationSource(this DependencyObject obj)
        {
            return PresentationSource.FromDependencyObject(obj) != null;
        }

        public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
        {
            if (sourceElement == null)
                return null;
            if (sourceElement is Visual)
                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
            return LogicalTreeHelper.GetParent(sourceElement);
        }
    }
}
