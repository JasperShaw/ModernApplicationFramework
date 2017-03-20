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
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Test;
using ModernApplicationFramework.Utilities;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using Separator = ModernApplicationFramework.Controls.Separator;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.ViewModels
{
    [Export(typeof(IToolBarHostViewModel))]
    public class ToolbarHostViewModel : ViewModelBase, IToolBarHostViewModel
    {
        /*
            This Dictionary contains all the information needed to interact with the toolbar across classes
            This Dictionary contains as Key:
                IdName of toolbar
            And as Value
                a Tuple with 4 Items:
                    Item1: Toolbar-Object
                    Item2: Toolbar-Dock orientation
                    Item3: Toolbar-Visible
        */
        //private readonly Dictionary<string, Tuple<ToolBar, Dock, bool>> _contextList =
        //    new Dictionary<string, Tuple<ToolBar, Dock, bool>>();


        private readonly ObservableCollectionEx<ToolbarDefinition> _toolbarDefinitions;


        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private Theme _theme;
        private ToolBarTray _topToolBarTay;

        [ImportingConstructor]
        public ToolbarHostViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            _toolbarDefinitions = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definition in toolbarDefinitions)
                _toolbarDefinitions.Add(definition);
            _toolbarDefinitions.CollectionChanged += _toolbarDefinitions_CollectionChanged;
            _toolbarDefinitions.ItemPropertyChanged += _toolbarDefinitions_ItemPropertyChanged;
            ContextMenu = new ContextMenu();
            BuildContextMenu();
        }

        private ContextMenu ContextMenu { get; }

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




        #region Done

        private async void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await OpenContextMenuCommand.Execute();
        }

        public void BuildContextMenu()
        {
            ContextMenu.Items.Clear();

            foreach (var definition in _toolbarDefinitions.OrderBy(x => x.Name))
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
            if (_toolbarDefinitions.Contains(definition))
                throw new ArgumentException();
            if (string.IsNullOrEmpty(definition.Name))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (_toolbarDefinitions.Any(item => item.ToolBar.Name == definition.ToolBar.Name))
                throw new ToolBarAlreadyExistsException();
            _toolbarDefinitions.Add(definition);
        }

        public void RemoveToolbarDefinition(ToolbarDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException();
            if (!_toolbarDefinitions.Contains(definition))
                throw new ArgumentException();
            _toolbarDefinitions.Remove(definition);
        }

        protected virtual bool CanClickContextMenuItem(ContextMenuGlyphItem item)
        {
            return true;
        }

        public ToolbarDefinition GeToolbarDefinitionByName(string name)
        {
            foreach (var definition in _toolbarDefinitions)
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
            if (!_toolbarDefinitions.Contains(definition))
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
            if (!_toolbarDefinitions.Contains(definition))
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

        #endregion

        public void SetupToolbars()
        {
            var definitions = _toolbarDefinitions.OrderBy(x => x.SortOrder);
            foreach (var definition in definitions)
                ChangeToolBarVisibility(definition);
        }






        // /*
        //    This adds a new toolbar by:
        //        Checking for:
        //            Not Null Object
        //            ObjectID not null
        //            Not Existing already
        //        Creating a new ContextMenuItem for it
        //        Adds to Dictionary
        //        If visible param is true
        //            Show Toolbar on screen
        //*/
        // /// <summary>
        // ///     Adds new Toolbar to HostControl
        // /// </summary>
        // /// <param name="toolBar">Toolbar object</param>
        // /// <param name="visible">Toolbar visibility</param>
        // /// <param name="dock">Toolbar orientation</param>
        // public void AddToolBar(ToolBar toolBar, bool visible, Dock dock)
        // {
        //     if (toolBar == null)
        //         throw new ArgumentNullException(nameof(toolBar));
        //     if (string.IsNullOrEmpty(toolBar.IdentifierName))
        //         throw new NullReferenceException("Toolbar Id must not be null");
        //     if (_contextList.ContainsKey(toolBar.IdentifierName))
        //         throw new ToolBarAlreadyExistsException();
        //     _contextList.Add(toolBar.IdentifierName,
        //         new Tuple<ToolBar, Dock, bool>(toolBar, dock, visible));
        //     if (!visible)
        //         return;
        //     ShowToolBarByName(toolBar.IdentifierName);
        // }

        //public void AddToolBar(ToolbarDefinition definition)
        //{
        //    if (definition == null)
        //        throw new ArgumentNullException(nameof(definition));
        //    if (string.IsNullOrEmpty(definition.ToolBar.IdentifierName))
        //        throw new NullReferenceException("Toolbar Id must not be null");
        //    if (_toolbarDefinitions.Contains(definition))
        //        throw new ToolBarAlreadyExistsException();


        //    //TODO

        //}


        //public void RemoveToolBar(ToolbarDefinition definition)
        //{
        //    if (definition == null)
        //        throw new ArgumentNullException(nameof(definition));
        //    if (!_toolbarDefinitions.Contains(definition))
        //        throw new ToolBarNotFoundException();

        //    //TODO

        //}


        //public ToolbarDefinition GetToolbarDefinition(string name)
        //{
        //    if (string.IsNullOrEmpty(name))
        //        throw new ArgumentNullException();
        //}


        // /*
        //     Change Orientation of Toolbar by:
        //         Checking for:
        //             At least one Tray exists
        //             IdName not null
        //             Toolbar exists
        //         Save visibility
        //         HideToolbar
        //         Change Dock
        //         If was Visible
        //             Show Again
        // */
        // /// <summary>
        // ///     Change Orientation of Toolbar
        // /// </summary>
        // /// <param name="name">IdentifierName of Toolbar</param>
        // /// <param name="newValue">New Orientation Value</param>
        // public void ChangeToolBarPosition(string name, Dock newValue)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     if (!_contextList.ContainsKey(name))
        //         throw new ToolBarNotFoundException();
        //     var wasVisible = _contextList[name].Item3;
        //     HideToolBarByName(name);
        //     UpdateDock(name, newValue);
        //     if (wasVisible)
        //         ShowToolBarByName(name);
        // }

        // /*
        //     Change Visible of Toolbar by:
        //          Checking for:
        //             At least one Tray exists
        //             IdName not null
        //             Toolbar exists
        //         If newValue true
        //             Show Toolbar
        //         Else
        //             Hide Toolbar  
        // */
        // /// <summary>
        // ///     Change Visible of Toolbar
        // /// </summary>
        // /// <param name="name">IdentifierName of Toolbar</param>
        // /// <param name="newValue">New Visible Value</param>



        // public void ChangeToolBarVisibility(ToolbarDefinition definition)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");
        //     if (!_toolbarDefinitions.Contains(definition))
        //         throw new ToolBarNotFoundException();
        //     if (string.IsNullOrEmpty(definition.ToolBar.IdentifierName))
        //         throw new ArgumentNullException(nameof(definition.ToolBar.IdentifierName));
        //     if (definition.Visible)
        //         InternalShowToolBar(definition);
        //     else
        //         InternalHideToolBar(definition);
        // }


        // public void ChangeToolBarVisibility(ToolBar toolBar, bool visible)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");


        //     var toolbar = GetToolBar()

        //     if (!_toolbarDefinitions.Contains(definition))
        //         throw new ToolBarNotFoundException();
        //     if (string.IsNullOrEmpty(definition.ToolBar.IdentifierName))
        //         throw new ArgumentNullException(nameof(definition.ToolBar.IdentifierName));
        //     if (definition.Visible)
        //         InternalShowToolBar(definition);
        //     else
        //         InternalHideToolBar(definition);
        // }



        // /// <summary>
        // ///     Get a Toolbar by Name
        // /// </summary>
        // /// <param name="name">IdentifierName of Toolbar</param>
        // /// <returns>Found Toolbar Object</returns>
        // public ToolBar GetToolBar(string name)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));

        //     foreach (var definition in _toolbarDefinitions)
        //     {
        //         if (definition.ToolBar.IdentifierName == name)
        //             return definition.ToolBar;
        //     }
        //     throw new ToolBarNotFoundException();
        // }


        // public void GetDefinition(ToolBar toolBar)
        // {

        // }


        // /// <summary>
        // ///     Get Orientation of Toolbar
        // /// </summary>
        // /// <param name="name">Identifier Name of Toolbar</param>
        // /// <returns>Orientation</returns>
        // public Dock GetToolBarPosition(string name)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     if (!_contextList.ContainsKey(name))
        //         throw new ToolBarNotFoundException();
        //     return _contextList[name].Item2;
        // }

        // /// <summary>
        // ///     Returns a list of Toolbar Objects
        // /// </summary>
        // /// <returns>List with Toolsbars</returns>
        // public List<ToolBar> GetToolBars()
        // {
        //     return _toolbarDefinitions.Select(definition => definition.ToolBar).ToList();
        // }

        // /// <summary>
        // ///     Get Toolbar Visible
        // /// </summary>
        // /// <param name="name">Identifier Name of Toolbar</param>
        // /// <returns>Bool of visibility</returns>
        // public bool GetToolBarVisibility(string name)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     if (!_contextList.ContainsKey(name))
        //         throw new ToolBarNotFoundException();
        //     return _contextList[name].Item3;
        // }

        // /*
        //    Hides Toolbar by:
        //        Check for:
        //            At least one tray exists
        //        Decide Dock
        //        Remove From Tray
        //*/
        // /// <summary>
        // ///     Hides toolbar
        // /// </summary>
        // /// <param name="toolBar">Toolbar</param>
        // /// <param name="dock">Orientation</param>
        // public void HideToolBar(ToolBar toolBar, Dock dock)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");
        //     switch (dock)
        //     {
        //         case Dock.Top:
        //             TopToolBarTray.RemoveToolBar(toolBar);
        //             break;
        //         case Dock.Left:
        //             LeftToolBarTray.RemoveToolBar(toolBar);
        //             break;
        //         case Dock.Right:
        //             RightToolBarTray.RemoveToolBar(toolBar);
        //             break;
        //         default:
        //             BottomToolBarTray.RemoveToolBar(toolBar);
        //             break;
        //     }
        // }

        // /*
        //     Hides Toolbar from IdName by:
        //         Check for:
        //             name not null/empty
        //         UpdateVisibility
        //         Hide ( Object and Orientation)
        //         Remove Checkmark from MenuItem
        // */
        // /// <summary>
        // ///     Hides Toolbar by name
        // /// </summary>
        // /// <param name="name">Name</param>
        // public void HideToolBarByName(string name)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         return;
        //     UpdateVisibility(name, false);
        //     HideToolBar(_contextList[name].Item1, _contextList[name].Item2);
        // }





        // public ToolbarDefinition GeToolbarDefinition(string idName)
        // {
        //     if (string.IsNullOrEmpty(idName))
        //         throw new ArgumentNullException(nameof(idName));
        //     return _toolbarDefinitions.FirstOrDefault(d => d.ToolBar.IdentifierName == idName);
        // }

        // /*
        //    Show Toolbar by:
        //        Check for:
        //            At least one tray exists
        //        Decide Dock
        //        Add Toolbar to tray
        //*/
        // public void ShowToolBar(ToolBar toolBar, Dock dock)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");
        //     switch (dock)
        //     {
        //         case Dock.Top:
        //             TopToolBarTray.AddToolBar(toolBar);
        //             break;
        //         case Dock.Left:
        //             LeftToolBarTray.AddToolBar(toolBar);
        //             break;
        //         case Dock.Right:
        //             RightToolBarTray.AddToolBar(toolBar);
        //             break;
        //         default:
        //             BottomToolBarTray.AddToolBar(toolBar);
        //             break;
        //     }
        // }

        // private void InternalShowToolBar(ToolbarDefinition definition)
        // {
        //     if (TopToolBarTray == null || LeftToolBarTray == null || RightToolBarTray == null ||
        //         BottomToolBarTray == null)
        //         throw new NullReferenceException("Could not find all 4 ToolbarTrays");
        //     switch (definition.Position)
        //     {
        //         case Dock.Top:
        //             TopToolBarTray.AddToolBar(definition.ToolBar);
        //             break;
        //         case Dock.Left:
        //             LeftToolBarTray.AddToolBar(definition.ToolBar);
        //             break;
        //         case Dock.Right:
        //             RightToolBarTray.AddToolBar(definition.ToolBar);
        //             break;
        //         default:
        //             BottomToolBarTray.AddToolBar(definition.ToolBar);
        //             break;
        //     }
        // }



        // /*
        //     Show Toolbar with IdName by:
        //         Check for: 
        //             name not null/empty
        //         UpdateVisibility
        //         Show Toolbar (Object Orientation)
        //         Set Checkmark for MenuItem
        // */
        // public void ShowToolBarByName(string name)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     UpdateVisibility(name, true);
        //     ShowToolBar(_contextList[name].Item1, _contextList[name].Item2);
        // }

        // /*
        //     Update Dock by:
        //         Check for:
        //             Name not null/empty
        //             Toolbar exists
        //         Save Toolbar Object, Visible, MenuItem
        //         Remove toolbar from Dictionary
        //         Add new Entry to Dictionary by using:
        //             just saved values 
        //             new Dock value (param)
        // */
        // public void UpdateDock(string name, Dock newValue)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     if (!_contextList.ContainsKey(name))
        //         throw new ToolBarNotFoundException();

        //     var oldToolbar = _contextList[name].Item1;
        //     var oldVibility = _contextList[name].Item3;

        //     _contextList.Remove(name);
        //     _contextList.Add(name,
        //         new Tuple<ToolBar, Dock, bool>(oldToolbar, newValue, oldVibility));
        // }

        // /*
        //    Update Visible by:
        //        Check for:
        //            Name not null/empty
        //            Toolbar exists
        //        Save Toolbar Object, Dock, MenuItem
        //        Remove toolbar from Dictionary
        //        Add new Entry to Dictionary by using:
        //            just saved values 
        //            new Visible value (param)
        //*/
        // public void UpdateVisibility(string name, bool newValue)
        // {
        //     if (string.IsNullOrEmpty(name))
        //         throw new ArgumentNullException(nameof(name));
        //     if (!_contextList.ContainsKey(name))
        //         throw new ToolBarNotFoundException();

        //     var oldToolbar = _contextList[name].Item1;
        //     var oldDock = _contextList[name].Item2;

        //     _contextList.Remove(name);
        //     _contextList.Add(name,
        //         new Tuple<ToolBar, Dock, bool>(oldToolbar, oldDock, newValue));
        // }


        // public void SetupToolbars()
        // {
        //     var definitions = _toolbarDefinitions.OrderBy(x => x.SortOrder);
        //     foreach (var definition in definitions)
        //         AddToolBar(definition);
        // }
    }
}