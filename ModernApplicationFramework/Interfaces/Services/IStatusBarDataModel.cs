using System.ComponentModel;
using System.Windows.Media;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// The view-model that controls a Statusbar
    /// </summary>
    public interface IStatusBarDataModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates whether the Statusbar is visible
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Returns the main status text
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The maximal units of the ProgressBar
        /// </summary>
        uint ProgressBarMax { get; }

        /// <summary>
        /// The current Value of the ProgressBar
        /// </summary>
        uint ProgressBarValue { get; }

        /// <summary>
        /// Indicates whether the ProgressBar is visible
        /// </summary>
        bool IsProgressBarActive { get; }

        /// <summary>
        /// The Background color the the StatusBar
        /// </summary>
        Brush Background { get; }

        /// <summary>
        /// The Foreground color the the StatusBar
        /// </summary>
        Brush Foreground { get; }
    }
}