using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc />
    /// <summary>
    /// Basic implementation to create menus
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Creators.CreatorBase" />
    public abstract class MenuCreatorBase : CreatorBase
    {
        /// <summary>
        /// Creates a sub-tree of an <see cref="T:System.Windows.Controls.ItemsControl" /> recursively
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:System.Windows.Controls.ItemsControl" /></typeparam>
        /// <param name="itemsControl">The <see cref="T:System.Windows.Controls.ItemsControl" /> that should be filled</param>
        /// <param name="itemDefinition">The data model of the current item</param>
        /// <inheritdoc />
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