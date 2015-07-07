using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ModernApplicationFramework.ViewModels
{

    //Based on this: http://danrigby.com/2012/04/01/inotifypropertychanged-the-net-4-5-way-revisited/
    public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true; 
        }

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
