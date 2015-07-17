using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.ViewModels
{
    public interface IWindowViewModel
    {
        Command ChangeWindowIconActiveCommand { get; }
        Command ChangeWindowIconPassiveCommand { get; }
        Command CloseCommand { get; }
        Command MaximizeResizeCommand { get; }
        Command MinimizeCommand { get; }
        Command SimpleMoveWindowCommand { get; }
        System.Windows.Media.Imaging.BitmapImage ActiveIcon { get; set; }
        bool IsSimpleWindow { get; set; }
        System.Windows.Media.Imaging.BitmapImage PassiveIcon { get; set; }
        bool UseSimpleMovement { get; set; }
    }
}