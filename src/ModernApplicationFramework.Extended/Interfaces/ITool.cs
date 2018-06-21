using System.Windows.Input;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Special <see cref="T:ModernApplicationFramework.Extended.Interfaces.ILayoutItemBase" /> that's used for anchorable side tools
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Extended.Interfaces.ILayoutItemBase" />
    public interface ITool : ILayoutItemBase, IWindowSearch, IToolbarProvider
    {
        /// <summary>
        /// Command to close the tool
        /// </summary>
        ICommand CloseCommand { get; }

        /// <summary>
        /// Default height in pixels
        /// </summary>
        double PreferredHeight { get; }

        /// <summary>
        /// Default pin location
        /// </summary>
        PaneLocation PreferredLocation { get; }

        /// <summary>
        /// Default width in pixels
        /// </summary>
        double PreferredWidth { get; }

        /// <summary>
        /// Indicates and triggers the tool's visibility
        /// </summary>
        bool IsVisible { get; set; }


        //IWindowSearchHost SearchHost { get; }
    }
}