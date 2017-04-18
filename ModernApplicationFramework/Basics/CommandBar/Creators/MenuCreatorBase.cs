using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    public abstract class MenuCreatorBase : CreatorBase
    {
        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition)
        {
            var topItem = GetSingleSubDefinitions(itemDefinition);
            foreach (var item in topItem)
            {
                MenuItem menuItemControl;
                if (item.CommandDefinition is CommandListDefinition)
                    menuItemControl = new DummyListMenuItem(item, itemsControl);
                else
                    menuItemControl = new MenuItem(item);

                if (item is MenuDefinition)
                    CreateRecursive(ref menuItemControl, item);

                itemsControl.Items.Add(menuItemControl);
            }
        }
    }
}