using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces
{
    public interface IHasInternalName : INotifyPropertyChanged
    {
        string InternalName { get; set; }
    }
}