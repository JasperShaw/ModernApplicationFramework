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

        private readonly Dictionary<string, Tuple<ToolBar, System.Windows.Controls.Dock, bool, ContextMenuGlyphItem>> _contextList =
            new Dictionary<string, Tuple<ToolBar, System.Windows.Controls.Dock, bool, ContextMenuGlyphItem>>();

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

        public void AddToolBar(ToolBar toolBar, bool visible, System.Windows.Controls.Dock dock)
        {
            if (toolBar == null)
                throw new ArgumentNullException("toolBar");
            if (string.IsNullOrEmpty(toolBar.IdentifierName))
                throw new NullReferenceException("Toolbar Id must not be null");
            if (_contextList.ContainsKey(toolBar.IdentifierName))
                throw new ToolBarAlreadyExistsException();
            var item = CreateContextMenuItem(toolBar.IdentifierName);
            _contextList.Add(toolBar.IdentifierName,
                new Tuple<ToolBar, System.Windows.Controls.Dock, bool, ContextMenuGlyphItem>(toolBar, dock, visible, item));
            if (!visible)
                return;
            ShowToolBarByName(toolBar.IdentifierName);
        }

        public void OpenToolBarEditDialog()
        {
            new CustomizeDialog().Show();
        }

        public void ChangeToolBarDock(string name, System.Windows.Controls.Dock newValue)
        {
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            var wasVisible = _contextList[name].Item3;
            HideToolBarByName(name);
            UpdateDock(name, newValue);
            if (wasVisible)
                ShowToolBarByName(name);
        }

        public void ChangeToolBarVisibility(string name, bool newValue)
        {
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
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

        public ToolBar GetToolBar(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item1;
        }

        public System.Windows.Controls.Dock GetToolBarDock(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item2;
        }

        public bool GetToolBarVisibility(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();
            return _contextList[name].Item3;
        }

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

        private void HideToolBar(ToolBar toolBar, System.Windows.Controls.Dock dock)
        {
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case System.Windows.Controls.Dock.Top:
                    _topToolBarTay.RemoveToolBar(toolBar);
                    break;
                case System.Windows.Controls.Dock.Left:
                    _leftToolBarTay.RemoveToolBar(toolBar);
                    break;
                case System.Windows.Controls.Dock.Right:
                    _rightToolBarTay.RemoveToolBar(toolBar);
                    break;
                default:
                    _bottomToolBarTay.RemoveToolBar(toolBar);
                    break;
            }
        }

        private void HideToolBarByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            UpdateVisibility(name, false);
            HideToolBar(_contextList[name].Item1, _contextList[name].Item2);
            _contextList[name].Item4.Icon = null;
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

        private void ShowToolBar(ToolBar toolBar, System.Windows.Controls.Dock dock)
        {
            if (_topToolBarTay == null || _leftToolBarTay == null || _rightToolBarTay == null ||
                _bottomToolBarTay == null)
                throw new NullReferenceException("Could not find all 4 ToolbarTrays");
            switch (dock)
            {
                case System.Windows.Controls.Dock.Top:
                    _topToolBarTay.ShowToolBar(toolBar);
                    break;
                case System.Windows.Controls.Dock.Left:
                    _leftToolBarTay.ShowToolBar(toolBar);
                    break;
                case System.Windows.Controls.Dock.Right:
                    _rightToolBarTay.ShowToolBar(toolBar);
                    break;
                default:
                    _bottomToolBarTay.ShowToolBar(toolBar);
                    break;
            }
        }

        private void ShowToolBarByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            UpdateVisibility(name, true);
            ShowToolBar(_contextList[name].Item1, _contextList[name].Item2);
            SetCheckMark(_contextList[name].Item4);
        }

        private void ToolBarTrayMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            OpenMenu();
        }

        private void UpdateDock(string name, System.Windows.Controls.Dock newValue)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();

            var oldToolbar = _contextList[name].Item1;
            var oldVibility = _contextList[name].Item3;
            var oldMenuItem = _contextList[name].Item4;

            _contextList.Remove(name);
            _contextList.Add(name,
                new Tuple<ToolBar, System.Windows.Controls.Dock, bool, ContextMenuGlyphItem>(oldToolbar, newValue, oldVibility, oldMenuItem));
        }

        private void UpdateVisibility(string name, bool newValue)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!_contextList.ContainsKey(name))
                throw new ToolBarNotFoundException();

            var oldToolbar = _contextList[name].Item1;
            var oldDock = _contextList[name].Item2;
            var oldMenuItem = _contextList[name].Item4;

            _contextList.Remove(name);
            _contextList.Add(name,
                new Tuple<ToolBar, System.Windows.Controls.Dock, bool, ContextMenuGlyphItem>(oldToolbar, oldDock, newValue, oldMenuItem));
        }
    }
}