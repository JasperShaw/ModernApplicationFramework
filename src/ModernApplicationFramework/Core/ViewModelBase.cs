using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernApplicationFramework.Core
{
    //Based on this: http://danrigby.com/2012/04/01/inotifypropertychanged-the-net-4-5-way-revisited/
    /// <inheritdoc />
    /// <summary>
    /// The foundation of a view model implementing the <see cref="T:System.ComponentModel.INotifyPropertyChanged" /> interface
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}