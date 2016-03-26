using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel, IHasTheme
    {
        MenuHostViewModel MenuHostViewModel { get; }
        StatusBar StatusBar { get; }
        ToolBarHostViewModel ToolBarHostViewModel { get; }
        bool UseStatusBar { get; set; }
        bool UseTitleBar { get; set; }
    }
}