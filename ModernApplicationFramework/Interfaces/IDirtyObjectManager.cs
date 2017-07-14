using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernApplicationFramework.Interfaces
{
    public interface IDirtyObjectManager : INotifyPropertyChanged
    {
        event PropertyChangedEventHandler IsDirtyChanged;

        bool IsDirty { get; }

        void Clear();

        void SetData(object oldValue,object newValue,  [CallerMemberName] string propertyName = null);
    }
}