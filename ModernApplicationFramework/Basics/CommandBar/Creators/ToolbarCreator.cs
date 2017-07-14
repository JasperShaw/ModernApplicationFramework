using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : CreatorBase, IToolbarCreator
    {
        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition)
        {
            var topItem = GetSingleSubDefinitions(itemDefinition);
            foreach (var item in topItem)
                if (itemDefinition is ToolbarDefinition)
                {
                    if (item is MenuDefinition)
                        CreateRecursive(ref itemsControl, item);
                    itemsControl.Items.Add(new CommandDefinitionButton(item));
                }
        }
    }
}