using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics
{
    public class DirtyObjectManager : IDirtyObjectManager
    {

        private readonly ConcurrentDictionary<string, object> _changes = new ConcurrentDictionary<string, object>();

        public event PropertyChangedEventHandler IsDirtyChanged;
        public bool IsDirty => _changes.Count > 0;
        public void Clear()
        {
            _changes.Clear();
            RaiseDataChanged(string.Empty);

        }

        public void SetData(object oldValue, object newValue,[CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;
            if (_changes.ContainsKey(propertyName))
            {
                if (!_changes[propertyName].Equals(newValue))
                    return;
                _changes.TryRemove(propertyName, out object _);
                RaiseDataChanged(propertyName);
            }
            else
            {
                if (!_changes.TryAdd(propertyName, oldValue))
                    throw new ArgumentException("Unable to add value");
                RaiseDataChanged(propertyName);
            }
        }

        private void RaiseDataChanged(string propertyName)
        {
            IsDirtyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(IsDirty));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface IDirtyObjectManager : INotifyPropertyChanged
    {
        event PropertyChangedEventHandler IsDirtyChanged;

        bool IsDirty { get; }

        void Clear();

        void SetData(object oldValue,object newValue,  [CallerMemberName] string propertyName = null);
    }
}
