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
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    [Export(typeof(IMenuHostViewModel))]
    public sealed class MenuHostViewModel : CommandBarHost, IMenuHostViewModel
    {
        private readonly IToolBarHostViewModel _toolBarHost;
        private IMainWindowViewModel _mainWindowViewModel;
        private MenuHostControl _menuHostControl;

        public ObservableCollection<MenuBarDefinition> MenuBars { get; }

        /// <summary>
        ///     Contains the Items of the MenuHostControl
        /// </summary>
        public ObservableCollection<MenuItem> Items { get; }

        public ICommand RightClickCommand => new Command(ExecuteRightClick);

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        ///     Default is true
        /// </summary>
        public bool AllowOpenToolBarContextMenu { get; set; } = true;

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
        public MenuHostViewModel([ImportMany] MenuBarDefinition[] menubars)
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
            MenuBars = new ObservableCollection<MenuBarDefinition>(menubars);

            MenuBars.CollectionChanged += OnCollectionChanged;
            DefinitionHost.ItemGroupDefinitions.CollectionChanged += OnCollectionChanged;
            DefinitionHost.ItemDefinitions.CollectionChanged += OnCollectionChanged;
            DefinitionHost.ExcludedItemDefinitions.CollectionChanged += OnCollectionChanged;
            Build();
        }

        public override void Build()
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

        private IEnumerable<CommandBarDefinitionBase> GetSubHeaderMenus(CommandBarDefinitionBase definition)
        {
            var group = DefinitionHost.ItemGroupDefinitions.FirstOrDefault(x => x.Parent == definition);
            var list = new List<CommandBarDefinitionBase>();
            var headerMenus = DefinitionHost.ItemDefinitions.Where(x => x is MenuDefinition)
                .Where(x => x.Group == group)
                .OrderBy(x => x.SortOrder);

            foreach (var headerMenu in headerMenus)
            {
                list.Add(headerMenu);
                list.AddRange(GetSubHeaderMenus(headerMenu));
            }
            return list;
        }

        private void ExecuteRightClick()
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