using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces
{
    public interface IHasTextProperty : INotifyPropertyChanged
    {
        string Text { get; set; }
    }
}