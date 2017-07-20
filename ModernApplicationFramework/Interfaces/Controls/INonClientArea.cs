using System.Windows;

namespace ModernApplicationFramework.Interfaces.Controls
{
    /// <summary>
    /// Represents a visual element that may logically contain a non-client Win32 area when WM_NCHITTEST is sent to an HwndSource.
    /// </summary>
    internal interface INonClientArea
    {
        /// <summary>
        /// Given a point, determines what the hit test result should be for WM_NCHITTEST.
        /// </summary>
        /// <param name="point">The point hit, relative to the screen.</param>
        /// <returns>The HT* result representing the non-client hit test result, or HTNOWHERE if the point is not within this element.</returns>
        int HitTest(Point point);
    }
}