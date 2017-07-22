using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Utilities.Annotations;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// An implementation of an <see cref="IDirtyObjectManager" /> interface.
    /// </summary>
    /// <seealso cref="IDirtyObjectManager" />
    public class DirtyObjectManager : IDirtyObjectManager
    {
        private readonly ConcurrentDictionary<string, object> _changes = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Fires when the document's state is changed
        /// </summary>
        public event PropertyChangedEventHandler IsDirtyChanged;

        /// <inheritdoc />
        /// <summary>
        /// Indicates whether the object is dirty or not.
        /// </summary>
        public bool IsDirty => _changes.Count > 0;

        /// <inheritdoc />
        /// <summary>
        /// Changes the object's state to a clean state
        /// </summary>
        public void Clear()
        {
            _changes.Clear();
            RaiseDataChanged(string.Empty);
        }

        /// <inheritdoc />
        /// <summary>
        /// Adds property information of the object to the manager.
        /// </summary>
        /// <param name="oldValue">The original value</param>
        /// <param name="newValue">The new value</param>
        /// <param name="propertyName">The name of the property that was altered</param>
        /// <exception cref="T:System.ArgumentException">Unable to add value</exception>
        public void SetData(object oldValue, object newValue,[CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;
            if (_changes.ContainsKey(propertyName))
            {
                if (_changes[propertyName] != newValue)
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
}
