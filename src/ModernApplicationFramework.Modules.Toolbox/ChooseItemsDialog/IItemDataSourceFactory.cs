using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public interface IItemDataFactory
    {
        ItemDataSource Create(ToolboxItemDefinitionBase definition);
    }
}
