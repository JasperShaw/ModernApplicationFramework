using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel, IHasTheme
    {
        MenuHostViewModel MenuHostViewModel { get; set; }
        StatusBar StatusBar { get; set; }
        ToolBarHostViewModel ToolBarHostViewModel { get; set; }
        bool UseStatusBar { get; set; }
        bool UseTitleBar { get; set; }
    }
}