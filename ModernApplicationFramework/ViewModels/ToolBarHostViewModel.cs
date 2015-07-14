using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Customize;
using ModernApplicationFramework.Core.Exception;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;
using Separator = ModernApplicationFramework.Controls.Separator;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;
using ToolBarTray = ModernApplicationFramework.Controls.ToolBarTray;

namespace ModernApplicationFramework.ViewModels
{
    public class ToolBarHostViewModel : ViewModelBase
    {
        /*
            This Dictionary contains all the information needed to interact with the toolbar across classes
            This Dictionary contains as Key:
                IdName of toolbar
            And as Value
                a Tuple with 4 Items:
                    Item1: Toolbar-Object
                    Item2: Toolbar-Dock orientation
                    Item3: Toolbar-Visibility
                    Item4: Toolbar-Linked ContextMenuItem  
        */

        private readonly Dictionary<string, Tuple<ToolBar, Dock, bool, ContextMenuGlyphItem>> _contextList =
            new Dictionary<string, Tuple<ToolBar, Dock, bool, ContextMenuGlyphItem>>();

        private ToolBarTray _bottomToolBarTay;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        private MainWindowViewModel _mainWindowViewModel;

        public ToolBarHostViewModel(ToolBarHostControl toolBarHostControl)
        {
            ToolBarHostControl = toolBarHostControl;
            ContextMenu = new ContextMenu();
            SetupContextMenu();
        }

