using System.Windows.Input;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the view model for a window
    /// </summary>
    public interface IWindowViewModel
    {
        /// <summary>
        /// The command to close the window
        /// </summary>
        ICommand CloseCommand { get; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        ICommand MaximizeResizeCommand { get; }

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        ICommand MinimizeCommand { get; }

        /// <summary>
        /// The command to handle the simple window movement when there is no Non-Client area
        /// </summary>
        ICommand SimpleMoveWindowCommand { get; }

        /// <summary>
        /// Option to disable the Non-Client area completely. 
        /// </summary>
        bool IsSimpleWindow { get; set; }

        /// <summary>
        /// Option to use the simple movement command
        /// </summary>
        bool UseSimpleMovement { get; set; }
    }
}