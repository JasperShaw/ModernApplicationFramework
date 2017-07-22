using System;
using System.Windows;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class HelperMethods
    {
        internal static void HandlePropertyChange<T>(DependencyPropertyChangedEventArgs e, Action<T> attach, Action<T> detach) where T : class
        {
            Validate.IsNotNull(e, "e");
            T oldValue = e.OldValue as T;
            if (oldValue != null)
                detach?.Invoke(oldValue);
            T newValue = e.NewValue as T;
            if (newValue == null || attach == null)
                return;
            attach(newValue);
        }
    }
}
