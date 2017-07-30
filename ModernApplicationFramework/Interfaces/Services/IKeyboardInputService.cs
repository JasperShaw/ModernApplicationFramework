using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// An <see cref="IKeyboardInputService"/> allows to hook keyboard input for WPF-Windows
    /// </summary>
    public interface IKeyboardInputService
    {
        /// <summary>
        /// Fires when a key was pressed inside a registered window
        /// </summary>
        event KeyEventHandler PreviewKeyDown;

        //Enables or disables the Keyboard hook for all registered windows
        bool Enabled { get; set; }
             
        /// <summary>
        /// Registers a window the service
        /// </summary>
        /// <param name="window">The window.</param>
        void Register(Window window);

        /// <summary>
        /// Unregisters a window from the service
        /// </summary>
        /// <param name="window">The window.</param>
        void Unregister(Window window);
    }
}