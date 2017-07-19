namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IMainWindowViewModel : IWindowViewModel
    {
        IMenuHostViewModel MenuHostViewModel { get; set; }
        IToolBarHostViewModel ToolBarHostViewModel { get; set; }
        bool UseTitleBar { get; set; }
        bool UseMenu { get; set; }
    }
}