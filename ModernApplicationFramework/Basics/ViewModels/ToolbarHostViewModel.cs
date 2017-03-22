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
using ModernApplicationFramework.Basics.Customize;
using ModernApplicationFramework.Basics.Definitions;
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
        public ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; set; }


        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private Theme _theme;
        private ToolBarTray _topToolBarTay;

        [ImportingConstructor]
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            ToolbarDefinitions = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definition in toolbarDefinitions)
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
            return true;
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

        public void AddToolbarDefinition(ToolbarDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (ToolbarDefinitions.Contains(definition))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(definition.Name))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (ToolbarDefinitions.Any(toolbarDefinition => toolbarDefinition.Name.Equals(definition.Name)))
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

        protected virtual bool CanClickContextMenuItem(ContextMenuGlyphItem item)
        {
            return true;
        }

        public ToolbarDefinition GeToolbarDefinitionByName(string name)
        {
            foreach (var definition in ToolbarDefinitions)
            {
                if (definition.ToolBar.Name == name)
                    return definition;
            }
            throw new ToolBarNotFoundException();
        }

        public void ChangeToolBarVisibility(ToolbarDefinition definition, bool visible)
        {
            if (definition == null)
                throw new ArgumentNullException();
            definition.Visible = visible;
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

        private void InternalShowToolBar(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (!definition.Visible)
                return;
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(definition.ToolBar);
                    break;
            }
        }

        private void _toolbarDefinitions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ToolbarDefinition item in e.NewItems)
                    if (item.Visible)
                        InternalShowToolBar(item);
            if (e.OldItems != null)
                foreach (ToolbarDefinition item in e.OldItems)
                    InternalHideToolBar(item);
            BuildContextMenu();
        }


        private void InternalHideToolBar(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                default:
                    BottomToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
            }
        }

        private void _toolbarDefinitions_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var definition = sender as ToolbarDefinition;
            if (definition == null)
                return;

            if (e.PropertyName == nameof(ToolbarDefinition.Visible))
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
            if (string.IsNullOrEmpty(definition.Name))
                throw new ArgumentNullException(nameof(definition.ToolBar.Name));
            if (definition.Visible)
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
            if (string.IsNullOrEmpty(definition.Name))
                throw new ArgumentNullException(nameof(definition.ToolBar.Name));
                InternalChangePosition(definition);
        }

        private void InternalChangePosition(ToolbarDefinition definition)
        {
            if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
                BottomToolBarTray == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");

            if (!definition.Visible)
                return;

            switch (definition.LastPosition)
            {
                case Dock.Left:
                    LeftToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                case Dock.Top:
                    TopToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
                case Dock.Bottom:
                    BottomToolBarTray.RemoveToolBar(definition.ToolBar);
                    break;
            }
            switch (definition.Position)
            {
                case Dock.Top:
                    TopToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                case Dock.Right:
                    RightToolBarTray.AddToolBar(definition.ToolBar);
                    break;
                default:
                    BottomToolBarTray.AddToolBar(definition.ToolBar);
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