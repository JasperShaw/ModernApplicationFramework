using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.Basics.ViewModels
{
    [Export(typeof(IToolBarHostViewModel))]
    public class ToolbarHostViewModel : ViewModelBase, IToolBarHostViewModel
    {
        public ObservableCollectionEx<ToolbarDefinitionOld> ToolbarDefinitions { get; }


        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private Theme _theme;
        private ToolBarTray _topToolBarTay;

        [ImportingConstructor]
        public ToolbarHostViewModel([ImportMany] ToolbarDefinitionOld[] toolbarDefinitionsOld)
        {
            ToolbarDefinitions = new ObservableCollectionEx<ToolbarDefinitionOld>();
            foreach (var definition in toolbarDefinitionsOld)
                ToolbarDefinitions.Add(definition);
            ToolbarDefinitions.CollectionChanged += _toolbarDefinitions_CollectionChanged;
            ToolbarDefinitions.ItemPropertyChanged += _toolbarDefinitions_ItemPropertyChanged;
            ContextMenu = new ContextMenu();
            BuildContextMenu();
        }

        public ContextMenu ContextMenu { get; }

        public Command<ContextMenuGlyphItem> ClickContextMenuItemCommand
            => new Command<ContextMenuGlyphItem>(ClickContextMenuItem, CanClickContextMenuItem);

        public ICommand OpenCostumizeDialogCommand => new Command(OpenCostumizeDialog, CanOpenCostumizeDialog);

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

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

        private string InternalGetUniqueToolBarName(int index)
        {
            var i = index;
            var uniqueName = $"{i} (custom)";
            return ToolbarDefinitions.Any(toolbarDefinition => uniqueName.Equals(toolbarDefinition.Name)) ? InternalGetUniqueToolBarName(++i) : uniqueName;
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

        private async void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await OpenContextMenuCommand.Execute();
        }

        private void BuildContextMenu()
        {
            ContextMenu.Items.Clear();

            foreach (var definition in ToolbarDefinitions.OrderBy(x => x.Name))
            {
                var item = new ContextMenuGlyphItem
                {
                    Header = definition.Name,
                    Command = ClickContextMenuItemCommand,
                    DataContext = definition
                };
                if (definition.Visible)
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
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = newTheme.GetResourceUri() });

            ContextMenu.ChangeTheme(oldValue, newValue);
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }

        public void AddToolbarDefinition(ToolbarDefinitionOld definitionOld)
        {
            if (definitionOld == null)
                throw new ArgumentNullException();
            if (ToolbarDefinitions.Contains(definitionOld))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(definitionOld.Name))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (ToolbarDefinitions.Any(toolbarDefinition => toolbarDefinition.Name.Equals(definitionOld.Name)))
                throw new ToolBarAlreadyExistsException();
            ToolbarDefinitions.Add(definitionOld);
        }

        public void RemoveToolbarDefinition(ToolbarDefinitionOld definitionOld)
        {
            if (definitionOld == null)
                throw new ArgumentNullException();
            if (!ToolbarDefinitions.Contains(definitionOld))
                throw new ArgumentException();
            ToolbarDefinitions.Remove(definitionOld);
        }

        protected virtual bool CanClickContextMenuItem(ContextMenuGlyphItem item)
        {
            return true;
        }

        public ToolbarDefinitionOld GeToolbarDefinitionByName(string name)
        {
            foreach (var definition in ToolbarDefinitions)
            {
                if (definition.ToolBar.Name == name)
                    return definition;
            }
            throw new ToolBarNotFoundException();
        }

        public void ChangeToolBarVisibility(ToolbarDefinitionOld definitionOld, bool visible)
        {
            if (definitionOld == null)
                throw new ArgumentNullException();
            definitionOld.Visible = visible;
        }

        protected virtual void ClickContextMenuItem(ContextMenuGlyphItem contextMenuItem)
        {
            var dataContext = contextMenuItem.DataContext as ToolbarDefinitionOld;
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

        private void InternalShowToolBar(ToolbarDefinitionOld definitionOld)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!definitionOld.Visible)
                return;
            switch (definitionOld.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
            }
        }

        private void _toolbarDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ToolbarDefinitionOld item in e.NewItems)
                    if (item.Visible)
                        InternalShowToolBar(item);
            if (e.OldItems != null)
                foreach (ToolbarDefinitionOld item in e.OldItems)
                    InternalHideToolBar(item);
            OpenContextMenuCommand.RaiseCanExecuteChanged();
            BuildContextMenu();
        }


        private void InternalHideToolBar(ToolbarDefinitionOld definitionOld)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (definitionOld.Position)
            {
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                default:
                    BottomToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
            }
        }

        private void _toolbarDefinitions_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var definition = sender as ToolbarDefinitionOld;
            if (definition == null)
                return;

            if (e.PropertyName == nameof(ToolbarDefinitionOld.Visible))
                ChangeToolBarVisibility(definition);
            if (e.PropertyName == nameof(ToolbarDefinitionOld.Position))
                ChangeToolBarPosition(definition);
            BuildContextMenu();
        }

        private void ChangeToolBarVisibility(ToolbarDefinitionOld definitionOld)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!ToolbarDefinitions.Contains(definitionOld))
                throw new ToolBarNotFoundException();
            if (string.IsNullOrEmpty(definitionOld.Name))
                throw new ArgumentNullException(nameof(definitionOld.ToolBar.Name));
            if (definitionOld.Visible)
                InternalShowToolBar(definitionOld);
            else
                InternalHideToolBar(definitionOld);
        }

        private void ChangeToolBarPosition(ToolbarDefinitionOld definitionOld)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!ToolbarDefinitions.Contains(definitionOld))
                throw new ToolBarNotFoundException();
            if (string.IsNullOrEmpty(definitionOld.Name))
                throw new ArgumentNullException(nameof(definitionOld.ToolBar.Name));
                InternalChangePosition(definitionOld);
        }

        private void InternalChangePosition(ToolbarDefinitionOld definitionOld)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");

            if (!definitionOld.Visible)
                return;

            switch (definitionOld.LastPosition)
            {
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Bottom:
                    BottomToolBarTray.RemoveToolBar(definitionOld.ToolBar);
                    break;
            }
            switch (definitionOld.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(definitionOld.ToolBar);
                    break;
            }
        }

        public void SetupToolbars()
        {
            var definitions = ToolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
                ChangeToolBarVisibility(definition);
        }
    }
}