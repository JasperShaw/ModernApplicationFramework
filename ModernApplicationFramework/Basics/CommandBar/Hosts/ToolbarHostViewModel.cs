using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    [Export(typeof(IToolBarHostViewModel))]
    public sealed class ToolbarHostViewModel : CommandBarHost, IToolBarHostViewModel
    {
        private readonly Dictionary<ToolbarDefinition, ToolBar> _toolbars;
        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        public override ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

        public ContextMenu ContextMenu { get; }

        public ICommand OpenContextMenuCommand => new Command(OpenContextMenu, CanOpenContextMenu);

        public IMainWindowViewModel MainWindowViewModel { get; set; }

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
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            _toolbars = new Dictionary<ToolbarDefinition, ToolBar>();

            TopLevelDefinitions = new ObservableCollectionEx<CommandBarDefinitionBase>();
            foreach (var definition in toolbarDefinitions)
                TopLevelDefinitions.Add(definition);

            ((ObservableCollectionEx<CommandBarDefinitionBase>) TopLevelDefinitions).CollectionChanged +=
                _toolbarDefinitions_CollectionChanged;
            ((ObservableCollectionEx<CommandBarDefinitionBase>) TopLevelDefinitions).ItemPropertyChanged +=
                _toolbarDefinitions_ItemPropertyChanged;
            ContextMenu = IoC.Get<IContextMenuHost>().GetContextMenu(ContextMenuDefinition.ToolbarsContextMenu);
        }

        public override void Build()
        {
            _toolbars.Clear();
            InternalHideAllToolbars();
            var definitions = TopLevelDefinitions.OrderBy(x => x.SortOrder).Cast<ToolbarDefinition>();
            foreach (var definition in definitions)
            {
                BuildLogical(definition);
                var toolBar = IoC.Get<IToolbarCreator>().CreateToolbar(definition);
                _toolbars.Add(definition, toolBar);
                ChangeToolBarVisibility(definition);
            }
        }

        public override void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent,
            bool addAboveSeparator)
        {
            base.AddItemDefinition(definition, parent, addAboveSeparator);
            var toolbarDef = parent as ToolbarDefinition;
            if (toolbarDef == null)
                return;
            RebuildToolbar(toolbarDef);
        }

        public override void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent)
        {
            base.DeleteItemDefinition(definition, parent);
            var toolbarDef = parent as ToolbarDefinition;
            if (toolbarDef == null)
                return;
            RebuildToolbar(toolbarDef);
        }

        public string GetUniqueToolBarName()
        {
            return InternalGetUniqueToolBarName(1);
        }

        public void AddToolbarDefinition(ToolbarDefinition definition)
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

        public void RemoveToolbarDefinition(ToolbarDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (!TopLevelDefinitions.Contains(definition))
                throw new ArgumentException();
            TopLevelDefinitions.Remove(definition);
        }

        public ToolbarDefinition GeToolbarDefinitionByName(string name)
        {
            foreach (var definition in TopLevelDefinitions)
                if (definition.Text == name)
                    return definition as ToolbarDefinition;
            throw new ToolBarNotFoundException();
        }

        public void ChangeToolBarVisibility(ToolbarDefinition definition, bool visible)
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

        private void RebuildToolbar(ToolbarDefinition definition)
        {
            InternalHideToolBar(definition);
            var toolbar = IoC.Get<IToolbarCreator>().CreateToolbar(definition);
            _toolbars[definition] = toolbar;
            ChangeToolBarVisibility(definition);
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
            var uniqueName = $"{i} (custom)";
            return TopLevelDefinitions.Any(toolbarDefinition => uniqueName.Equals(toolbarDefinition.Text))
                ? InternalGetUniqueToolBarName(++i)
                : uniqueName;
        }

        private void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
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
            var definition = sender as ToolbarDefinition;
            if (definition == null)
                return;

            if (e.PropertyName == nameof(ToolbarDefinition.IsVisible))
                ChangeToolBarVisibility(definition);
            if (e.PropertyName == nameof(ToolbarDefinition.Position))
                ChangeToolBarPosition(definition);
        }

        private void ChangeToolBarVisibility(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!TopLevelDefinitions.Contains(definition))
                throw new ToolBarNotFoundException();
            if (string.IsNullOrEmpty(definition.Text))
                throw new ArgumentNullException(nameof(definition.Text));
            if (definition.IsVisible)
                InternalShowToolBar(definition);
            else
                InternalHideToolBar(definition);
        }

        private void ChangeToolBarPosition(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!TopLevelDefinitions.Contains(definition))
                throw new ToolBarNotFoundException();
            if (string.IsNullOrEmpty(definition.Text))
                throw new ArgumentNullException(nameof(definition.Text));
            InternalChangePosition(definition);
        }

        private void InternalHideToolBar(ToolbarDefinition definition)
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

        private void InternalShowToolBar(ToolbarDefinition definition)
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

        private void InternalChangePosition(ToolbarDefinition definition)
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