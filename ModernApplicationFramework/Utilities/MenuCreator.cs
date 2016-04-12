using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Commands.Service;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using Separator = ModernApplicationFramework.Controls.Separator;

namespace ModernApplicationFramework.Utilities
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        private readonly MenuItemDefinition[] _menuItems;

        [ImportingConstructor]
        public MenuCreator(ICommandService commandService, [ImportMany] MenuItemDefinition[] menuItems)
        {
            _menuItems = menuItems;
        }

        public void CreateMenu(IMenuHostViewModel model)
        {
            model.Items.Clear();
            var items = new List<MenuItem>();

            var menuDefinitions = _menuItems; //definitions.GetDefinitions();

            var topLevelMenus =
                menuDefinitions.Where(x => x.HasItems == false)
                               .Where(x => x.HasParent == false)
                               .OrderBy(x => x.Priority);

            foreach (var topLevel in topLevelMenus)
            {
                var topItem = CreateItem(topLevel);
                CreateItemsRecursive(topLevel, topItem, menuDefinitions);
                items.Add(topItem);
            }
            foreach (var menuItem in items)
                model.Items.Add(menuItem);
        }

        public MenuItem CreateItem(MenuItemDefinition definition)
        {
            return new MenuItem {Header = definition.Name};
        }

        /// <summary>
        ///     Creates a MenuItem from CommandDefinition. If the Attached Command is Type GestureCommand the Item will bind the
        ///     Gesture text
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        protected virtual MenuItem CreateItem(CommandDefinition definition)
        {
            object vb = null;
            if (!string.IsNullOrEmpty(definition.IconSource?.OriginalString))
            {
                var myResourceDictionary = new ResourceDictionary {Source = definition.IconSource};
                vb = myResourceDictionary[definition.IconId];
            }


            var menuItem = new MenuItem
            {
                Header = definition.Name,
                Icon = vb
            };


            var myBindingC = new Binding
            {
                Source = definition,
                Path = new PropertyPath(nameof(definition.Command)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, System.Windows.Controls.MenuItem.CommandProperty, myBindingC);


            var c = definition.Command as GestureCommandWrapper;
            if (c == null)
                return menuItem;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, System.Windows.Controls.MenuItem.InputGestureTextProperty, myBinding);
            return menuItem;
        }


        private void CreateItemsRecursive(MenuItemDefinition topLevel, ItemsControl topItem,
                                          ICollection<MenuItemDefinition> list)
        {
            //MenuDefinitions which are parent of top
            var menuItems = list.Where(x => x.Parent == topLevel).OrderBy(x => x.Priority);

            //For some reason 'list' has FixedSize
            var tempList = new List<MenuItemDefinition>(list);

            foreach (var menusItem in menuItems)
            {
                //SubMenuDefinitions which are parent of currnet
                var subDefinitonItems =
                    tempList.Where(x => x.HasItems == false).Where(x => x.Parent == topLevel).OrderBy(x => x.Priority);
                foreach (var subDefinitonItem in subDefinitonItems)
                {
                    tempList.Remove(subDefinitonItem);
                    if (subDefinitonItem.Name == "Separator")
                    {
                        topItem.Items.Add(new Separator());
                    }
                    else
                    {
                        var subItem = CreateItem(subDefinitonItem);
                        CreateItemsRecursive(subDefinitonItem, subItem, list);
                        topItem.Items.Add(subItem);
                    }
                }

                //Normal Items which can be added directly
                foreach (var definition in menusItem.Definitions)
                    topItem.Items.Add(CreateItem(definition));
            }
        }


        public void CreateMenu(IMenuHostViewModel model, MenuItem item)
        {
            CreateMenu(model);
            model.Items.Add(item);
        }
    }
}