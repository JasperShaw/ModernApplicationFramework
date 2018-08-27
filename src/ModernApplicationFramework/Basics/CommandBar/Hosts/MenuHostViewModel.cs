using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    /// <inheritdoc cref="IMenuHostViewModel" />
    /// <summary>
    /// Implementation of <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.IMenuHostViewModel" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Hosts.CommandBarHost" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IMenuHostViewModel" />
    [Export(typeof(IMenuHostViewModel))]
    public sealed class MenuHostViewModel : CommandBarHost, IMenuHostViewModel
    {
        private readonly IToolBarHostViewModel _toolBarHost;
        private IMainWindowViewModel _mainWindowViewModel;
        private MenuHostControl _menuHostControl;

        public override ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }

        public ObservableCollection<MenuItem> Items { get; }

        public bool AllowOpenToolBarContextMenu { get; set; } = true;

        public ContextMenu ContextMenu => _toolBarHost?.ContextMenu;

        public IMainWindowViewModel MainWindowViewModel
        {
            get => _mainWindowViewModel;
            set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        internal MenuHostControl MenuHostControl
        {
            get => _menuHostControl;
            set
            {
                if (Equals(_menuHostControl, value))
                    return;
                _menuHostControl = value;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public MenuHostViewModel([Import] ICommandBarDefinitionHost host)
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
            var menuBar = host.ItemDefinitions.FirstOrDefault(x => x.UiType == CommandControlTypes.MenuBar);
            TopLevelDefinitions = new ObservableCollection<CommandBarDataSource>{ menuBar };
            Build();
        }

        public override void Build()
        {
            Items.Clear();
            IoC.Get<IMainMenuCreator>().CreateMenuBar(this);
        }

        public override void Build(CommandBarDataSource definition)
        {
            BuildLogical(definition);
            if (definition is MenuBarDataSource)
                Build();
            else
            {

                var groups = DefinitionHost.GetSortedGroupsOfDefinition(definition);
                var newItem = IoC.Get<IMainMenuCreator>().CreateMenuItem(definition, groups, DefinitionHost.GetItemsOfGroup);
                ReplaceItemWithinList(Items, newItem, item => definition == item.DataContext);
            }
        }

        public override void AddItemDefinition(CommandBarItemDataSource dataSource, CommandBarDataSource parent, bool addAboveSeparator)
        {
            base.AddItemDefinition(dataSource, parent, addAboveSeparator);
            Build(parent);
        }

        public override void DeleteItemDefinition(CommandBarItemDataSource dataSource)
        {
            base.DeleteItemDefinition(dataSource);
            Build(dataSource.Group.Parent);
        }

        private static bool ReplaceItemWithinList<T>(IList items, T newItem, Func<T, bool> func) where T: ItemsControl
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (!(items[i] is T itemsControl))
                    continue;
                if (func(itemsControl))
                {
                    items[i] = newItem;
                    return true;
                }
                if (ReplaceItemWithinList(itemsControl.Items, newItem, func))
                    return true;
            }
            return false;
        }
    }
}