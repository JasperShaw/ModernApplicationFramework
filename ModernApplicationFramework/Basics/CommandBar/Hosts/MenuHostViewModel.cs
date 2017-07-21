using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

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

        public ICommand RightClickCommand => new Command(ExecuteRightClick);

        public bool AllowOpenToolBarContextMenu { get; set; } = true;

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
                var menuItem = IoC.Get<IMainMenuCreator>().CreateMenuItem(definition);
                if (!Items.Contains(menuItem))
                    Items.Add(menuItem);
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


        private void ExecuteRightClick()
        {
            if (_toolBarHost == null)
                return;

            if (AllowOpenToolBarContextMenu && _toolBarHost.TopLevelDefinitions.Any())
                _toolBarHost.OpenContextMenuCommand.Execute(null);
        }

        private void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightClickCommand.Execute(null);
        }
    }
}