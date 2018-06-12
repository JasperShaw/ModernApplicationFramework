using System.Windows.Controls;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// A Provider to get a specific context menu
    /// </summary>
    public interface IContextMenuProvider
    {
        /// <summary>
        /// Gets a <see cref="ContextMenu"/> by a given condition
        /// </summary>
        /// <param name="data">Option do determinate which context menu to return</param>
        /// <returns>Returns the context menu</returns>
        ContextMenu Provide(object data);
    }
}
