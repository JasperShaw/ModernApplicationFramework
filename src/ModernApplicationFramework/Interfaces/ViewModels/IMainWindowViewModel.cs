using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the view model for the main window
    /// </summary>
    public interface IMainWindowViewModel : IWindowViewModel
    {
        /// <summary>
        /// A reference to an instance of <see cref="IMenuHostViewModel"/>
        /// </summary>
        IMenuHostViewModel MenuHostViewModel { get; set; }

        /// <summary>
        /// A reference to an instance of <see cref="IToolBarHostViewModel"/>
        /// </summary>
        IToolBarHostViewModel ToolBarHostViewModel { get; set; }

        /// <summary>
        /// A reference to an instance of <see cref="IInfoBarHost"/>
        /// </summary>
        IInfoBarHost InfoBarHost { get; set; }

        /// <summary>
        /// Option to use a title bar or not
        /// </summary>
        bool UseTitleBar { get; set; }

        /// <summary>
        /// Option to use a menu bar or not
        /// </summary>
        bool UseMenu { get; set; }
    }
}