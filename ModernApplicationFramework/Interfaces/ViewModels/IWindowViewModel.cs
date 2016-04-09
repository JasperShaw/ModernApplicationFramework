using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IWindowViewModel
    {
        ICommand ChangeWindowIconActiveCommand { get; }
        ICommand ChangeWindowIconPassiveCommand { get; }
        ICommand CloseCommand { get; }
        ICommand MaximizeResizeCommand { get; }
        ICommand MinimizeCommand { get; }
        ICommand SimpleMoveWindowCommand { get; }
        BitmapImage ActiveIcon { get; set; }
        bool IsSimpleWindow { get; set; }
        BitmapImage PassiveIcon { get; set; }
        bool UseSimpleMovement { get; set; }
    }
}