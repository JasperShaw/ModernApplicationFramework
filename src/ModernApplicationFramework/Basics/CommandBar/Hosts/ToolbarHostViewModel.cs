using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities;
using ContextMenu = ModernApplicationFramework.Controls.Menu.ContextMenu;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    /// <inheritdoc cref="IToolBarHostViewModel" />
    /// <summary>
    /// Implementation of <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarHostViewModel" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Hosts.CommandBarHost" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarHostViewModel" />
    [Export(typeof(IToolBarHostViewModel))]
    internal sealed class ToolbarHostViewModel : CommandBarHost, IToolBarHostViewModelInternal
    {
        private readonly Dictionary<ToolBarDataSource, ToolBar> _toolbars;
        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        public override ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }

        public ContextMenu ContextMenu { get; }

        public ICommand OpenContextMenuCommand => new DelegateCommand(o => OpenContextMenu(), p => CanOpenContextMenu());

        public IMainWindowViewModel MainWindowViewModel { get; set; }

        public ToolbarScope ToolbarScope => ToolbarScope.MainWindow;

        public ToolBarTray BottomToolBarTray
        {
            get => _bottomToolBarTay;
            set
            {
                _bottomToolBarTay = value;
                _bottomToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
                _bottomToolBarTay.MouseLeftButtonDown += _ToolBarTay_MouseLeftButtonDown;
            }
        }

        public ToolBarTray LeftToolBarTray
        {
            get => _leftToolBarTay;
            set
            {
                _leftToolBarTay = value;
                _leftToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
                _leftToolBarTay.MouseLeftButtonDown += _ToolBarTay_MouseLeftButtonDown;
            }
        }

        public ToolBarTray RightToolBarTray
        {
            get => _rightToolBarTay;
            set
            {
                _rightToolBarTay = value;
                _rightToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
                _rightToolBarTay.MouseLeftButtonDown += _ToolBarTay_MouseLeftButtonDown;
            }
        }

        public ToolBarTray TopToolBarTray
        {
            get => _topToolBarTay;
            set
            {
                _topToolBarTay = value;
                _topToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
                _topToolBarTay.MouseLeftButtonDown += _ToolBarTay_MouseLeftButtonDown;
            }
        }

        [ImportingConstructor]
        public ToolbarHostViewModel([Import] ICommandBarDefinitionHost host)
        {
            _toolbars = new Dictionary<ToolBarDataSource, ToolBar>();

            TopLevelDefinitions = new ObservableCollectionEx<CommandBarDataSource>();
            foreach (var definition in host.ItemDefinitions.Where(x => x.UiType == CommandControlTypes.Toolbar && ((ToolBarDataSource) x).ToolbarScope == ToolbarScope))
                TopLevelDefinitions.Add(definition);

            ((ObservableCollectionEx<CommandBarDataSource>) TopLevelDefinitions).CollectionChanged +=
                _toolbarDefinitions_CollectionChanged;
            ((ObservableCollectionEx<CommandBarDataSource>) TopLevelDefinitions).ItemPropertyChanged +=
                _toolbarDefinitions_ItemPropertyChanged;
            ContextMenu = IoC.Get<IContextMenuHost>().GetContextMenu(ToolbarsContextMenuDefinition.ToolbarsContextMenu);
        }

        public override void Build()
        {
            _toolbars.Clear();
            InternalHideAllToolbars();
            var definitions = TopLevelDefinitions.OfType<ToolBarDataSource>().OrderBy(x => x.BandIndex).Where(x => x.ToolbarScope == ToolbarScope.MainWindow);
            foreach (var definition in definitions)
                Build(definition);
        }

        public override void Build(CommandBarDataSource definition)
        {
            if (!(definition is ToolBarDataSource toolbarDefinition))
                return;
           
            if (!_toolbars.ContainsKey(toolbarDefinition))
            {
                BuildToolbarCore(toolbarDefinition, out var toolBar);
                _toolbars.Add(toolbarDefinition, toolBar);
            }
            else
            {
                InternalHideToolBar(toolbarDefinition);
                BuildToolbarCore(toolbarDefinition, out var toolBar);
                _toolbars[toolbarDefinition] = toolBar;             
            }
            ChangeToolBarVisibility(toolbarDefinition);
        }

        private void BuildToolbarCore(CommandBarDataSource toolbarDefinition, out ToolBar toolBar)
        {
            
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var groups = DefinitionHost.GetSortedGroupsOfDefinition(toolbarDefinition);

            BuildLogical(toolbarDefinition, groups, group =>
            {
                return group.Items.Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder).ToList();
            });

            toolBar = new ToolBar(toolbarDefinition);
            IoC.Get<IToolbarCreator>().CreateRecursive(ref toolBar, toolbarDefinition, groups, group =>
            {
                return host.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder).ToList();
            });
        }

        public override void AddItemDefinition(CommandBarItemDataSource dataSource, CommandBarDataSource parent,
            bool addAboveSeparator)
        {
            base.AddItemDefinition(dataSource, parent, addAboveSeparator);        
            var root = FindRoot(dataSource);         
            Build(root);
        }

        public override void DeleteItemDefinition(CommandBarItemDataSource dataSource)
        {
            base.DeleteItemDefinition(dataSource);
            var root = FindRoot(dataSource);
            Build(root);
        }

        public string GetUniqueToolBarName()
        {
            return InternalGetUniqueToolBarName(1);
        }


        public void AddToolbarDefinition(ToolBarDataSource definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (TopLevelDefinitions.Contains(definition))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(definition.Text))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (TopLevelDefinitions.Any(toolbarDefinition => toolbarDefinition.Text.Equals(definition.Text)))
                throw new ToolBarAlreadyExistsException();
            TopLevelDefinitions.Add(definition);
        }

        public void RemoveToolbarDefinition(ToolBarDataSource definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (!TopLevelDefinitions.Contains(definition))
                throw new ArgumentException();
            TopLevelDefinitions.Remove(definition);
        }

        public ToolBarDataSource GetToolbarDefinitionByName(string name)
        {
            foreach (var definition in TopLevelDefinitions)
                if (definition.Text == name)
                    return definition as ToolBarDataSource;
            throw new ToolBarNotFoundException();
        }

        public void ChangeToolBarVisibility(ToolBarDataSource definition, bool visible)
        {
            if (definition == null)
                throw new ArgumentNullException();
            definition.IsVisible = visible;
        }

        private static void _ToolBarTay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                new CustomizeMenuCommandDefinition().Command.Execute(null);
        }

        private void OpenContextMenu()
        {
            ContextMenu.IsOpen = true;
        }

        private bool CanOpenContextMenu()
        {
            return TopLevelDefinitions.Count != 0;
        }

        private string InternalGetUniqueToolBarName(int index)
        {
            var i = index;
            var uniqueName = string.Format(CultureInfo.CurrentUICulture, CommandBarResources.NewToolBarUniqueNamePatter, i);
            return TopLevelDefinitions.Any(toolbarDefinition => uniqueName.Equals(toolbarDefinition.Text))
                ? InternalGetUniqueToolBarName(++i)
                : uniqueName;
        }

        private void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OpenContextMenuCommand.CanExecute(null))
            OpenContextMenuCommand.Execute(null);
        }

        private void _toolbarDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null && e.OldItems == null)
                return;
            InternalHideAllToolbars();
            Build();
        }

        private void InternalHideAllToolbars()
        {
            TopToolBarTray.RemoveAllToolbars();
            LeftToolBarTray.RemoveAllToolbars();
            BottomToolBarTray.RemoveAllToolbars();
            RightToolBarTray.RemoveAllToolbars();
        }

        private void _toolbarDefinitions_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ToolBarDataSource definition))
                return;

            if (e.PropertyName == nameof(ToolBarDataSource.IsVisible))
                ChangeToolBarVisibility(definition);
            if (e.PropertyName == nameof(ToolBarDataSource.Position))
                ChangeToolBarPosition(definition);
        }

        private void ChangeToolBarVisibility(ToolBarDataSource definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!TopLevelDefinitions.Contains(definition))
                return;
            if (string.IsNullOrEmpty(definition.Text))
                throw new ArgumentNullException(nameof(definition.Text));
            if (definition.IsVisible)
                InternalShowToolBar(definition);
            else
                InternalHideToolBar(definition);
        }

        private void ChangeToolBarPosition(ToolBarDataSource definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!TopLevelDefinitions.Contains(definition))
                return;
            if (string.IsNullOrEmpty(definition.Text))
                throw new ArgumentNullException(nameof(definition.Text));
            InternalChangePosition(definition);
        }

        private void InternalHideToolBar(ToolBarDataSource definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!_toolbars.ContainsKey(definition))
                throw new ToolBarNotFoundException(definition.Text);
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                default:
                    BottomToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
            }
        }

        private void InternalShowToolBar(ToolBarDataSource definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!_toolbars.ContainsKey(definition))
                throw new ToolBarNotFoundException(definition.Text);
            if (!definition.IsVisible)
                return;
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
            }
        }

        private void InternalChangePosition(ToolBarDataSource definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!_toolbars.ContainsKey(definition))
                throw new ToolBarNotFoundException(definition.Text);
            if (!definition.IsVisible)
                return;

            switch (definition.LastPosition)
            {
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
                case Dock.Bottom:
                    BottomToolBarTray.RemoveToolBar(_toolbars[definition]);
                    break;
            }
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(_toolbars[definition]);
                    break;
            }
        }
    }
}