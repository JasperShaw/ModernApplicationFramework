using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel, IChangeTheme
    {
        MenuHostViewModel MenuHostViewModel { get; }
        StatusBar StatusBar { get; }
        ToolBarHostViewModel ToolBarHostViewModel { get; }
        Theme Theme { get; set; }
        bool UseStatusBar { get; set; }
        bool UseTitleBar { get; set; }
    }
}