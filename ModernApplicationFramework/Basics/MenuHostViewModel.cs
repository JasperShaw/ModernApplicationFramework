using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics
{
    [Export(typeof(IMenuHostViewModel))]
    public class MenuHostViewModel : ViewModelBase, IMenuHostViewModel
    {
        private readonly IToolBarHostViewModel _toolBarHost;
        private IMainWindowViewModel _mainWindowViewModel;
        private MenuHostControl _menuHostControl;

        public ObservableCollection<MenuBarDefinition> MenuBars { get; }
        //public ObservableCollectionEx<MenuDefinition> MenuDefinitions { get; }
        public ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        public ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }
        public ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        internal MenuHostControl MenuHostControl
        {
            get => _menuHostControl;
            set
            {
                if (_menuHostControl != null)
                    _menuHostControl.MouseRightButtonDown -= _control_MouseRightButtonDown;
                _menuHostControl = value;
                _menuHostControl.MouseRightButtonDown += _control_MouseRightButtonDown;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public MenuHostViewModel(
            [ImportMany] MenuBarDefinition[] menubars,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] CommandBarGroupDefinition[] menuItemGroups,
            [ImportMany] CommandBarItemDefinition[] menuItems,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems)
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
            MenuBars = new ObservableCollection<MenuBarDefinition>(menubars);
            ItemGroupDefinitions = new ObservableCollection<CommandBarGroupDefinition>();
            foreach (var menuDefinition in menuItemGroups)
                ItemGroupDefinitions.Add(menuDefinition);
            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>();
            foreach (var menuDefinition in menuItems)
                ItemDefinitions.Add(menuDefinition);
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);
            

            MenuBars.CollectionChanged += OnCollectionChanged;
            ItemGroupDefinitions.CollectionChanged += OnCollectionChanged;
            ItemDefinitions.CollectionChanged += OnCollectionChanged;
            ExcludedItemDefinitions.CollectionChanged += OnCollectionChanged;
            Build();
        }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        ///     Default is true
        /// </summary>
        public bool AllowOpenToolBarContextMenu { get; set; } = true;

        /// <summary>
        ///     Contains the Items of the MenuHostControl
        /// </summary>
        public ObservableCollection<MenuItem> Items { get; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        public ICommand RightClickCommand => new Command(ExecuteRightClick);

        public void Build()
        {
            IoC.Get<IMenuCreator>().CreateMenuBar(this);
        }

        public IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions()
        {
            var list = new List<CommandBarDefinitionBase>();
            IEnumerable<CommandBarDefinitionBase> barDefinitions = MenuBars.OrderBy(x => x.SortOrder).ToList();

            foreach (var barDefinition in barDefinitions)
            {
                list.Add(barDefinition);
                list.AddRange(GetSubHeaderMenus(barDefinition));
            }
            return list;
        }

        public void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator)
        {
            if (!addAboveSeparator)
            {
                var definitionsToChange =
                    ItemDefinitions.Where(
                            x => x.Group == definition.Group)
                        .OrderBy(x => x.SortOrder);

                foreach (var definitionToChange in definitionsToChange)
                {
                    if (definitionToChange.Group != definition.Group)
                        continue;
                    if (definitionToChange.SortOrder >= definition.SortOrder)
                        definitionToChange.SortOrder++;
                }
            }
            ItemDefinitions.Add(definition);
        }

        private IEnumerable<CommandBarDefinitionBase> GetSubHeaderMenus(CommandBarDefinitionBase definition)
        {
            var group = ItemGroupDefinitions.FirstOrDefault(x => x.Parent == definition);
            var list = new List<CommandBarDefinitionBase>();
            var headerMenus = ItemDefinitions.Where(x => x.CommandDefinition is MenuHeaderCommandDefinition)
                .Where(x => x.Group == group).OrderBy(x => x.SortOrder);

            foreach (var headerMenu in headerMenus)
            {
                list.Add(headerMenu);
                list.AddRange(GetSubHeaderMenus(headerMenu));
            }
            return list;
        }

        protected virtual void ExecuteRightClick()
        {
            if (_toolBarHost == null)
                return;

            if (AllowOpenToolBarContextMenu && _toolBarHost.ToolbarDefinitions.Any())
                _toolBarHost.OpenContextMenuCommand.Execute(null);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Build();
        }

        private void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClickCommand.Execute(null);
        }
    }
}