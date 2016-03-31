using System.Windows.Input;

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
        System.Windows.Media.Imaging.BitmapImage ActiveIcon { get; set; }
        bool IsSimpleWindow { get; set; }
        System.Windows.Media.Imaging.BitmapImage PassiveIcon { get; set; }
        bool UseSimpleMovement { get; set; }
    }
}