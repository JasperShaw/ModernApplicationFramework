using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Factory that creates an <see cref="ItemDataSource"/> instance
    /// </summary>
    public interface IItemDataFactory
    {
        /// <summary>
        /// Creates an <see cref="ItemDataSource"/> instance based on a <see cref="ToolboxItemDefinitionBase"/>.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>The <see cref="ItemDataSource"/> instance.</returns>
        ItemDataSource Create(ToolboxItemDefinitionBase definition);
    }
}