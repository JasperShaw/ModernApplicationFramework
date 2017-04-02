using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.ViewModels
{
    [Export(typeof(IToolBarHostViewModel))]
    public class ToolbarHostViewModel : ViewModelBase, IToolBarHostViewModel
    {
        private readonly Dictionary<ToolbarDefinition, ToolBar> _toolbars;
        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        private readonly MenuItem _editContextMenuItem;
        private readonly MenuItem _separatorItem;

        public ObservableCollectionEx<ToolbarItemGroupDefinition> ToolbarItemGroupDefinitions { get; }
        public ObservableCollectionEx<ToolbarItemDefinition> ToolbarItemDefinitions { get; }

        [ImportingConstructor]
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions,
            [ImportMany] ToolbarItemGroupDefinition[] toolbarItemGroupDefinitions,
            [ImportMany] ToolbarItemDefinition[] toolbarItemDefinitions)
        {
            _toolbars = new Dictionary<ToolbarDefinition, ToolBar>();

            ToolbarDefinitions = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definition in toolbarDefinitions)
                ToolbarDefinitions.Add(definition);


            ToolbarItemGroupDefinitions = new ObservableCollectionEx<ToolbarItemGroupDefinition>();
            foreach (var definition in toolbarItemGroupDefinitions)
                ToolbarItemGroupDefinitions.Add(definition);

            ToolbarItemDefinitions = new ObservableCollectionEx<ToolbarItemDefinition>();
            foreach (var definition in toolbarItemDefinitions)
                ToolbarItemDefinitions.Add(definition);


            ToolbarDefinitions.CollectionChanged += _toolbarDefinitions_CollectionChanged;
            ToolbarDefinitions.ItemPropertyChanged += _toolbarDefinitions_ItemPropertyChanged;

            var editDefinition = new CommandMenuItemDefinition(new EditMenuCommandDefinition());
            _editContextMenuItem = new MenuItem(editDefinition);
            _separatorItem = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);

            ContextMenu = new ContextMenu();
        }

        public ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; }

        public ContextMenu ContextMenu { get; }

        public IMainWindowViewModel MainWindowViewModel { get; set; }

        public Command OpenContextMenuCommand => new Command(OpenContextMenu, CanOpenContextMenu);

        public ToolBarTray BottomToolBarTray
        {
            get => _bottomToolBarTay;
            set
            {
                _bottomToolBarTay = value;
                _bottomToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        public string GetUniqueToolBarName()
        {
            return InternalGetUniqueToolBarName(1);
        }

        public ToolBarTray LeftToolBarTray
        {
            get => _leftToolBarTay;
            set
            {
                _leftToolBarTay = value;
                _leftToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        public ToolBarTray RightToolBarTray
        {
            get => _rightToolBarTay;
            set
            {
                _rightToolBarTay = value;
                _rightToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        public ToolBarTray TopToolBarTray
        {
            get => _topToolBarTay;
            set
            {
                _topToolBarTay = value;
                _topToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        public void AddToolbarDefinition(ToolbarDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (ToolbarDefinitions.Contains(definition))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(definition.Text))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (ToolbarDefinitions.Any(toolbarDefinition => toolbarDefinition.Text.Equals(definition.Text)))
                throw new ToolBarAlreadyExistsException();
            ToolbarDefinitions.Add(definition);
        }

        public void RemoveToolbarDefinition(ToolbarDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (!ToolbarDefinitions.Contains(definition))
                throw new ArgumentException();
            ToolbarDefinitions.Remove(definition);
        }

        public void SetupToolbars()
        {
            _toolbars.Clear();
            var definitions = ToolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
            {
                var toolBar = new ToolBar(definition);
                BuildToolBar(definition, toolBar);
                _toolbars.Add(definition, toolBar);
                ChangeToolBarVisibility(definition);
            }
            BuildContextMenu();
        }

        public ToolbarDefinition GeToolbarDefinitionByName(string name)
        {
            foreach (var definition in ToolbarDefinitions)
                if (definition.Text == name)
                    return definition;
            throw new ToolBarNotFoundException();
        }

        public void ChangeToolBarVisibility(ToolbarDefinition definition, bool visible)
        {
            if (definition == null)
                throw new ArgumentNullException();
            definition.IsVisible = visible;
        }

        protected virtual void OpenContextMenu()
        {
            ContextMenu.IsOpen = true;
        }

        protected virtual bool CanOpenContextMenu()
        {
            return ToolbarDefinitions.Count != 0;
        }

        private void BuildToolBar(ToolbarDefinition definition, ItemsControl toolBar)
        {
            var groups = ToolbarItemGroupDefinitions
                .Where(x => x.ParentToolbar == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();


            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = ToolbarItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                foreach (var toolBarItem in toolBarItems)
                {
                    var button = new CommandDefinitionButton(toolBarItem);
                    toolBar.Items.Add(button);
                }

                if (i < groups.Count - 1 && toolBarItems.Any())
                    toolBar.Items.Add(new CommandDefinitionButton(CommandBarSeparatorDefinition.MenuSeparatorDefinition));
            }
        }

        private string InternalGetUniqueToolBarName(int index)
        {
            var i = index;
            var uniqueName = $"{i} (custom)";
            return ToolbarDefinitions.Any(toolbarDefinition => uniqueName.Equals(toolbarDefinition.Text))
                ? InternalGetUniqueToolBarName(++i)
                : uniqueName;
        }

        private async void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await OpenContextMenuCommand.Execute();
        }

        private void BuildContextMenu()
        {
            ContextMenu.Items.Clear();
            foreach (var definition in ToolbarDefinitions.OrderBy(x => x.Text))
            {
                var def = new CommandMenuItemDefinition(new ToolBarVisibilityCommandDefinition(definition),
                    definition.IsVisible, false);
                var item = new MenuItem(def);
                ContextMenu.Items.Add(item);
            }         
            ContextMenu.Items.Add(_separatorItem);
            ContextMenu.Items.Add(_editContextMenuItem);
        }

        private void _toolbarDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null && e.OldItems == null)
                return;
            InternalHideAllToolbars();
            SetupToolbars();
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
            BuildContextMenu();
        }

        private void ChangeToolBarVisibility(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!ToolbarDefinitions.Contains(definition))
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
            if (!ToolbarDefinitions.Contains(definition))
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

        private sealed class ToolBarVisibilityCommandDefinition : CommandDefinition
        {
            private readonly ToolbarDefinition _definition;

            public override string Name => null;
            public override string Text { get; }
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override string ShortcutText => null;

            public override ICommand Command { get; }

            public ToolBarVisibilityCommandDefinition(ToolbarDefinition definition)
            {
                _definition = definition;
                Text = definition.Text;
                Command = new Command(ClickContextMenuItem, () => true);
            }

            private void ClickContextMenuItem()
            {
                _definition.IsVisible = !_definition.IsVisible;
            }
        }

        private sealed class EditMenuCommandDefinition : CommandDefinition
        {
            public EditMenuCommandDefinition()
            {
                Command = new Command(OpenCustomizeDialog, ()=> true);
            }

            private void OpenCustomizeDialog()
            {
                var windowManager = new WindowManager();
                var customizeDialog = new CustomizeDialogViewModel();
                windowManager.ShowDialog(customizeDialog);
            }

            public override ICommand Command { get; }
            public override string Name => null;
            public override string Text => "&Customize...";
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override string ShortcutText => null;
        }
    }
}