using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel, IHasTheme
    {
        IMenuHostViewModel MenuHostViewModel { get; set; }
        StatusBar StatusBar { get; set; }
        IToolBarHostViewModel ToolBarHostViewModel { get; set; }
        bool UseStatusBar { get; set; }
        bool UseTitleBar { get; set; }
        bool UseMenu { get; set; }
    }
}