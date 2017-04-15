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
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.ToolbarHostViewModel
{
    [Export(typeof(IToolBarHostViewModel))]
    public class ToolbarHostViewModel : ViewModelBase, IToolBarHostViewModel
    {
        private readonly Dictionary<ToolbarDefinition, ToolBar> _toolbars;
        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        public ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        public ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }

        public ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        public ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; }

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
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions,
            [ImportMany] CommandBarGroupDefinition[] toolbarItemGroupDefinitions,
            [ImportMany] CommandBarItemDefinition[] toolbarItemDefinitions)
        {
            _toolbars = new Dictionary<ToolbarDefinition, ToolBar>();

            ToolbarDefinitions = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definition in toolbarDefinitions)
                ToolbarDefinitions.Add(definition);


            ItemGroupDefinitions = new ObservableCollection<CommandBarGroupDefinition>(toolbarItemGroupDefinitions);

            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>(toolbarItemDefinitions);

            ToolbarDefinitions.CollectionChanged += _toolbarDefinitions_CollectionChanged;
            ToolbarDefinitions.ItemPropertyChanged += _toolbarDefinitions_ItemPropertyChanged;
            ContextMenu = IoC.Get<IContextMenuHost>().GetContextMenu(ContextMenuDefinition.ToolbarsContextMenu);
        }

        public string GetUniqueToolBarName()
        {
            return InternalGetUniqueToolBarName(1);
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

        public void Build()
        {
            _toolbars.Clear();   
            var definitions = ToolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
            {
                var toolBar = IoC.Get<IToolbarCreator>().CreateToolbar(this, definition);
                _toolbars.Add(definition, toolBar);
                ChangeToolBarVisibility(definition);
            }
        }

        public void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator)
        {
            //Apparently the current toolbar is empty so we need to add a group first
            if (definition.Group == null)
            {
                var group = new CommandBarGroupDefinition(parent, uint.MinValue);
                definition.Group = group;
                ItemGroupDefinitions.Add(group);
            }

            if (!addAboveSeparator)
            {
                var definitionsToChange = ItemDefinitions.Where(x => x.Group == definition.Group)
                    .Where(x => x.SortOrder >= definition.SortOrder)
                    .OrderBy(x => x.SortOrder);

                foreach (var definitionToChange in definitionsToChange)
                {
                    if (definitionToChange == definition)
                        continue;
                    definitionToChange.SortOrder++;
                }
            }
            ItemDefinitions.Add(definition);

            var toolbarDef = parent as ToolbarDefinition;
            if (toolbarDef == null)
                return;
            RebuildToolbar(toolbarDef);
        }

        public void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent)
        {
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {

            }
            else
            {
                var definitionsInGroup = ItemDefinitions.Where(x => x.Group == definition.Group).ToList();

                if (definitionsInGroup.Count <= 1)
                    ItemGroupDefinitions.Remove(definition.Group);
                else
                {
                    var definitionsToChange = definitionsInGroup.Where(x => x.SortOrder >= definition.SortOrder).OrderBy(x => x.SortOrder);
                    foreach (var definitionToChange in definitionsToChange)
                    {
                        if (definitionToChange == definition)
                            continue;
                        definitionToChange.SortOrder--;
                    }
                }
                ItemDefinitions.Remove(definition);
            }

            var toolbarDef = parent as ToolbarDefinition;
            if (toolbarDef == null)
                return;
            RebuildToolbar(toolbarDef);
        }

        private void RebuildToolbar(ToolbarDefinition definition)
        {
            InternalHideToolBar(definition);
            var toolbar = IoC.Get<IToolbarCreator>().CreateToolbar(this, definition);
            _toolbars[definition] = toolbar;
            ChangeToolBarVisibility(definition);
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

        private static void _ToolBarTay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                new CustomizeMenuCommandDefinition().Command.Execute(null);
        }

        private string InternalGetUniqueToolBarName(int index)
        {
            var i = index;
            var uniqueName = $"{i} (custom)";
            return ToolbarDefinitions.Any(toolbarDefinition => uniqueName.Equals(toolbarDefinition.Text))
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
    }
}