        public ToolBarHostControl ToolBarHostControl { get; }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            internal set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        internal ToolBarTray BottomToolBarTay
        {
            get { return _bottomToolBarTay; }
            set
            {
                _bottomToolBarTay = value;
                _bottomToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        internal ToolBarTray LeftToolBarTay
        {
            get { return _leftToolBarTay; }
            set
            {
                _leftToolBarTay = value;
                _leftToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        internal ToolBarTray RightToolBarTay
        {
            get { return _rightToolBarTay; }
            set
            {
                _rightToolBarTay = value;
                _rightToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        internal ToolBarTray TopToolBarTay
        {
            get { return _topToolBarTay; }
            set
            {
                _topToolBarTay = value;
                _topToolBarTay.MouseRightButtonDown += ToolBarTay_MouseRightButtonDown;
            }
        }

        private void ToolBarTay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenContextMenuCommand.Execute(null);
        }

        private ContextMenu ContextMenu { get; }
        /*
            This adds a new toolbar by:
                Checking for:
                    Not Null Object
                    ObjectID not null
                    Not Existing already
                Creating a new ContextMenuItem for it
                Adds to Dictionary
                If visible param is true
                    Show Toolbar on screen
        */

        /// <summary>
        /// Adds new Toolbar to HostControl
        /// </summary>
        /// <param name="toolBar">Toolbar object</param>
        /// <param name="visible">Toolbar visibility</param>
        /// <param name="dock">Toolbar orientation</param>
        public void AddToolBar(ToolBar toolBar, bool visible, Dock dock)
        {
            if (toolBar == null)
                throw new ArgumentNullException(nameof(toolBar));
            if (string.IsNullOrEmpty(toolBar.IdentifierName))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (_contextList.ContainsKey(toolBar.IdentifierName))
                throw new ToolBarAlreadyExistsException();
            var item = CreateContextMenuItem(toolBar.IdentifierName);
            _contextList.Add(toolBar.IdentifierName,
                new Tuple<ToolBar, Dock, bool, ContextMenuGlyphItem>(toolBar, dock, visible, item));
            if (!visible)
                return;
            ShowToolBarByName(toolBar.IdentifierName);
        }

        /*
            This removes a Toolbar by:
                Checking for:
                    Name not null
                If Tooblar not exists
                    Do nothing
                Else
                    Make invisible
                    Remove Context Menu Entry
                    Remove from Tuple
        */
        /// <summary>
        /// Removes Toolbar from ToolBarHostControl
        /// </summary>
        /// <param name="name"></param>
        public void RemoveToolBar(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                return;
            ChangeToolBarVisibility(name, false);
            ContextMenu.Items.Remove(_contextList[name].Item4);
            _contextList.Remove(name);
        }

        /*
            Change Orientation of Toolbar by:
                Checking for:
                    At least one Tray exists
                    IdName not null
                    Toolbar exists
                Save visibility
                HideToolbar
                Change Dock
                If was Visible
                    Show Again
        */

        /// <summary>
        /// Change Orientation of Toolbar
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <param name="newValue">New Orientation Value</param>
        public void ChangeToolBarDock(string name, Dock newValue)
        {
            if (TopToolBarTay == null || LeftToolBarTay == null || RightToolBarTay == null ||
                BottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            var wasVisible = _contextList[name].Item3;
            HideToolBarByName(name);
            UpdateDock(name, newValue);
            if (wasVisible)
                ShowToolBarByName(name);
        }

        /*
            Change Visibility of Toolbar by:
                 Checking for:
                    At least one Tray exists
                    IdName not null
                    Toolbar exists
                If newValue true
                    Show Toolbar
                Else
                    Hide Toolbar  
        */

        /// <summary>
        /// Change Visibility of Toolbar
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <param name="newValue">New Visibility Value</param>
        public void ChangeToolBarVisibility(string name, bool newValue)
        {
            if (TopToolBarTay == null || LeftToolBarTay == null || RightToolBarTay == null ||
                BottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            if (newValue)
                ShowToolBarByName(name);
            else
                HideToolBarByName(name);
        }

        /// <summary>
        /// Get a Toolbar by Name
        /// </summary>
        /// <param name="name">IdentifierName of Toolbar</param>
        /// <returns>Found Toolbar Object</returns>
        public ToolBar GetToolBar(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item1;
        }

        /// <summary>
        /// Get Orientation of Toolbar
        /// </summary>
        /// <param name="name">Identifier Name of Toolbar</param>
        /// <returns>Orientation</returns>
        public Dock GetToolBarDock(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item2;
        }

        /// <summary>
        /// Returns a list of Toolbar Objects
        /// </summary>
        /// <returns></returns>
        public List<ToolBar> GetToolBars()
        {
            return new List<ToolBar>(_contextList.Values.Select(x => x.Item1).ToList());
        }

        /// <summary>
        /// Get Toolbar Visibility
        /// </summary>
        /// <param name="name">Identifier Name of Toolbar</param>
        /// <returns>Bool of visibility</returns>
        public bool GetToolBarVisibility(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item3;
        }

        /*
            Returns a toolbar specific MenuItem by:
                Creating the Item
                Header is IdentifierName of Toolbar
                Creats Click Event
                Adds into Menu
                Returns Item
        */

        private ContextMenuGlyphItem CreateContextMenuItem(string identifierName)
        {
            var item = new ContextMenuGlyphItem
            {
                Header = identifierName,
                Command = ClickContextMenuItemCommand
            };
            item.CommandParameter = item;
            if (ContextMenu.Items.Count < 2)
                ContextMenu.Items.Add(item);
            else
                ContextMenu.Items.Insert(ContextMenu.Items.Count - 2, item);
            return item;
        }

        /*
            Hides Toolbar by:
                Check for:
                    At least one tray exists
                Decide Dock
                Remove From Tray
        */

        private void HideToolBar(ToolBar toolBar, Dock dock)
        {
            if (TopToolBarTay == null || LeftToolBarTay == null || RightToolBarTay == null ||
                BottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case Dock.Top:
                    TopToolBarTay.RemoveToolBar(toolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTay.RemoveToolBar(toolBar);
                    break;
                case Dock.Right:
                    RightToolBarTay.RemoveToolBar(toolBar);
                    break;
                default:
                    BottomToolBarTay.RemoveToolBar(toolBar);
                    break;
            }
        }

        /*
            Hides Toolbar from IdName by:
                Check for:
                    name not null/empty
                UpdateVisibility
                Hide ( Object and Orientation)
                Remove Checkmark from MenuItem
        */

        private void HideToolBarByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            UpdateVisibility(name, false);
            HideToolBar(_contextList[name].Item1, _contextList[name].Item2);
            _contextList[name].Item4.IconGeometry = null;
        }

        private void SetupContextMenu()
        {
            var editItem = new ContextMenuItem
            {
                Header = "Edit...",
                Command = OpenCostumizeDialogCommand
            };
            ContextMenu.Items.Add(new Separator());
            ContextMenu.Items.Add(editItem);
        }

        /*
            Show Toolbar by:
                Check for:
                    At least one tray exists
                Decide Dock
                Add Toolbar to tray
        */

        private void ShowToolBar(ToolBar toolBar, Dock dock)
        {
            if (TopToolBarTay == null || LeftToolBarTay == null || RightToolBarTay == null ||
                BottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case Dock.Top:
                    TopToolBarTay.AddToolBar(toolBar);
                    break;
                case Dock.Left:
                    LeftToolBarTay.AddToolBar(toolBar);
                    break;
                case Dock.Right:
                    RightToolBarTay.AddToolBar(toolBar);
                    break;
                default:
                    BottomToolBarTay.AddToolBar(toolBar);
                    break;
            }
        }

        /*
            Show Toolbar with IdName by:
                Check for: 
                    name not null/empty
                UpdateVisibility
                Show Toolbar (Object Orientation)
                Set Checkmark for MenuItem
        */

        private void ShowToolBarByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            UpdateVisibility(name, true);
            ShowToolBar(_contextList[name].Item1, _contextList[name].Item2);
            Controls.Utilities.ContextMenuGlyphItemUtilities.SetCheckMark(_contextList[name].Item4);
        }

        /*
            Update Dock by:
                Check for:
                    Name not null/empty
                    Toolbar exists
                Save Toolbar Object, Visibility, MenuItem
                Remove toolbar from Dictionary
                Add new Entry to Dictionary by using:
                    just saved values 
                    new Dock value (param)
        */

        private void UpdateDock(string name, Dock newValue)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();

            var oldToolbar = _contextList[name].Item1;
            var oldVibility = _contextList[name].Item3;
            var oldMenuItem = _contextList[name].Item4;

            _contextList.Remove(name);
            _contextList.Add(name,
                new Tuple<ToolBar, Dock, bool, ContextMenuGlyphItem>(oldToolbar, newValue, oldVibility, oldMenuItem));
        }

        /*
            Update Visibility by:
                Check for:
                    Name not null/empty
                    Toolbar exists
                Save Toolbar Object, Dock, MenuItem
                Remove toolbar from Dictionary
                Add new Entry to Dictionary by using:
                    just saved values 
                    new Visibility value (param)
        */

        private void UpdateVisibility(string name, bool newValue)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();

            var oldToolbar = _contextList[name].Item1;
            var oldDock = _contextList[name].Item2;
            var oldMenuItem = _contextList[name].Item4;

            _contextList.Remove(name);
            _contextList.Add(name,
                new Tuple<ToolBar, Dock, bool, ContextMenuGlyphItem>(oldToolbar, oldDock, newValue, oldMenuItem));
        }

        #region Commands

        public ICommand OpenContextMenuCommand => new Command(OpenContextMenu, CanOpenContextMenu);

        protected virtual void OpenContextMenu()
        {
            ContextMenu.IsOpen = true;
        }

        protected virtual bool CanOpenContextMenu()
        {
            return true;
        }

        public ICommand ClickContextMenuItemCommand => new Command<ContextMenuGlyphItem>(ClickContextMenuItem, CanClickContextMenuItem);

        protected virtual void ClickContextMenuItem(ContextMenuGlyphItem contextMenuItem)
        {
            var item = contextMenuItem;
            if (item != null && item.IconGeometry == null)
            {
                Controls.Utilities.ContextMenuGlyphItemUtilities.SetCheckMark(item);
                ShowToolBarByName(item.Header.ToString());
            }
            else
            {
                if (item == null)
                    return;
                item.IconGeometry = null;
                HideToolBarByName(item.Header.ToString());
            }
        }

        protected virtual bool CanClickContextMenuItem(ContextMenuGlyphItem item)
        {
            return true;
        }

        public ICommand OpenCostumizeDialogCommand => new Command(OpenCostumizeDialog, CanOpenCostumizeDialog);

        protected virtual void OpenCostumizeDialog()
        {
            new CustomizeDialog(this).ShowDialog();
        }

        protected virtual bool CanOpenCostumizeDialog()
        {
            return true;
        }
        #endregion
    }
}