/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Core.CommandFocus;
using ModernApplicationFramework.Core.MenuModeHelper;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls.Search;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public abstract class LayoutItem : FrameworkElement
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnTitleChanged));

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnIconSourceChanged));

        public static readonly DependencyProperty ContentIdProperty =
            DependencyProperty.Register("ContentId", typeof(string), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnContentIdChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(LayoutItem),
                new FrameworkPropertyMetadata(false,
                    OnIsSelectedChanged));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(LayoutItem),
                new FrameworkPropertyMetadata(false,
                    OnIsActiveChanged));

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(LayoutItem),
                new FrameworkPropertyMetadata(true,
                    OnCanCloseChanged));

        public static readonly DependencyProperty CanFloatProperty =
            DependencyProperty.Register("CanFloat", typeof(bool), typeof(LayoutItem),
                new FrameworkPropertyMetadata(true,
                    OnCanFloatChanged));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseCommandChanged,
                    CoerceCloseCommandValue));

        public static readonly DependencyProperty FloatCommandProperty =
            DependencyProperty.Register("FloatCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnFloatCommandChanged,
                    CoerceFloatCommandValue));

        public static readonly DependencyProperty DockAsDocumentCommandProperty =
            DependencyProperty.Register("DockAsDocumentCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnDockAsDocumentCommandChanged,
                    CoerceDockAsDocumentCommandValue));

        public static readonly DependencyProperty CloseAllButThisCommandProperty =
            DependencyProperty.Register("CloseAllButThisCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseAllButThisCommandChanged,
                    CoerceCloseAllButThisCommandValue));

        public static readonly DependencyProperty ActivateCommandProperty =
            DependencyProperty.Register("ActivateCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnActivateCommandChanged,
                    CoerceActivateCommandValue));

        public static readonly DependencyProperty NewVerticalTabGroupCommandProperty =
            DependencyProperty.Register("NewVerticalTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnNewVerticalTabGroupCommandChanged));

        public static readonly DependencyProperty NewHorizontalTabGroupCommandProperty =
            DependencyProperty.Register("NewHorizontalTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnNewHorizontalTabGroupCommandChanged));

        public static readonly DependencyProperty MoveToNextTabGroupCommandProperty =
            DependencyProperty.Register("MoveToNextTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnMoveToNextTabGroupCommandChanged));

        public static readonly DependencyProperty MoveToPreviousTabGroupCommandProperty =
            DependencyProperty.Register("MoveToPreviousTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnMoveToPreviousTabGroupCommandChanged));

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnAddCommandChanged,
                    CoerceAddCommandValue));


        public static readonly DependencyProperty CloseAllCommandProperty =
            DependencyProperty.Register("CloseAllCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseAllCommandChanged,
                    CoerceCloseAllCommandValue));


        public static readonly DependencyProperty PinCommandProperty =
            DependencyProperty.Register("PinCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnPinCommandChanged,
                    CoercePinCommandValue));



        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.Register("IsFloating",
            typeof(bool), typeof(LayoutItem),
            new UIPropertyMetadata(false));


        private readonly ReentrantFlag _isActiveReentrantFlag = new ReentrantFlag();
        private readonly ReentrantFlag _isSelectedReentrantFlag = new ReentrantFlag();
        private ICommand _defaultActivateCommand;
        private ICommand _defaultAddCommand;
        private ICommand _defaultCloseAllButThisCommand;
        private ICommand _defaultCloseAllCommand;
        private ICommand _defaultCloseCommand;
        private ICommand _defaultDockAsDocumentCommand;
        private ICommand _defaultFloatCommand;
        private ICommand _defaultMoveToNextTabGroupCommand;
        private ICommand _defaultMoveToPreviousTabGroupCommand;
        private ICommand _defaultNewHorizontalTabGroupCommand;
        private ICommand _defaultNewVerticalTabGroupCommand;
        private ICommand _defaultPinCommand;
        private ContentPresenter _view;

        private SearchPlacement _searchControlPlacement = SearchPlacement.Dynamic;
        private uint _searchControlMaxWidth = uint.MaxValue;

        internal LayoutItem()
        {
        }

        static LayoutItem()
        {
            ToolTipProperty.OverrideMetadata(typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnToolTipChanged));
            VisibilityProperty.OverrideMetadata(typeof(LayoutItem),
                new FrameworkPropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        }

        [XmlIgnore]
        public InfoBarHostControl InfoBarHost => AdornmentHost.InfoBarHost;

        private AdornmentHostingPanel _adornmentHost;

        [XmlIgnore]
        private AdornmentHostingPanel AdornmentHost
        {
            get
            {
                if (_adornmentHost != null)
                    return _adornmentHost;
                _adornmentHost = new AdornmentHostingPanel();
                Grid.SetRow(_adornmentHost, 0);
                Grid.SetColumn(_adornmentHost, 0);
                Grid.SetColumnSpan(_adornmentHost, 3);
                HostingPanel.Children.Add(_adornmentHost);
                return _adornmentHost;
            }
        }

        private class AdornmentHostingPanel : StackPanel
        {
            private InfoBarHostControl _infoBarHost;
            private ToolbarTrayAndSearchHostingPanel _searchHostingPanel;

            public InfoBarHostControl InfoBarHost
            {
                get
                {
                    if (_infoBarHost != null)
                        return _infoBarHost;
                    _infoBarHost = new InfoBarHostControl();
                    Children.Add(_infoBarHost);
                    CommandBarNavigationHelper.SetCommandFocusMode(_infoBarHost,
                        CommandBarNavigationHelper.CommandFocusMode.Container);
                    return _infoBarHost;
                }
            }

            public ToolbarTrayAndSearchHostingPanel SearchHostingPanel
            {
                get
                {
                    if (_searchHostingPanel == null)
                    {
                        _searchHostingPanel = new ToolbarTrayAndSearchHostingPanel();
                        Children.Insert(0, _searchHostingPanel);
                    }
                    return _searchHostingPanel;
                }
            }

            public AdornmentHostingPanel()
            {
                SetResourceReference(BackgroundProperty, EnvironmentColors.CommandBarMenuBackgroundGradientBegin);
                Focusable = false;
                KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.Cycle);
            }

            internal bool TryGetInfoBarHostIfCreated(out InfoBarHostControl control)
            {
                control = _infoBarHost;
                return control != null;
            }
        }

        private class ToolbarTrayAndSearchHostingPanel : DockPanel
        {
            private ContentControl _searchControlParent;
            private AnchorableToolBarTray _toolbarTray;

            public ContentControl SearchControlParent
            {
                get => _searchControlParent;
                private set
                {
                    if (_searchControlParent != null)
                        throw new InvalidOperationException();
                    _searchControlParent = value;
                    SearchControlParent.Focusable = false;
                    SearchControlParent.VerticalAlignment = VerticalAlignment.Center;
                    SearchControlParent.Margin = new Thickness(0, 0, 0, 2);
                    Children.Add(SearchControlParent);
                }
            }

            public AnchorableToolBarTray ToolbarTray
            {
                get => _toolbarTray;
                set
                {
                    if (_toolbarTray != null)
                        throw new InvalidOperationException();
                    _toolbarTray = value;
                    ToolbarTray.SetResourceReference(BackgroundProperty, EnvironmentColors.CommandBarGradientBegin);
                    ToolbarTray.HorizontalAlignment = HorizontalAlignment.Left;
                    SetDock(ToolbarTray, Dock.Left);
                    Children.Insert(0, ToolbarTray);
                }
            }

            public ToolbarTrayAndSearchHostingPanel()
            {
                SearchControlParent = new ContentControl();
                SetResourceReference(BackgroundProperty, EnvironmentColors.CommandBarGradientBegin);
                Focusable = false;
            }
        }

        [XmlIgnore]
        internal IWindowSearchHost SearchHost { get; private set; }

        private SearchPlacement SearchControlPlacement
        {
            get
            {
                if (SearchHost == null)
                    return SearchPlacement.None;
                var searchHost = SearchHost as WindowSearchHost;
                if (searchHost == null || AdornmentHost.SearchHostingPanel == null || searchHost.SearchParentControl !=
                    AdornmentHost.SearchHostingPanel.SearchControlParent)
                    return SearchPlacement.Custom;
                return _searchControlPlacement;
            }
            set
            {
                var controlPlacement = SearchControlPlacement;
                if (value == SearchPlacement.Custom)
                {
                    if (value != controlPlacement)
                        throw new InvalidOperationException();

                }
                else
                {
                    if (controlPlacement == SearchPlacement.Custom)
                        throw new InvalidOperationException();
                    _searchControlPlacement = value;
                }
                AdjustSearchControlPlacement();
            }
        }

        internal void CreateSearchHostAndControl()
        {
            if (SearchHost != null)
                return;
            SearchHost = new WindowSearchHost(AdornmentHost.SearchHostingPanel.SearchControlParent);
            ((WindowSearchHost)SearchHost).SearchControl.SetBinding(WidthProperty, new Binding
            {
                Source = AdornmentHost.SearchHostingPanel.SearchControlParent,
                Path = new PropertyPath(ActualWidthProperty.Name),
                Mode = BindingMode.OneWay
            });
            ConnectSearchControlEvents();
            AdjustSearchControlPlacement();
        }


        internal bool TryGetInfoBarHostIfCreated(out InfoBarHostControl control)
        {
            return AdornmentHost.TryGetInfoBarHostIfCreated(out control);
        }


        private void AddTopToolbarTray(AnchorableToolBarTray tray)
        {
            AdornmentHost.SearchHostingPanel.ToolbarTray = tray;
            OnTopToolbarsChanged();
        }

    
        private void OnTopToolbarsChanged()
        {
            AdjustSearchControlPlacement();
            SetToolbarsKeyboardNavigation();
        }

        private void SetToolbarsKeyboardNavigation()
        {
            var tray = AdornmentHost.SearchHostingPanel?.ToolbarTray;
            if (tray == null)
                return;
            KeyboardNavigation.SetControlTabNavigation(tray, KeyboardNavigationMode.Continue);
            foreach (var toolBar in tray.ToolBars)
            {
                KeyboardNavigation.SetTabNavigation(toolBar, KeyboardNavigationMode.Continue);
                KeyboardNavigation.SetDirectionalNavigation(toolBar, KeyboardNavigationMode.Continue);
            }
        }


        private void AdjustSearchControlPlacement()
        {
            if (SearchHost == null)
                return;
            var contolPlacement = SearchControlPlacement;
            if (contolPlacement == SearchPlacement.Custom)
                return;
            var dock = Dock.Top;
            var horizontalAlignment = HorizontalAlignment.Left;
            var flag = false;
            var num2 = double.PositiveInfinity;
            var searchControl = ((WindowSearchHost) SearchHost).SearchControl;

            AdornmentHost.Measure(new Size(double.MaxValue, double.MaxValue));

            if (contolPlacement == SearchPlacement.Dynamic)
            {
                if (AdornmentHost.SearchHostingPanel.ToolbarTray != null)
                {
                    AdornmentHost.SearchHostingPanel.ToolbarTray.MaxWidth = num2;
                    flag = true;
                }
                if (flag && searchControl != null)
                {
                    AdornmentHost.SearchHostingPanel.ToolbarTray.Measure(new Size(double.MaxValue, double.MaxValue));
                    if (AdornmentHost.SearchHostingPanel.ActualWidth - AdornmentHost.SearchHostingPanel.ToolbarTray.DesiredSize.Width >= searchControl.MinWidth)
                    {
                        dock = Dock.Left;
                        horizontalAlignment = HorizontalAlignment.Right;
                        num2 = Math.Max(AdornmentHost.SearchHostingPanel.ToolbarTray.DesiredSize.Width,
                            AdornmentHost.SearchHostingPanel.ActualWidth - _searchControlMaxWidth);
                    }
                }
            }

            if (searchControl != null)
            {
                searchControl.HorizontalAlignment = horizontalAlignment;
                AdjustSearchControlMaxWidth();
            }
            if (AdornmentHost.SearchHostingPanel.ToolbarTray == null)
                return;
            AdornmentHost.SearchHostingPanel.ToolbarTray.MaxWidth = num2;
            DockPanel.SetDock(AdornmentHost.SearchHostingPanel.ToolbarTray, dock);
        }

        private void AdjustSearchControlMaxWidth()
        {
            var searchControl = ((WindowSearchHost)SearchHost).SearchControl;
            var num = uint.MaxValue;
            if (searchControl.HorizontalAlignment == HorizontalAlignment.Right)
                num = _searchControlMaxWidth;
            var dataContext = searchControl.DataContext as SearchControlDataSource;
            if (dataContext == null)
                return;
            dataContext.SearchSettings.ControlMaxWidth = num;
        }

        private void ConnectSearchControlEvents()
        {
            if (SearchHost == null)
                return;
            AdornmentHost.SearchHostingPanel.SizeChanged += SearchHostingPanel_SizeChanged;
            Control searchControl = ((WindowSearchHost)SearchHost).SearchControl;
            if (searchControl == null)
                return;
            CommandBarNavigationHelper.SetCommandFocusMode(searchControl, CommandBarNavigationHelper.CommandFocusMode.Container);
        }

        private void SearchHostingPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustSearchControlPlacement();
        }

        private void InitializeSearchControlMaxWidth()
        {
            _searchControlMaxWidth =
                ((SearchControlDataSource) ((WindowSearchHost) SearchHost).SearchControl.DataContext)
                .SearchSettings.ControlMaxWidth;
        }

        public bool FocusSearchControl()
        {
            if (!IsVisible)
                ExecuteActivateCommand(null);
            var searchHost = (WindowSearchHost) SearchHost;
            var searchControl = searchHost?.SearchControl;
            if (searchControl == null || !searchHost.IsVisible || !searchHost.IsEnabled)
                return false;
            PendingFocusHelper.SetFocusOnLoad(searchControl);
            return true;
        }

        public IWindowSearchHost GetSearchHost()
        {
            CreateSearchHostAndControl();
            return SearchHost;
        }

        public void SetupSearch(IWindowSearch windowSearch)
        {
            if (!windowSearch.SearchEnabled)
                return;

            CreateSearchHostAndControl();
            SearchHost.SetupSearch(windowSearch);
            SearchControlPlacement = windowSearch.SearchControlPlacement;
            InitializeSearchControlMaxWidth();
            AdjustSearchControlMaxWidth();
        }

        public void AddToolbar(IToolbarProvider provider)
        {
            if (!provider.HasToolbar || provider.Toolbar == null || provider.Toolbar.ToolbarScope != ToolbarScope.Anchorable)
                return;

            var t = new AnchorableToolBarTray();
            var toolbar = new ModernApplicationFramework.Controls.ToolBar(provider.Toolbar);

            var tc = IoC.Get<IToolbarCreator>();
            tc?.CreateRecursive(ref toolbar, provider.Toolbar, provider.Toolbar.ContainedGroups.ToList(), group => group.Items);

            t.AddToolBar(toolbar);
            AddTopToolbarTray(t);
        }

        public void SetSearchPlacement(SearchPlacement placement)
        {
            SearchControlPlacement = placement;
        }

        public SearchPlacement GetSearchPlacement()
        {
            return SearchControlPlacement;
        }


















        public ContentPresenter View
        {
            get
            {
                if (_view != null)
                {
                    return _view;
                }
                _view = new ContentPresenter();
                var layoutElementRoot = LayoutElement.Root;
                if (layoutElementRoot != null)
                {
                    var dockingManager = layoutElementRoot.Manager;

                    _view.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { Source = LayoutElement });
                    _view.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding("LayoutItemTemplate") { Source = dockingManager });
                    _view.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding("LayoutItemTemplateSelector") { Source = dockingManager });
                    dockingManager.InternalAddLogicalChild(_view);
                }

                return _view;
            }
        }

        public ICommand ActivateCommand
        {
            get => (ICommand)GetValue(ActivateCommandProperty);
            set => SetValue(ActivateCommandProperty, value);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public bool CanClose
        {
            get => (bool)GetValue(CanCloseProperty);
            set => SetValue(CanCloseProperty, value);
        }

        public bool CanFloat
        {
            get => (bool)GetValue(CanFloatProperty);
            set => SetValue(CanFloatProperty, value);
        }

        public ICommand CloseAllButThisCommand
        {
            get => (ICommand)GetValue(CloseAllButThisCommandProperty);
            set => SetValue(CloseAllButThisCommandProperty, value);
        }

        public ICommand CloseAllCommand
        {
            get => (ICommand)GetValue(CloseAllCommandProperty);
            set => SetValue(CloseAllCommandProperty, value);
        }

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public string ContentId
        {
            get => (string)GetValue(ContentIdProperty);
            set => SetValue(ContentIdProperty, value);
        }

        public ICommand DockAsDocumentCommand
        {
            get => (ICommand)GetValue(DockAsDocumentCommandProperty);
            set => SetValue(DockAsDocumentCommandProperty, value);
        }

        public ICommand FloatCommand
        {
            get => (ICommand)GetValue(FloatCommandProperty);
            set => SetValue(FloatCommandProperty, value);
        }

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public ICommand PinCommand
        {
            get => (ICommand)GetValue(PinCommandProperty);
            set => SetValue(PinCommandProperty, value);
        }

        public bool IsFloating
        {
            [ExcludeFromCodeCoverage]
            get { return (bool)GetValue(IsFloatingProperty); }

            [ExcludeFromCodeCoverage]
            set { SetValue(IsFloatingProperty, value); }
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public LayoutContent LayoutElement { get; private set; }
        public object Model { get; private set; }

        public ICommand MoveToNextTabGroupCommand
        {
            get => (ICommand)GetValue(MoveToNextTabGroupCommandProperty);
            set => SetValue(MoveToNextTabGroupCommandProperty, value);
        }

        public ICommand MoveToPreviousTabGroupCommand
        {
            get => (ICommand)GetValue(MoveToPreviousTabGroupCommandProperty);
            set => SetValue(MoveToPreviousTabGroupCommandProperty, value);
        }

        public ICommand NewHorizontalTabGroupCommand
        {
            get => (ICommand)GetValue(NewHorizontalTabGroupCommandProperty);
            set => SetValue(NewHorizontalTabGroupCommandProperty, value);
        }

        public ICommand NewVerticalTabGroupCommand
        {
            get => (ICommand)GetValue(NewVerticalTabGroupCommandProperty);
            set => SetValue(NewVerticalTabGroupCommandProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        protected virtual void ClearDefaultBindings()
        {
            if (AddCommand == _defaultAddCommand)
                BindingOperations.ClearBinding(this, AddCommandProperty);
            if (CloseAllCommand == _defaultCloseAllCommand)
                BindingOperations.ClearBinding(this, CloseAllCommandProperty);
            if (CloseCommand == _defaultCloseCommand)
                BindingOperations.ClearBinding(this, CloseCommandProperty);
            if (FloatCommand == _defaultFloatCommand)
                BindingOperations.ClearBinding(this, FloatCommandProperty);
            if (DockAsDocumentCommand == _defaultDockAsDocumentCommand)
                BindingOperations.ClearBinding(this, DockAsDocumentCommandProperty);
            if (CloseAllButThisCommand == _defaultCloseAllButThisCommand)
                BindingOperations.ClearBinding(this, CloseAllButThisCommandProperty);
            if (ActivateCommand == _defaultActivateCommand)
                BindingOperations.ClearBinding(this, ActivateCommandProperty);
            if (NewVerticalTabGroupCommand == _defaultNewVerticalTabGroupCommand)
                BindingOperations.ClearBinding(this, NewVerticalTabGroupCommandProperty);
            if (NewHorizontalTabGroupCommand == _defaultNewHorizontalTabGroupCommand)
                BindingOperations.ClearBinding(this, NewHorizontalTabGroupCommandProperty);
            if (MoveToNextTabGroupCommand == _defaultMoveToNextTabGroupCommand)
                BindingOperations.ClearBinding(this, MoveToNextTabGroupCommandProperty);
            if (MoveToPreviousTabGroupCommand == _defaultMoveToPreviousTabGroupCommand)
                BindingOperations.ClearBinding(this, MoveToPreviousTabGroupCommandProperty);
            if (PinCommand == _defaultPinCommand)
                BindingOperations.ClearBinding(this, PinCommandProperty);
        }

        protected abstract void Close(object parameter);

        protected abstract void Pin();

        protected virtual void Float()
        {
        }

        protected virtual void InitDefaultCommands()
        {
            _defaultAddCommand = new DelegateCommand(ExecuteAddCommand, CanExecuteAddCommand);
            _defaultCloseAllCommand = new DelegateCommand(p => ExecuteCloseAllCommand(),
                p => CanExecuteCloseAllCommand());
            _defaultCloseCommand = new DelegateCommand(ExecuteCloseCommand, CanExecuteCloseCommand);
            _defaultFloatCommand = new DelegateCommand(ExecuteFloatCommand, CanExecuteFloatCommand);
            _defaultDockAsDocumentCommand = new DelegateCommand(ExecuteDockAsDocumentCommand,
                CanExecuteDockAsDocumentCommand);
            _defaultCloseAllButThisCommand = new DelegateCommand(ExecuteCloseAllButThisCommand,
                CanExecuteCloseAllButThisCommand);
            _defaultActivateCommand = new DelegateCommand(ExecuteActivateCommand, CanExecuteActivateCommand);
            _defaultNewVerticalTabGroupCommand = new DelegateCommand(ExecuteNewVerticalTabGroupCommand,
                CanExecuteNewVerticalTabGroupCommand);
            _defaultNewHorizontalTabGroupCommand = new DelegateCommand(ExecuteNewHorizontalTabGroupCommand,
                CanExecuteNewHorizontalTabGroupCommand);
            _defaultMoveToNextTabGroupCommand = new DelegateCommand(ExecuteMoveToNextTabGroupCommand,
                CanExecuteMoveToNextTabGroupCommand);
            _defaultMoveToPreviousTabGroupCommand = new DelegateCommand(ExecuteMoveToPreviousTabGroupCommand,
                CanExecuteMoveToPreviousTabGroupCommand);
            _defaultPinCommand = new DelegateCommand(ExecutePinCommand, CanExecutePinCommand);
        }

        private bool CanExecutePinCommand(object obj)
        {
            return LayoutElement?.Parent is LayoutDocumentPane;
        }

        private void ExecutePinCommand(object obj)
        {
            Pin();
        }

        protected virtual void OnActivateCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAddCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCanCloseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.CanClose = (bool)e.NewValue;
        }

        protected virtual void OnCanFloatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.CanFloat = (bool)e.NewValue;
        }

        protected virtual void OnCloseAllButThisCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCloseAllCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCloseCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnPinCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnContentIdChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.ContentId = (string)e.NewValue;
        }

        protected virtual void OnDockAsDocumentCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnFloatCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnIconSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.IconSource = IconSource;
        }

        protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isActiveReentrantFlag.CanEnter)
            {
                using (_isActiveReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                        LayoutElement.IsActive = (bool)e.NewValue;
                }
            }
        }

        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isSelectedReentrantFlag.CanEnter)
            {
                using (_isSelectedReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                        LayoutElement.IsSelected = (bool)e.NewValue;
                }
            }
        }

        protected virtual void OnMoveToNextTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnMoveToPreviousTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnNewHorizontalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnNewVerticalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.Title = (string)e.NewValue;
        }

        protected virtual void OnVisibilityChanged()
        {
            if (LayoutElement != null &&
                Visibility == Visibility.Collapsed)
                LayoutElement.Close();
        }

        protected virtual void SetDefaultBindings()
        {
            if (CloseCommand == null)
                CloseCommand = _defaultCloseCommand;
            if (FloatCommand == null)
                FloatCommand = _defaultFloatCommand;
            if (DockAsDocumentCommand == null)
                DockAsDocumentCommand = _defaultDockAsDocumentCommand;
            if (CloseAllButThisCommand == null)
                CloseAllButThisCommand = _defaultCloseAllButThisCommand;
            if (ActivateCommand == null)
                ActivateCommand = _defaultActivateCommand;
            if (NewVerticalTabGroupCommand == null)
                NewVerticalTabGroupCommand = _defaultNewVerticalTabGroupCommand;
            if (NewHorizontalTabGroupCommand == null)
                NewHorizontalTabGroupCommand = _defaultNewHorizontalTabGroupCommand;
            if (MoveToNextTabGroupCommand == null)
                MoveToNextTabGroupCommand = _defaultMoveToNextTabGroupCommand;
            if (MoveToPreviousTabGroupCommand == null)
                MoveToPreviousTabGroupCommand = _defaultMoveToPreviousTabGroupCommand;
            if (AddCommand == null)
                AddCommand = _defaultAddCommand;
            if (CloseAllCommand == null)
                CloseAllCommand = _defaultCloseAllCommand;
            if (PinCommand == null)
                PinCommand = _defaultPinCommand;


            IsSelected = LayoutElement.IsSelected;
            IsActive = LayoutElement.IsActive;
        }

        internal void _ClearDefaultBindings()
        {
            ClearDefaultBindings();
        }

        internal void _SetDefaultBindings()
        {
            SetDefaultBindings();
        }

        internal virtual void Attach(LayoutContent model)
        {
            LayoutElement = model;
            Model = model.Content;
            InitDefaultCommands();
            LayoutElement.IsSelectedChanged += LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged += LayoutElement_IsActiveChanged;
            DataContext = this;
            Trace.WriteLine($"Attach({LayoutElement.Title})");
        }

        internal virtual void Detach()
        {
            Trace.WriteLine($"Detach({LayoutElement.Title})");
            LayoutElement.IsSelectedChanged -= LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged -= LayoutElement_IsActiveChanged;
            LayoutElement = null;
            Model = null;
        }

        private static object CoerceActivateCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceAddCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseAllButThisCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseAllCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDockAsDocumentCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceFloatCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoercePinCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnActivateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnActivateCommandChanged(e);
        }

        private static void OnAddCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnAddCommandChanged(e);
        }

        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnCanCloseChanged(e);
        }

        private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnCanFloatChanged(e);
        }

        private static void OnCloseAllButThisCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnCloseAllButThisCommandChanged(e);
        }

        private static void OnCloseAllCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnCloseAllCommandChanged(e);
        }

        private static void OnCloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnCloseCommandChanged(e);
        }

        private static void OnContentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnContentIdChanged(e);
        }

        private static void OnDockAsDocumentCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnDockAsDocumentCommandChanged(e);
        }

        private static void OnFloatCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnFloatCommandChanged(e);
        }

        private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnIconSourceChanged(e);
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnIsActiveChanged(e);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnIsSelectedChanged(e);
        }

        private static void OnMoveToNextTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnMoveToNextTabGroupCommandChanged(e);
        }

        private static void OnMoveToPreviousTabGroupCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnMoveToPreviousTabGroupCommandChanged(e);
        }

        private static void OnNewHorizontalTabGroupCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnNewHorizontalTabGroupCommandChanged(e);
        }

        private static void OnNewVerticalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnNewVerticalTabGroupCommandChanged(e);
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnTitleChanged(e);
        }

        private static void OnToolTipChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)s).OnToolTipChanged();
        }

        private static void OnVisibilityChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)s).OnVisibilityChanged();
        }

        private static void OnPinCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).OnPinCommandChanged(e);
        }

        private bool CanExecuteActivateCommand(object parameter)
        {
            return LayoutElement != null;
        }

        private bool CanExecuteAddCommand(object parameter)
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;

            if (!LayoutElement.Root.Manager.CanAdd)
            {
                return false;
            }

            return
                LayoutElement.Root.Manager.Layout.Descendents()
                    .OfType<LayoutContent>()
                    .Any(
                        d =>
                            !Equals(d, LayoutElement) &&
                            (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow));
        }

        private bool CanExecuteCloseAllButThisCommand(object parameter)
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;

            if (!LayoutElement.Root.Manager.CanCloseAllButThis)
                return false;

            return LayoutElement.Root.Manager.Layout.
                Descendents()
                .OfType<LayoutContent>()
                .Any(
                    d =>
                        // I know this does not make much sense but VS behaves like this...
                        //!Equals(d, LayoutElement) &&
                        (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow));
        }

        private bool CanExecuteCloseAllCommand()
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;
            if (!LayoutElement.Root.Manager.CanCloseAll)
                return false;

            return
                LayoutElement.Root.Manager.Layout
                    .Descendents()
                    .OfType<LayoutContent>()
                    .Any(d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        private bool CanExecuteCloseCommand(object parameter)
        {
#if DEBUG
            if (LayoutElement != null)
                Trace.WriteLine(
                    $"CanExecuteCloseCommand({LayoutElement.Title}) = {LayoutElement.CanClose}");
#endif
            return LayoutElement != null && LayoutElement.CanClose;
        }

        private bool CanExecuteDockAsDocumentCommand(object parameter)
        {
            return LayoutElement != null && LayoutElement.FindParent<LayoutDocumentPane>() == null;
        }

        private bool CanExecuteFloatCommand(object anchorable)
        {
            return LayoutElement != null && LayoutElement.CanFloat &&
                   LayoutElement.FindParent<LayoutFloatingWindow>() == null;
        }

        private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;

            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            return parentDocumentGroup != null &&
                   LayoutElement.Parent is LayoutDocumentPane parentDocumentPane &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is
                       LayoutDocumentPane;
        }

        private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            return parentDocumentGroup != null &&
                   LayoutElement.Parent is LayoutDocumentPane parentDocumentPane &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is
                       LayoutDocumentPane;
        }

        private bool CanExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            return (parentDocumentGroup == null ||
                    parentDocumentGroup.ChildrenCount == 1 ||
                    parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                    parentDocumentGroup.Orientation == Orientation.Vertical) &&
                   LayoutElement.Parent is LayoutDocumentPane parentDocumentPane &&
                   parentDocumentPane.ChildrenCount > 1;
        }

        private bool CanExecuteNewVerticalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            return (parentDocumentGroup == null ||
                    parentDocumentGroup.ChildrenCount == 1 ||
                    parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                    parentDocumentGroup.Orientation == Orientation.Horizontal) &&
                   LayoutElement.Parent is LayoutDocumentPane parentDocumentPane &&
                   parentDocumentPane.ChildrenCount > 1;
        }

        private void ExecuteActivateCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteContentActivateCommand(LayoutElement);
        }

        private void ExecuteAddCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteAddCommand(LayoutElement);
        }

        private void ExecuteCloseAllButThisCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteCloseAllButThisCommand(LayoutElement);
        }

        private void ExecuteCloseAllCommand()
        {
            LayoutElement.Root.Manager._ExecuteCloseAllCommand();
        }

        private void ExecuteCloseCommand(object parameter)
        {
            Close(parameter);
        }

        private void ExecuteDockAsDocumentCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteDockAsDocumentCommand(LayoutElement);
            IsFloating = false;
        }

        private void ExecuteFloatCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteFloatCommand(LayoutElement);
            IsFloating = true;
        }

        private void ExecuteMoveToNextTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane + 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup() { Orientation = Orientation.Vertical };
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Vertical;
                var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteNewVerticalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup() { Orientation = Orientation.Horizontal };
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Horizontal;
                var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void LayoutElement_IsActiveChanged(object sender, EventArgs e)
        {
            if (!_isActiveReentrantFlag.CanEnter)
                return;
            using (_isActiveReentrantFlag.Enter())
            {
                BindingOperations.GetBinding(this, IsActiveProperty);
                IsActive = LayoutElement.IsActive;
                BindingOperations.GetBinding(this, IsActiveProperty);

                // Additional focus changing required when activating a layout.
                // Makes sure when clicking the Search Control of a other LayoutItem the old layout item
                // does not get reactivated when leaving the menu mode
                if (IsActive)
                {
                    EnsureFocused();
                }
            }
        }

        internal void EnsureFocused()
        {
            //// This line prevents some odd things on app startup that might relate to the GC. 
            if (HostingPanel.Content == null)
                return;
            CommandFocusManager.CancelRestoreFocus();
            PendingFocusHelper.SetFocusOnLoad(HostingPanel.Content);

            FocusHelper.MoveFocusInto(HostingPanel.Content);
            var presentationSource = PresentationSource.FromVisual(HostingPanel.Content);
            if (presentationSource == null)
                return;
            var rootVisual = presentationSource.RootVisual as IInputElement;
            Keyboard.Focus(rootVisual);
            FocusManager.SetFocusedElement(presentationSource.RootVisual, null);
        }


        private void LayoutElement_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_isSelectedReentrantFlag.CanEnter)
                return;
            using (_isSelectedReentrantFlag.Enter())
            {
                IsSelected = LayoutElement.IsSelected;
            }
        }

        private void OnToolTipChanged()
        {
            if (LayoutElement != null)
                LayoutElement.ToolTip = ToolTip;
        }


        private FrameworkElement _contentControl;


        [XmlIgnore]
        public FrameworkElement Content
        {
            get => _contentControl;
            set
            {
                if (value == null)
                {
                    _contentControl = null;
                }
                else
                {
                    if (value is ContentHostingPanel)
                        _contentControl = value;
                    else
                        HostingPanel.Content = value;
                }
            }
        }

        [XmlIgnore]
        protected ContentHostingPanel HostingPanel
        {
            get
            {
                if (_contentControl == null)
                    _contentControl = new ContentHostingPanel();
                return _contentControl as ContentHostingPanel;
            }
        }

        protected class ContentHostingPanel : Grid
        {
            private FrameworkElement _content;

            public FrameworkElement Content
            {
                get => _content;
                set
                {
                    if (_content != null)
                        Children.Remove(_content);
                    _content = null;
                    if (value == null)
                        return;
                    if (value.Parent is Panel parent)
                        parent.Children.Remove(value);
                    _content = value;
                    SetRow(_content, 1);
                    SetColumn(_content, 1);
                    Children.Add(_content);
                }
            }

            public ContentHostingPanel()
            {
                RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(0.0, GridUnitType.Auto)
                });
                RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1.0, GridUnitType.Star)
                });
                RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(0.0, GridUnitType.Auto)
                });
                ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(0.0, GridUnitType.Auto)
                });
                ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1.0, GridUnitType.Star)
                });
                ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(0.0, GridUnitType.Auto)
                });
            }
        }
    }
}