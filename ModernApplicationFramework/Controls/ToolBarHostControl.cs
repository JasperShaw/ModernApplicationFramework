using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Controls.Customize;
using ModernApplicationFramework.Core.Exception;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarHostControl : ContentControl
    {
        public static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register(
            "DefaultBackground", typeof (Brush), typeof (ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TopTrayBackgroundProperty = DependencyProperty.Register(
            "TopTrayBackground", typeof (Brush), typeof (ToolBarHostControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public static ToolBarHostControl Instance { get; private set; }

        private readonly ContextMenu _contextMenu = new ContextMenu();
        private ToolBarTray _bottomToolBarTay;
        private bool _contentLoaded;
        private ToolBarTray _leftToolBarTay;
        private ToolBarTray _rightToolBarTay;
        private ToolBarTray _topToolBarTay;

        public ToolBarHostControl()
        {
            InitializeComponent();
            Instance = this;
        }

        public Brush DefaultBackground
        {
            get { return (Brush) GetValue(DefaultBackgroundProperty); }
            set { SetValue(DefaultBackgroundProperty, value); }
        }

        public Brush TopTrayBackground
        {
            get { return (Brush) GetValue(TopTrayBackgroundProperty); }
            set { SetValue(TopTrayBackgroundProperty, value); }
        }


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

        /// <summary>
        /// Opens ToolbarCustomize Dialog
        /// </summary>
        public void OpenToolBarEditDialog()
        {
            new CustomizeDialog().ShowDialog();
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
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
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
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
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

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
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

        /// <summary>
        /// Returns a list of Toolbar Objects
        /// </summary>
        /// <returns></returns>
        public List<ToolBar> GetToolBars()
        {
            return new List<ToolBar>(_contextList.Values.Select(x => x.Item1).ToList());
        }

        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/ToolbarHostControl.xaml", UriKind.Relative));


            var editItem = new ContextMenuItem {Header = "Edit..."};
            editItem.Click += EditItem_Click;
            _contextMenu.Items.Add(new Separator());
            _contextMenu.Items.Add(editItem);
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            //TODO: If there is no toolbar we get an null exception here
            OpenToolBarEditDialog();
        }

        public override void OnApplyTemplate()
        {
            _topToolBarTay = GetTemplateChild("TopDockTray") as ToolBarTray;
            _leftToolBarTay = GetTemplateChild("LeftDockTray") as ToolBarTray;
            _rightToolBarTay = GetTemplateChild("RightDockTray") as ToolBarTray;
            _bottomToolBarTay = GetTemplateChild("BottomDockTray") as ToolBarTray;
            if (_topToolBarTay != null)
                _topToolBarTay.MouseRightButtonDown += ToolBarTrayMouseRightButtonDown;
            if (_leftToolBarTay != null)
                _leftToolBarTay.MouseRightButtonDown += ToolBarTrayMouseRightButtonDown;
            if (_rightToolBarTay != null)
                _rightToolBarTay.MouseRightButtonDown += ToolBarTrayMouseRightButtonDown;
            if (_bottomToolBarTay != null)
                _bottomToolBarTay.MouseRightButtonDown += ToolBarTrayMouseRightButtonDown;
            base.OnApplyTemplate();
        }

        public void OpenMenu()
        {
            _contextMenu.IsOpen = true;
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
            var item = new ContextMenuGlyphItem {Header = identifierName};
            item.Click += Item_Click;

            if (_contextMenu.Items.Count < 2)
                _contextMenu.Items.Add(item);
            else
                _contextMenu.Items.Insert(_contextMenu.Items.Count - 2, item);
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
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case Dock.Top:
                    _topToolBarTay.RemoveToolBar(toolBar);
                    break;
                case Dock.Left:
                    _leftToolBarTay.RemoveToolBar(toolBar);
                    break;
                case Dock.Right:
                    _rightToolBarTay.RemoveToolBar(toolBar);
                    break;
                default:
                    _bottomToolBarTay.RemoveToolBar(toolBar);
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

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as ContextMenuGlyphItem;
            if (item != null && item.IconGeometry == null)
            {
                SetCheckMark(item);
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

        private static void SetCheckMark(ContextMenuGlyphItem item)
        {
            var converter = TypeDescriptor.GetConverter(typeof (Geometry));
            var geomitry = (Geometry) converter.ConvertFrom("F1 M 5,11 L 3,7 L 5,7 L 6,9 L 9,3 L 11,3 L 7,11 L 5,11 Z");

            item.IconGeometry = geomitry;
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
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case Dock.Top:
                    _topToolBarTay.AddToolBar(toolBar);
                    break;
                case Dock.Left:
                    _leftToolBarTay.AddToolBar(toolBar);
                    break;
                case Dock.Right:
                    _rightToolBarTay.AddToolBar(toolBar);
                    break;
                default:
                    _bottomToolBarTay.AddToolBar(toolBar);
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
            SetCheckMark(_contextList[name].Item4);
        }

        private void ToolBarTrayMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            OpenMenu();
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
    }
}