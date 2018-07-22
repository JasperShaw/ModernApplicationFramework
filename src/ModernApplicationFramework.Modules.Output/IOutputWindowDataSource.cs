using System.ComponentModel;
using System.Windows;

namespace ModernApplicationFramework.Modules.Output
{
    internal interface IOutputWindowDataSource : INotifyPropertyChanged
    {
        FrameworkElement ActivePane { get; set; }
    }
}
