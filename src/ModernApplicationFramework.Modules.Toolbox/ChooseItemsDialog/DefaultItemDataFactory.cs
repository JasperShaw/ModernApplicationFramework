using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public class DefaultItemDataFactory : IItemDataFactory
    {
        private static IItemDataFactory _default;
        public static IItemDataFactory Default => _default ?? (_default = new DefaultItemDataFactory());

        public ItemDataSource Create(ToolboxItemDefinitionBase definition)
        {
            return new ItemDataSource(definition);
        }
    }
}