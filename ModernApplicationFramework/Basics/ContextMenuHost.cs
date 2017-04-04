using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics
{

    [Export(typeof(IContextMenuHost))]
    public class ContextMenuHost : IContextMenuHost
    {
        public ObservableCollectionEx<ContextMenuDefinition> ContextMenuDefinitions { get; }
        public ObservableCollectionEx<MenuItemGroupDefinition> MenuItemGroupDefinitions { get; }
        public ObservableCollectionEx<MenuItemDefinition> MenuItemDefinitions { get; }
        public ObservableCollection<CommandBarDefinitionBase> ExcludedContextMenuElementDefinitions { get; }

        private readonly Dictionary<ContextMenuDefinition, Controls.ContextMenu> _hostedContextMenus;

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] ContextMenuDefinition[] contextMenuDefinitions,
            [ImportMany] MenuItemGroupDefinition[] menuGroupDefinitions,
            [ImportMany] MenuItemDefinition[] menuItemDefinitions,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems)
        {
            _hostedContextMenus = new Dictionary<ContextMenuDefinition, Controls.ContextMenu>();

            ContextMenuDefinitions = new ObservableCollectionEx<ContextMenuDefinition>();
            foreach (var menuDefinition in contextMenuDefinitions)
                ContextMenuDefinitions.Add(menuDefinition);
            MenuItemGroupDefinitions = new ObservableCollectionEx<MenuItemGroupDefinition>();
            foreach (var menuDefinition in menuGroupDefinitions)
                MenuItemGroupDefinitions.Add(menuDefinition);
            MenuItemDefinitions = new ObservableCollectionEx<MenuItemDefinition>();
            foreach (var menuDefinition in menuItemDefinitions)
                MenuItemDefinitions.Add(menuDefinition);
            ExcludedContextMenuElementDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedContextMenuElementDefinitions.Add(item.ExcludedCommandBarDefinition);


            ContextMenuDefinitions.CollectionChanged += CreateNewMenu;
            MenuItemGroupDefinitions.CollectionChanged += UpdateMenu;
            MenuItemDefinitions.CollectionChanged += UpdateMenu;
            ExcludedContextMenuElementDefinitions.CollectionChanged += UpdateMenu;

            CreateAllContextMenus();
        }

        public void CreateAllContextMenus()
        {
            _hostedContextMenus.Clear();
            foreach (var definition in ContextMenuDefinitions.Where(x => !ExcludedContextMenuElementDefinitions.Contains(x)))
            {
                var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(this, definition);
                _hostedContextMenus.Add(definition, contextMenu);
            }
        }

        public Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition)
        {
            if (!_hostedContextMenus.TryGetValue(contextMenuDefinition, out Controls.ContextMenu contextMenu))
                throw new ArgumentException(contextMenuDefinition.Text);
            return contextMenu;
        }

        private void UpdateMenu(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void CreateNewMenu(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
