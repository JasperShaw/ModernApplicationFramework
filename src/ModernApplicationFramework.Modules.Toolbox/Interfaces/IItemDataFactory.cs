using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IItemDataFactory
    {
        ItemDataSource Create(ToolboxItemDefinitionBase definition);
    }
}