using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Interfaces.Controls
{
    /// <summary>
    /// A <see cref="IThemableIconContainer"/> contains the required information to have a themable Icon
    /// </summary>
    public interface IThemableIconContainer
    {
        /// <summary>
        /// This is the original source to icon. Usually this is a resource containing a <see cref="Viewbox"/> element.
        /// </summary>
        object IconSource { get; }

        /// <summary>
        /// This is the icon converted from <see cref="IconSource"/>. Usually the object is a <see cref="BitmapSource"/>.
        /// </summary>
        object Icon { get; set; }

        /// <summary>
        /// Indicates whether an element is visually enabled or not
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> if the element is enabled, otherwise <see langword="false" />.
        ///    Default is <see langword="true" />.
        /// </returns>
        bool IsEnabled { get; }
    }
}