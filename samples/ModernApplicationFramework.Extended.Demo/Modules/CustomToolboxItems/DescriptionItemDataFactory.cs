using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.CustomToolboxItems
{
    public class DescriptionItemDataFactory : IItemDataFactory
    {
        private static IItemDataFactory _default;
        public static IItemDataFactory Default => _default ?? (_default = new DescriptionItemDataFactory());

        public ItemDataSource Create(ToolboxItemDefinitionBase definition)
        {
            return new DescriptionDataSource((DescriptionToolboxItemDefinition) definition);
        }
    }
}