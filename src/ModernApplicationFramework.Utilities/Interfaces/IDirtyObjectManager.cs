using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernApplicationFramework.Utilities.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IDirtyObjectManager" /> handles dirty objects by monitoring their property changes
    /// </summary>
    public interface IDirtyObjectManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Fires when the document's state is changed
        /// </summary>
        event PropertyChangedEventHandler IsDirtyChanged;

        /// <summary>
        /// Indicates whether the object is dirty or not.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Changes the object's state to a clean state
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds property information of the object to the manager. 
        /// </summary>
        /// <param name="oldValue">The original value</param>
        /// <param name="newValue">The new value</param>
        /// <param name="propertyName">The name of the property that was altered</param>
        void SetData(object oldValue,object newValue,  [CallerMemberName] string propertyName = null);
    }
}