using ModernApplicationFramework.Controls.Primitives;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel
    {
        IMenuHostViewModel MenuHostViewModel { get; set; }
        StatusBar StatusBar { get; set; }
        IToolBarHostViewModel ToolBarHostViewModel { get; set; }
        bool UseStatusBar { get; set; }
        bool UseTitleBar { get; set; }
        bool UseMenu { get; set; }
    }
}