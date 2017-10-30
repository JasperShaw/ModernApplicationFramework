using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// Interface the holds a reference the main window data model
    /// </summary>
    public interface IHasMainWindowViewModel
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }
    }
}
