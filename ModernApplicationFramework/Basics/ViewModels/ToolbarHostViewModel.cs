using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using Separator = ModernApplicationFramework.Controls.Separator;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.ViewModels
{
    [Export(typeof(IToolBarHostViewModel))]
    public class ToolbarHostViewModel : ViewModelBase, IToolBarHostViewModel
    {
        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private Theme _theme;
        private ToolBarTray _topToolBarTay;

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        public Command<ContextMenuGlyphItem> ClickContextMenuItemCommand
            => new Command<ContextMenuGlyphItem>(ClickContextMenuItem, CanClickContextMenuItem);

        public ICommand OpenCostumizeDialogCommand => new Command(OpenCostumizeDialog, CanOpenCostumizeDialog);

        [ImportingConstructor]
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions,
            [ImportMany] ToolbarItemGroupDefinition[] toolbarItemGroupDefinitions,
            [ImportMany] ToolbarItemDefinition[] toolbarItemDefinitions)
        {
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
            ContextMenu = new ContextMenu();
        }

        public ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; }
        public ObservableCollectionEx<ToolbarItemGroupDefinition> ToolbarItemGroupDefinitions { get; }
        public ObservableCollectionEx<ToolbarItemDefinition> ToolbarItemDefinitions { get; }

        public ContextMenu ContextMenu { get; }

        public Theme Theme
        {
            get => _theme;
            set
            {
                if (value == null)
                    throw new NoNullAllowedException();
                if (Equals(value, _theme))
                    return;
                var oldTheme = _theme;
                _theme = value;
                OnPropertyChanged();
                ChangeTheme(oldTheme, _theme);
                OnRaiseThemeChanged(new ThemeChangedEventArgs(value, oldTheme));
            }
        }

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
            var definitions = ToolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
            {
                var toolBar = new ToolBar();
                BuildToolBar(definition, toolBar);

                LeftToolBarTray.AddToolBar(toolBar);
                //ChangeToolBarVisibility(definition);
            }


            BuildContextMenu();
        }

        private void BuildToolBar(ToolbarDefinition definition, ToolBar toolBar)
        {
            var groups = ToolbarItemGroupDefinitions
                .Where(x => x.ParentToolbar == definition)
                .OrderBy(x => x.SortOrder)
                .ToList();


            for (int i = 0; i < groups.Count; i++)
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
                    toolBar.Items.Add(new Separator());
            }
        }

        public void ChangeTheme(Theme oldValue, Theme newValue)
        {
            var oldTheme = oldValue;
            var newTheme = newValue;
            var resources = ContextMenu.Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }

            if (newTheme != null)
                resources.MergedDictionaries.Add(new ResourceDictionary {Source = newTheme.GetResourceUri()});

            ContextMenu.ChangeTheme(oldValue, newValue);
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

        protected virtual void OpenCostumizeDialog()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new CustomizeDialogViewModel();
            windowManager.ShowDialog(customizeDialog);
        }

        protected virtual bool CanOpenCostumizeDialog()
        {
            return true;
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }

        protected virtual bool CanClickContextMenuItem(ContextMenuGlyphItem item)
        {
            return true;
        }

        protected virtual void ClickContextMenuItem(ContextMenuGlyphItem contextMenuItem)
        {
            var dataContext = contextMenuItem.DataContext as ToolbarDefinition;
            if (contextMenuItem.IconGeometry == null)
            {
                ContextMenuGlyphItemUtilities.SetCheckMark(contextMenuItem);
                ChangeToolBarVisibility(dataContext, true);
            }
            else
            {
                contextMenuItem.IconGeometry = null;
                ChangeToolBarVisibility(dataContext, false);
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
                var item = new ContextMenuGlyphItem
                {
                    Header = definition.Text,
                    Command = ClickContextMenuItemCommand,
                    DataContext = definition
                };
                if (definition.IsVisible)
                    ContextMenuGlyphItemUtilities.SetCheckMark(item);
                item.CommandParameter = item;
                ContextMenu.Items.Add(item);
            }

            var editItem = new ContextMenuItem
            {
                Header = "Edit...",
                Command = OpenCostumizeDialogCommand
            };
            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(editItem);
        }

        private void _toolbarDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ToolbarDefinition item in e.NewItems)
                    if (item.IsVisible)
                        InternalShowToolBar(item);
            if (e.OldItems != null)
                foreach (ToolbarDefinition item in e.OldItems)
                    InternalHideToolBar(item);
            OpenContextMenuCommand.RaiseCanExecuteChanged();
            BuildContextMenu();
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
            //if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
            //    BottomToolBarTray == null)
            //    throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            //switch (definition.Position)
            //{
            //    case Dock.Top:
            //        TopToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Left:
            //        LeftToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Right:
            //        RightToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    default:
            //        BottomToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //}
        }

        private void InternalShowToolBar(ToolbarDefinition definition)
        {
            //if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
            //    BottomToolBarTray == null)
            //    throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            //if (!definition.IsVisible)
            //    return;
            //switch (definition.Position)
            //{
            //    case Dock.Top:
            //        TopToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Left:
            //        LeftToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Right:
            //        RightToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    default:
            //        BottomToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //}
        }

        private void InternalChangePosition(ToolbarDefinition definition)
        {
            //if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
            //    BottomToolBarTray == null)
            //    throw new NullReferenceException("Could not find all 4 ToolbarTrays");

            //if (!definition.IsVisible)
            //    return;

            //switch (definition.LastPosition)
            //{
            //    case Dock.Left:
            //        LeftToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Top:
            //        TopToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Right:
            //        RightToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Bottom:
            //        BottomToolBarTray.RemoveToolBar(definition.ToolBar);
            //        break;
            //}
            //switch (definition.Position)
            //{
            //    case Dock.Top:
            //        TopToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Left:
            //        LeftToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    case Dock.Right:
            //        RightToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //    default:
            //        BottomToolBarTray.AddToolBar(definition.ToolBar);
            //        break;
            //}
        }
    }
}