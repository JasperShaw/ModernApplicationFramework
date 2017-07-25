using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc cref="IToolbarCreator" />
    /// <summary>
    /// Implementation of <see cref="IToolbarCreator"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Creators.CreatorBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Utilities.IToolbarCreator" />
    [Export(typeof(IToolbarCreator))]
    public class ToolbarCreator : CreatorBase, IToolbarCreator
    {
        public override void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition)
        {
            var topItem = GetSingleSubDefinitions(itemDefinition);
            foreach (var item in topItem)
            {
                if (itemDefinition is ToolbarDefinition)
                {
                    if (item is MenuDefinition)
                    {
                        var list = new ObservableCollection<CommandBarItemDefinition>();
                        Fill(ref list, item);
                        item.Items = list;
                        itemsControl.Items.Add(new CommandDefinitionButton(item));
                    }
                    else
                        itemsControl.Items.Add(new CommandDefinitionButton(item));
                }
            }
        }

        private void Fill(ref ObservableCollection<CommandBarItemDefinition> list, CommandBarDefinitionBase item)
        {
            var defs = GetSingleSubDefinitions(item);
            foreach (var def in defs)
            {
                list.Add(def);
                if (def is MenuDefinition)
                {
                    var newList = new ObservableCollection<CommandBarItemDefinition>();
                    Fill(ref newList, def);
                    def.Items = newList;
                }
            }
        }
    }
}