using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = System.Windows.Controls.ContextMenu;

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

        public override ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

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
        public MenuHostViewModel([ImportMany] MenuBarDefinition[] menubars)
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
            TopLevelDefinitions = new ObservableCollection<CommandBarDefinitionBase>(menubars);
            Build();
        }

        public override void Build()
        {
            Items.Clear();
            IoC.Get<IMainMenuCreator>().CreateMenuBar(this);
        }

        public override void Build(CommandBarDefinitionBase definition)
        {
            BuildLogical(definition);
            if (definition is MenuBarDefinition)
                Build();
            else
            {

                var groups = DefinitionHost.GetSortedGroupsOfDefinition(definition);
                var newItem = IoC.Get<IMainMenuCreator>().CreateMenuItem(definition, groups, DefinitionHost.GetItemsOfGroup);  
                var olditem = Items.FirstOrDefault(x => x.DataContext.Equals(definition));    
                if (olditem == null)
                    Items.Add(newItem);
                else
                {
                    var index = Items.IndexOf(olditem);
                    Items.Remove(olditem);
                    Items.Insert(index, newItem);
                }
            }
        }

        public override void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator)
        {
            base.AddItemDefinition(definition, parent, addAboveSeparator);
            Build(parent);
        }

        public override void DeleteItemDefinition(CommandBarItemDefinition definition)
        {
            base.DeleteItemDefinition(definition);
            Build(definition.Group.Parent);
        }
    }
}