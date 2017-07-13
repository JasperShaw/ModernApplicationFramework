using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Core.Utilities
{
    [DataContract]
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T field, T newValue, Action beforeNotifyAction, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            beforeNotifyAction?.Invoke();
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T field, T newValue, Action<T, T> beforeNotifyAction, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            T obj = field;
            field = newValue;
            beforeNotifyAction?.Invoke(obj, newValue);
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty(ref IntPtr field, IntPtr newValue, [CallerMemberName] string propertyName = null)
        {
            if (!(field != newValue))
                return false;
            field = newValue;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty(ref UIntPtr field, UIntPtr newValue, [CallerMemberName] string propertyName = null)
        {
            if (!(field != newValue))
                return false;
            field = newValue;
            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
