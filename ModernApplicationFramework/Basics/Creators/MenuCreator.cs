using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Menu;
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
        private readonly MenuDefinition[] _menus;
        private readonly MenuItemGroupDefinition[] _menuItemGroups;

        [ImportingConstructor]
        public MenuCreator(
            ICommandService commandService, 
            [ImportMany] MenuItemDefinition[] menuItems, 
            [ImportMany] ExcludeMenuDefinition[] excludedItems,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuDefinition[] menus)
        {
            _menuItems = menuItems;
            _menus = menus;
            _menuItemGroups = menuItemGroups;
        }


        public void CreateMenu(IMenuHostViewModel model)
        {
            var menus = _menus.OrderBy(x => x.SortOrder);

            foreach (var menu in menus)
            {
                var menuItem = CreateItem(menu);
                AddGroupsRecursive(menu, menuItem);
                model.Items.Add(menuItem);
            }
        }

        private void AddGroupsRecursive(MenuDefinitionBase menu, MenuItem menuItem)
        {
            var groups = _menuItemGroups.Where(x => x.Parent == menu)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = _menuItems.Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition.CommandDefinition, menuItem);
                    else
                        menuItemControl = CreateItemFromDefinition(menuItemDefinition.CommandDefinition);
                    AddGroupsRecursive(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                }
                if (i < groups.Count - 1 && menuItems.Any())
                    menuItem.Items.Add(new Separator());
            }
        }

        //public void CreateMenu(IMenuHostViewModel model)
        //{
        //    model.Items.Clear();
        //    var items = new List<MenuItem>();

        //    var menuDefinitions = _menuItems; //definitions.GetDefinitions();

        //    var topLevelMenus =
        //        menuDefinitions.Where(x => x.HasItems == false)
        //                       .Where(x => x.HasParent == false)
        //                       .Where(x => !_exludedMenus.Contains(x))
        //                       .OrderBy(x => x.Priority);

        //    foreach (var topLevelMenu in topLevelMenus)
        //    {
        //        var topItem = CreateItem(topLevelMenu);
        //        CreateItemsRecursive(topLevelMenu, topItem, menuDefinitions);
        //        items.Add(topItem);
        //    }
        //    foreach (var menuItem in items)
        //        model.Items.Add(menuItem);
        //}

        public MenuItem CreateItem(MenuDefinition definitionOld)
        {
            return new MenuItem { Header = definitionOld.DisplayName };
        }

        /// <summary>
        ///     Creates a MenuItem from CommandDefinition. If the Attached Command is Type GestureCommand the Item will bind the
        ///     Gesture text
        /// </summary>
        /// <param name="definition"></param>
        /// <returns>MenuItem</returns>
        public static MenuItem CreateItemFromDefinition(DefinitionBase definition)
        {
            object vb = null;
            if (!string.IsNullOrEmpty(definition.IconSource?.OriginalString))
            {
                var myResourceDictionary = new ResourceDictionary {Source = definition.IconSource};
                vb = myResourceDictionary[definition.IconId];
            }
            var menuItem = new MenuItem
            {
                Header = definition.Text,
                Icon = vb
            };
            if (!(definition is CommandDefinition commandDefinition))
                return menuItem;
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
            return menuItem;
        }


        //private void CreateItemsRecursive(MenuItemDefinitionOld topLevel, ItemsControl topItem,
        //                                  ICollection<MenuItemDefinitionOld> list)
        //{
        //    //MenuDefinitions which are parent of top
        //    var menuItems = list.Where(x => x.Parent == topLevel)
        //        .Where(x => !_exludedMenus.Contains(x))
        //        .OrderBy(x => x.Priority);

        //    //For some reason 'list' has FixedSize
        //    var tempList = new List<MenuItemDefinitionOld>(list);

        //    for (int i = 0; i < menuItems.Count(); i++)
        //    {
        //        //SubMenuDefinitions which are parent of currnet
        //        var subDefinitonItems =
        //            tempList.Where(x => x.Parent == topLevel)
        //            .Where(x => !_exludedMenus.Contains(x))
        //            .OrderBy(x => x.Priority);
        //        foreach (var subDefinitonItem in subDefinitonItems)
        //        {
        //            tempList.Remove(subDefinitonItem);
        //            if (subDefinitonItem.IsSeparator)
        //                topItem.Items.Add(new Separator());
        //            if (subDefinitonItem.HasItems)
        //                foreach (var definition in subDefinitonItem.Definitions)
        //                {
        //                    if (definition is CommandListDefinition)
        //                        topItem.Items.Add(new DummyListMenuItem(definition ,topItem));
        //                    else if (definition is CommandDefinition commandDefinition)
        //                    {
        //                        if (commandDefinition.CanShowInMenu)
        //                            topItem.Items.Add(CreateItemFromDefinition(commandDefinition));
        //                    }
                                                
        //                }         
        //            else
        //            {
        //                if (subDefinitonItem.IsSeparator)
        //                    continue;
        //                var subItem = CreateItem(subDefinitonItem);
        //                CreateItemsRecursive(subDefinitonItem, subItem, list);
        //                topItem.Items.Add(subItem);
        //            }

        //        }
        //    }
        //}

        public void CreateMenu(IMenuHostViewModel model, MenuItem item)
        {
            CreateMenu(model);
            model.Items.Add(item);
        }
    }
}