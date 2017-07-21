using System.Windows.Media;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A service that contains the information to show the window icon
    /// </summary>
    public interface IApplicationIconService
    {
        /// <summary>
        /// The vector icon data
        /// </summary>
        Geometry VectorIcon { get; }

        /// <summary>
        /// The brush if the window is active
        /// </summary>
        Brush ActiveColor { get; }

        /// <summary>
        /// The brush if the window in inactive 
        /// </summary>
        Brush InactiveColor { get; }
    }
}