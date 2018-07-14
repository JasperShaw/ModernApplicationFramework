using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    /// <summary>
    /// Default <see cref="IItemDataFactory"/> that creates an <see cref="ItemDataSource"/> instance
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Modules.Toolbox.Interfaces.IItemDataFactory" />
    public class DefaultItemDataFactory : IItemDataFactory
    {
        private static IItemDataFactory _default;
        public static IItemDataFactory Default => _default ?? (_default = new DefaultItemDataFactory());

        /// <summary>
        /// Creates an data source.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>new <see cref="ItemDataSource"/> instance</returns>
        public ItemDataSource Create(ToolboxItemDefinitionBase definition)
        {
            return new ItemDataSource(definition);
        }
    }
}