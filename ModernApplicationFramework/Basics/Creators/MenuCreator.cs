using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using Binding = System.Windows.Data.Binding;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.DefinitionBase;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using Separator = ModernApplicationFramework.Controls.Separator;

namespace ModernApplicationFramework.Basics.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        private readonly MenuItemDefinition[] _menuItems;
        private readonly MenuItemDefinition[] _exludedMenus;

        [ImportingConstructor]
        public MenuCreator(ICommandService commandService, [ImportMany] MenuItemDefinition[] menuItems, [ImportMany] ExcludeMenuDefinition[] excludedItems)
        {
            _menuItems = menuItems;
            _exludedMenus = excludedItems.Select(x => x.ExludedMenuItemDefinition).ToArray();
        }

        public void CreateMenu(IMenuHostViewModel model)
        {
            model.Items.Clear();
            var items = new List<MenuItem>();

            var menuDefinitions = _menuItems; //definitions.GetDefinitions();

            var topLevelMenus =
                menuDefinitions.Where(x => x.HasItems == false)
                               .Where(x => x.HasParent == false)
                               .Where(x => !_exludedMenus.Contains(x))
                               .OrderBy(x => x.Priority);

            foreach (var topLevelMenu in topLevelMenus)
            {
                var topItem = CreateItem(topLevelMenu);
                CreateItemsRecursive(topLevelMenu, topItem, menuDefinitions);
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
        protected virtual MenuItem CreateItem(DefinitionBase definition)
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


            if (definition is CommandDefinition commandDefinition)
            {
                var myBindingC = new Binding
                {
                    Source = definition,
                    Path = new PropertyPath(nameof(commandDefinition.Command)),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(menuItem, System.Windows.Controls.MenuItem.CommandProperty, myBindingC);

                var c = commandDefinition.Command as GestureCommandWrapper;
                if (c == null)
                    return menuItem;

                var myBinding = new Binding
                {
                    Source = c,
                    Path = new PropertyPath(nameof(c.GestureText)),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(menuItem, System.Windows.Controls.MenuItem.InputGestureTextProperty,
                    myBinding);

            }       
            return menuItem;
        }


        private void CreateItemsRecursive(MenuItemDefinition topLevel, ItemsControl topItem,
                                          ICollection<MenuItemDefinition> list)
        {
            //MenuDefinitions which are parent of top
            var menuItems = list.Where(x => x.Parent == topLevel)
                .Where(x => !_exludedMenus.Contains(x))
                .OrderBy(x => x.Priority);

            //For some reason 'list' has FixedSize
            var tempList = new List<MenuItemDefinition>(list);

            for (int i = 0; i < menuItems.Count(); i++)
            {
                //SubMenuDefinitions which are parent of currnet
                var subDefinitonItems =
                    tempList.Where(x => x.Parent == topLevel)
                    .Where(x => !_exludedMenus.Contains(x))
                    .OrderBy(x => x.Priority);
                foreach (var subDefinitonItem in subDefinitonItems)
                {
                    tempList.Remove(subDefinitonItem);
                    if (subDefinitonItem.IsSeparator)
                        topItem.Items.Add(new Separator());
                    if (subDefinitonItem.HasItems)
                        foreach (var definition in subDefinitonItem.Definitions)
                        {
                            if (definition is CommandListDefinition)
                            {
                                topItem.Items.Add(new CommandMenuItem(definition ,topItem));

                            }
                            else
                            {
                                topItem.Items.Add(CreateItem(definition));
                            }                           
                        }         
                    else
                    {
                        if (subDefinitonItem.IsSeparator)
                            continue;
                        var subItem = CreateItem(subDefinitonItem);
                        CreateItemsRecursive(subDefinitonItem, subItem, list);
                        topItem.Items.Add(subItem);
                    }

                }
            }
        }


        public void CreateMenu(IMenuHostViewModel model, MenuItem item)
        {
            CreateMenu(model);
            model.Items.Add(item);
        }
    }
}