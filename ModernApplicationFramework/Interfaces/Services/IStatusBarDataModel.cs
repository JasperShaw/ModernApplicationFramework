using System.ComponentModel;
using System.Windows.Media;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IStatusBarDataModel : INotifyPropertyChanged
    {
        bool IsVisible { get; }
        string Text { get; }
        uint ProgressBarMax { get; }
        uint ProgressBarValue { get; }
        bool IsProgressBarActive { get; }
        Brush Background { get; }
        Brush Foreground { get; }
    }
}