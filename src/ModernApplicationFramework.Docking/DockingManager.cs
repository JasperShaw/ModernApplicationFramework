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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking.ContextMenuDefinitions;
using ModernApplicationFramework.Docking.ContextMenuProviders;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
using Action = System.Action;

namespace ModernApplicationFramework.Docking
{
    /// <inheritdoc cref="Control" />
    /// <summary>
    ///     This is the avalon docking manager. For full documentation of all the docking framework please consider their
    ///     website
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Control" />
    /// <seealso cref="T:ModernApplicationFramework.Docking.Controls.IOverlayWindowHost" />
    [ContentProperty("Layout")]
    [TemplatePart(Name = "PART_AutoHideArea")]
    public class DockingManager : Control, IOverlayWindowHost //, ILogicalChildrenContainer
    {
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(LayoutRoot), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutChanged, CoerceLayoutValue));

        public static readonly DependencyProperty LayoutUpdateStrategyProperty =
            DependencyProperty.Register("LayoutUpdateStrategy", typeof(ILayoutUpdateStrategy), typeof(DockingManager),
                new FrameworkPropertyMetadata((ILayoutUpdateStrategy)null));

        public static readonly DependencyProperty DocumentPaneTemplateProperty =
            DependencyProperty.Register("DocumentPaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneTemplateChanged));

        public static readonly DependencyProperty AnchorablePaneTemplateProperty =
            DependencyProperty.Register("AnchorablePaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorablePaneTemplateChanged));

        public static readonly DependencyProperty AnchorGroupTemplateProperty =
            DependencyProperty.Register("AnchorGroupTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        public static readonly DependencyProperty AnchorSideTemplateProperty =
            DependencyProperty.Register("AnchorSideTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        public static readonly DependencyProperty AnchorTemplateProperty =
            DependencyProperty.Register("AnchorTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((ControlTemplate)null));

        public static readonly DependencyProperty DocumentPaneControlStyleProperty =
            DependencyProperty.Register("DocumentPaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneControlStyleChanged));

        public static readonly DependencyProperty AnchorablePaneControlStyleProperty =
            DependencyProperty.Register("AnchorablePaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorablePaneControlStyleChanged));

        public static readonly DependencyProperty DocumentHeaderTemplateProperty =
            DependencyProperty.Register("DocumentHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateChanged,
                    CoerceDocumentHeaderTemplateValue));

        public static readonly DependencyProperty DocumentHeaderTemplateSelectorProperty =
            DependencyProperty.Register("DocumentHeaderTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateSelectorChanged,
                    CoerceDocumentHeaderTemplateSelectorValue));

        public static readonly DependencyProperty DocumentTitleTemplateProperty =
            DependencyProperty.Register("DocumentTitleTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentTitleTemplateChanged, CoerceDocumentTitleTemplateValue));

        public static readonly DependencyProperty DocumentTitleTemplateSelectorProperty =
            DependencyProperty.Register("DocumentTitleTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null,
                    OnDocumentTitleTemplateSelectorChanged,
                    CoerceDocumentTitleTemplateSelectorValue));

        public static readonly DependencyProperty AnchorableTitleTemplateProperty =
            DependencyProperty.Register("AnchorableTitleTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null,
                    OnAnchorableTitleTemplateChanged,
                    CoerceAnchorableTitleTemplateValue));

        public static readonly DependencyProperty AnchorableTitleTemplateSelectorProperty =
            DependencyProperty.Register("AnchorableTitleTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableTitleTemplateSelectorChanged));

        public static readonly DependencyProperty AnchorableHeaderTemplateProperty =
            DependencyProperty.Register("AnchorableHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateChanged,
                    CoerceAnchorableHeaderTemplateValue));

        public static readonly DependencyProperty AnchorableHeaderTemplateSelectorProperty =
            DependencyProperty.Register("AnchorableHeaderTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager), new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateSelectorChanged));

        public static readonly DependencyProperty LayoutRootPanelProperty =
            DependencyProperty.Register("LayoutRootPanel", typeof(LayoutPanelControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutRootPanelChanged));

        public static readonly DependencyProperty RightSidePanelProperty =
            DependencyProperty.Register("RightSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnRightSidePanelChanged));

        public static readonly DependencyProperty LeftSidePanelProperty =
            DependencyProperty.Register("LeftSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLeftSidePanelChanged));

        public static readonly DependencyProperty TopSidePanelProperty =
            DependencyProperty.Register("TopSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnTopSidePanelChanged));

        public static readonly DependencyProperty BottomSidePanelProperty =
            DependencyProperty.Register("BottomSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnBottomSidePanelChanged));

        private static readonly DependencyPropertyKey AutoHideWindowPropertyKey
            = DependencyProperty.RegisterReadOnly("AutoHideWindow", typeof(LayoutAutoHideWindowControl),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAutoHideWindowChanged));

        public static readonly DependencyProperty AutoHideWindowProperty
            = AutoHideWindowPropertyKey.DependencyProperty;

        public static readonly DependencyProperty LayoutItemTemplateProperty =
            DependencyProperty.Register("LayoutItemTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemTemplateChanged));

        public static readonly DependencyProperty LayoutItemTemplateSelectorProperty =
            DependencyProperty.Register("LayoutItemTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemTemplateSelectorChanged));

        public static readonly DependencyProperty DocumentsSourceProperty =
            DependencyProperty.Register("DocumentsSource", typeof(IEnumerable), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentsSourceChanged));

        public static readonly DependencyProperty AnchorablesSourceProperty =
            DependencyProperty.Register("AnchorablesSource", typeof(IEnumerable), typeof(DockingManager),
                new FrameworkPropertyMetadata(null,
                    OnAnchorablesSourceChanged));

        public static readonly DependencyProperty ActiveContentProperty =
            DependencyProperty.Register("ActiveContent", typeof(object), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnActiveContentChanged));

        public static readonly DependencyProperty GridSplitterHeightProperty =
            DependencyProperty.Register("GridSplitterHeight", typeof(double), typeof(DockingManager),
                new FrameworkPropertyMetadata(6.0));

        public static readonly DependencyProperty GridSplitterWidthProperty =
            DependencyProperty.Register("GridSplitterWidth", typeof(double), typeof(DockingManager),
                new FrameworkPropertyMetadata(6.0));

        public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateProperty =
            DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplate", typeof(DataTemplate),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateChanged,
                    CoerceDocumentPaneMenuItemHeaderTemplateValue));

        public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateSelectorProperty =
            DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateSelectorChanged,
                    CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue));

        public static readonly DependencyProperty IconContentTemplateProperty =
            DependencyProperty.Register("IconContentTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplate)null));

        public static readonly DependencyProperty IconContentTemplateSelectorProperty =
            DependencyProperty.Register("IconContentTemplateSelector", typeof(DataTemplateSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));

        public static readonly DependencyProperty LayoutItemContainerStyleProperty =
            DependencyProperty.Register("LayoutItemContainerStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleChanged));

        public static readonly DependencyProperty LayoutItemContainerStyleSelectorProperty =
            DependencyProperty.Register("LayoutItemContainerStyleSelector", typeof(StyleSelector),
                typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleSelectorChanged));

        public static readonly DependencyProperty AllowMixedOrientationProperty =
            DependencyProperty.Register("AllowMixedOrientation", typeof(bool), typeof(DockingManager),
                new FrameworkPropertyMetadata(false));


        public static readonly DependencyProperty CanAddProperty = DependencyProperty.Register("CanAdd", typeof(bool),
            typeof(DockingManager), new UIPropertyMetadata(true));

        public static readonly DependencyProperty CanCloseAllButThisProperty =
            DependencyProperty.Register("CanCloseAllButThis", typeof(bool), typeof(DockingManager),
                new UIPropertyMetadata(true));

        public static readonly DependencyProperty CanCloseAllProperty = DependencyProperty.Register("CanCloseAll",
            typeof(bool), typeof(DockingManager), new UIPropertyMetadata(true));


        public static readonly DependencyProperty ShowSystemMenuProperty =
            DependencyProperty.Register("ShowSystemMenu", typeof(bool), typeof(DockingManager),
                new FrameworkPropertyMetadata(true));


        internal bool SuspendAnchorablesSourceBinding = false;

        internal bool SuspendDocumentsSourceBinding = false;

        private readonly IContextMenuHost _contextMenuHost;


        private readonly List<LayoutFloatingWindowControl> _fwList = new List<LayoutFloatingWindowControl>();

        private readonly LinkedList<LayoutContent> _lastLayoutContentElements = new LinkedList<LayoutContent>();

        private readonly List<LayoutItem> _layoutItems = new List<LayoutItem>();

        private readonly List<WeakReference> _logicalChildren = new List<WeakReference>();

        private List<IDropArea> _areas;

        private FrameworkElement _autohideArea;

        private AutoHideWindowManager _autoHideWindowManager;


        private DispatcherOperation _collectLayoutItemsOperations;


        private bool _insideInternalSetActiveContent;

        private NavigatorWindow _navigatorWindow;

        private OverlayWindow _overlayWindow;

        //private DispatcherOperation _setFocusAsyncOperation;

        private bool _suspendLayoutItemCreation;
        private DragUndockHeader currentDragUndockHeader;

        public event EventHandler ActiveContentChanged;

        public event EventHandler<DocumentsClosedEventArgs> DocumentsClosed;

        public event EventHandler<DocumentsClosingEventArgs> DocumentsClosing;

        public event EventHandler<AnchorablesClosingEventArgs> AnchorablesClosing;

        public event EventHandler<AnchorablesClosedEventArgs> AnchorablesClosed;

        public event EventHandler LayoutChanged;

        public event EventHandler LayoutChanging;

        public static event EventHandler<FloatingWindowEventArgs> FloatingWindowShown;


        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        public static DockingManager Instance { get; private set; }

        public object ActiveContent
        {
            get => GetValue(ActiveContentProperty);
            set => SetValue(ActiveContentProperty, value);
        }

        public bool AllowMixedOrientation
        {
            get => (bool)GetValue(AllowMixedOrientationProperty);
            set => SetValue(AllowMixedOrientationProperty, value);
        }


        public ContextMenu AnchorableAsDocumentContextMenu =>
            _contextMenuHost.GetContextMenu(AnchorableAsDocumentContextMenuDefinition.AnchorableAsDocumentContextMenu);

        public ContextMenu AnchorableContextMenu =>
            _contextMenuHost.GetContextMenu(AnchorableContextMenuDefinition.AnchorableContextMenu);

        public DataTemplate AnchorableHeaderTemplate
        {
            get => (DataTemplate)GetValue(AnchorableHeaderTemplateProperty);
            set => SetValue(AnchorableHeaderTemplateProperty, value);
        }

        public DataTemplateSelector AnchorableHeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(AnchorableHeaderTemplateSelectorProperty);
            set => SetValue(AnchorableHeaderTemplateSelectorProperty, value);
        }

        public Style AnchorablePaneControlStyle
        {
            get => (Style)GetValue(AnchorablePaneControlStyleProperty);
            set => SetValue(AnchorablePaneControlStyleProperty, value);
        }

        public ControlTemplate AnchorablePaneTemplate
        {
            get => (ControlTemplate)GetValue(AnchorablePaneTemplateProperty);
            set => SetValue(AnchorablePaneTemplateProperty, value);
        }

        public IEnumerable AnchorablesSource
        {
            get => (IEnumerable)GetValue(AnchorablesSourceProperty);
            set => SetValue(AnchorablesSourceProperty, value);
        }

        public DataTemplate AnchorableTitleTemplate
        {
            get => (DataTemplate)GetValue(AnchorableTitleTemplateProperty);
            set => SetValue(AnchorableTitleTemplateProperty, value);
        }

        public DataTemplateSelector AnchorableTitleTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(AnchorableTitleTemplateSelectorProperty);
            set => SetValue(AnchorableTitleTemplateSelectorProperty, value);
        }

        public ControlTemplate AnchorGroupTemplate
        {
            get => (ControlTemplate)GetValue(AnchorGroupTemplateProperty);
            set => SetValue(AnchorGroupTemplateProperty, value);
        }

        public ControlTemplate AnchorSideTemplate
        {
            get => (ControlTemplate)GetValue(AnchorSideTemplateProperty);
            set => SetValue(AnchorSideTemplateProperty, value);
        }

        public ControlTemplate AnchorTemplate
        {
            get => (ControlTemplate)GetValue(AnchorTemplateProperty);
            set => SetValue(AnchorTemplateProperty, value);
        }

        public LayoutAutoHideWindowControl AutoHideWindow
            => (LayoutAutoHideWindowControl)GetValue(AutoHideWindowProperty);

        public LayoutAnchorSideControl BottomSidePanel
        {
            get => (LayoutAnchorSideControl)GetValue(BottomSidePanelProperty);
            set => SetValue(BottomSidePanelProperty, value);
        }

        public bool CanAdd
        {
            [ExcludeFromCodeCoverage]
            get { return (bool)GetValue(CanAddProperty); }

            [ExcludeFromCodeCoverage]
            set { SetValue(CanAddProperty, value); }
        }

        public bool CanCloseAll
        {
            [ExcludeFromCodeCoverage]
            get { return (bool)GetValue(CanCloseAllProperty); }

            [ExcludeFromCodeCoverage]
            set { SetValue(CanCloseAllProperty, value); }
        }

        public bool CanCloseAllButThis
        {
            [ExcludeFromCodeCoverage]
            get { return (bool)GetValue(CanCloseAllButThisProperty); }

            [ExcludeFromCodeCoverage]
            set { SetValue(CanCloseAllButThisProperty, value); }
        }

        public ContextMenu DocumentContextMenu =>
            _contextMenuHost.GetContextMenu(DocumentContextMenuDefinition.DocumentContextMenu);

        public DataTemplate DocumentHeaderTemplate
        {
            get => (DataTemplate)GetValue(DocumentHeaderTemplateProperty);
            set => SetValue(DocumentHeaderTemplateProperty, value);
        }

        public DataTemplateSelector DocumentHeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DocumentHeaderTemplateSelectorProperty);
            set => SetValue(DocumentHeaderTemplateSelectorProperty, value);
        }

        public Style DocumentPaneControlStyle
        {
            get => (Style)GetValue(DocumentPaneControlStyleProperty);
            set => SetValue(DocumentPaneControlStyleProperty, value);
        }

        public DataTemplate DocumentPaneMenuItemHeaderTemplate
        {
            get => (DataTemplate)GetValue(DocumentPaneMenuItemHeaderTemplateProperty);
            set => SetValue(DocumentPaneMenuItemHeaderTemplateProperty, value);
        }

        public DataTemplateSelector DocumentPaneMenuItemHeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty);
            set => SetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty, value);
        }

        public ControlTemplate DocumentPaneTemplate
        {
            get => (ControlTemplate)GetValue(DocumentPaneTemplateProperty);
            set => SetValue(DocumentPaneTemplateProperty, value);
        }

        public IEnumerable DocumentsSource
        {
            get => (IEnumerable)GetValue(DocumentsSourceProperty);
            set => SetValue(DocumentsSourceProperty, value);
        }

        public DataTemplate DocumentTitleTemplate
        {
            get => (DataTemplate)GetValue(DocumentTitleTemplateProperty);
            set => SetValue(DocumentTitleTemplateProperty, value);
        }

        public DataTemplateSelector DocumentTitleTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(DocumentTitleTemplateSelectorProperty);
            set => SetValue(DocumentTitleTemplateSelectorProperty, value);
        }

        public IEnumerable<LayoutFloatingWindowControl> FloatingWindows => _fwList;

        public double GridSplitterHeight
        {
            get => (double)GetValue(GridSplitterHeightProperty);
            set => SetValue(GridSplitterHeightProperty, value);
        }

        public double GridSplitterWidth
        {
            get => (double)GetValue(GridSplitterWidthProperty);
            set => SetValue(GridSplitterWidthProperty, value);
        }

        public DataTemplate IconContentTemplate
        {
            get => (DataTemplate)GetValue(IconContentTemplateProperty);
            set => SetValue(IconContentTemplateProperty, value);
        }

        public DataTemplateSelector IconContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(IconContentTemplateSelectorProperty);
            set => SetValue(IconContentTemplateSelectorProperty, value);
        }

        public LayoutRoot Layout
        {
            get => (LayoutRoot)GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

        public Style LayoutItemContainerStyle
        {
            get => (Style)GetValue(LayoutItemContainerStyleProperty);
            set => SetValue(LayoutItemContainerStyleProperty, value);
        }

        public StyleSelector LayoutItemContainerStyleSelector
        {
            get => (StyleSelector)GetValue(LayoutItemContainerStyleSelectorProperty);
            set => SetValue(LayoutItemContainerStyleSelectorProperty, value);
        }

        public DataTemplate LayoutItemTemplate
        {
            get => (DataTemplate)GetValue(LayoutItemTemplateProperty);
            set => SetValue(LayoutItemTemplateProperty, value);
        }

        public DataTemplateSelector LayoutItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(LayoutItemTemplateSelectorProperty);
            set => SetValue(LayoutItemTemplateSelectorProperty, value);
        }

        public LayoutPanelControl LayoutRootPanel
        {
            get => (LayoutPanelControl)GetValue(LayoutRootPanelProperty);
            set => SetValue(LayoutRootPanelProperty, value);
        }

        public ILayoutUpdateStrategy LayoutUpdateStrategy
        {
            get => (ILayoutUpdateStrategy)GetValue(LayoutUpdateStrategyProperty);
            set => SetValue(LayoutUpdateStrategyProperty, value);
        }

        public LayoutAnchorSideControl LeftSidePanel
        {
            get => (LayoutAnchorSideControl)GetValue(LeftSidePanelProperty);
            set => SetValue(LeftSidePanelProperty, value);
        }

        public LayoutAnchorSideControl RightSidePanel
        {
            get => (LayoutAnchorSideControl)GetValue(RightSidePanelProperty);
            set => SetValue(RightSidePanelProperty, value);
        }

        public bool ShowSystemMenu
        {
            get => (bool)GetValue(ShowSystemMenuProperty);
            set => SetValue(ShowSystemMenuProperty, value);
        }

        public LayoutAnchorSideControl TopSidePanel
        {
            get => (LayoutAnchorSideControl)GetValue(TopSidePanelProperty);
            set => SetValue(TopSidePanelProperty, value);
        }

        internal DragUndockHeader CurrentDragUndockHeader
        {
            get => currentDragUndockHeader;
            set
            {
                if (currentDragUndockHeader != null)
                    PresentationSource.RemoveSourceChangedHandler(currentDragUndockHeader, OnViewHeaderPresentationSourceChanged);
                currentDragUndockHeader = value;
                if (currentDragUndockHeader != null)
                    PresentationSource.AddSourceChangedHandler(currentDragUndockHeader, OnViewHeaderPresentationSourceChanged);
                KeyboardStateManager.CurrentDragUndockHeader = currentDragUndockHeader;
            }
        }

        private void OnViewHeaderPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (e.NewSource != null || CurrentDragUndockHeader == null)
                return;
            ClearAdorners();
             IsDragging = false;
            CurrentDragUndockHeader = null;
        }

        DockingManager IOverlayWindowHost.Manager => this;

        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).GetEnumerator(); }
        }

        private bool IsNavigatorWindowActive => _navigatorWindow != null;

        static DockingManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager),
                new FrameworkPropertyMetadata(typeof(DockingManager)));
            FocusableProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(false));
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            DocumentPaneTabPanel.SelectedItemHidden += DocumentPaneTabPanel_SelectedItemHidden;

            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragHeaderClickedEvent,
                new RoutedEventHandler((sender, args) => Instance.OnViewHeaderClicked(sender, args)));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragStartedEvent,
                new RoutedEventHandler((sender, args) => Instance.OnViewHeaderDragStarted(sender, args)));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragAbsoluteEvent,
                new RoutedEventHandler((sender, args) => Instance.OnViewHeaderDragAbsolute(sender, args)));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragCompletedAbsoluteEvent,
                new RoutedEventHandler((sender, args) => Instance.OnViewHeaderDragCompleted(sender, args)));

            EventManager.RegisterClassHandler(typeof(ViewPresenter), PreviewMouseDownEvent,
                new RoutedEventHandler((sender, args) => Instance.OnViewMouseDown(sender, args)));
            EventManager.RegisterClassHandler(typeof(LayoutDocumentPaneControl), PreviewMouseDownEvent,
                new RoutedEventHandler((sender, args) => Instance.OnTabControlMouseDown(sender, args)));
            EventManager.RegisterClassHandler(typeof(LayoutDocumentPaneControl), Selector.SelectionChangedEvent,
                new RoutedEventHandler((sender, args) => Instance.OnTabControlSelectionChanged(sender, args)));
            EventManager.RegisterClassHandler(typeof(TabItem), PreviewMouseDownEvent,
                new RoutedEventHandler((sender, args) => Instance.OnTabItemMouseDown(sender, args)));

            EventManager.RegisterClassHandler(typeof(LayoutDocumentPaneControl), DragUndockHeader.DragHeaderContextMenuEvent,
                new EventHandler<DragUndockHeaderContextMenuEventArgs>(OnDocumentTabContextMenu));
            EventManager.RegisterClassHandler(typeof(TabGroupControl), DragUndockHeader.DragHeaderContextMenuEvent,
                new EventHandler<DragUndockHeaderContextMenuEventArgs>(OnTabGroupTabContextMenu));
            EventManager.RegisterClassHandler(typeof(AnchorablePaneTitle), DragUndockHeader.DragHeaderContextMenuEvent,
                new EventHandler<DragUndockHeaderContextMenuEventArgs>(OnViewHeaderContextMenu));

        }

        private static void OnViewHeaderContextMenu(object sender, DragUndockHeaderContextMenuEventArgs args)
        {
            var originalSource = args.OriginalSource as DragUndockHeader;
            var screen = originalSource.PointToScreen(args.HeaderPoint);
            var model = originalSource.Model;

            ShowShellContextMenu(model, screen, AnchorableContextMenuProvider.Instance);
        }

        private static void OnTabGroupTabContextMenu(object sender, DragUndockHeaderContextMenuEventArgs e)
        {
            var originalSource = e.OriginalSource as DragUndockHeader;
            var screen = originalSource.PointToScreen(e.HeaderPoint);

            foreach (var layoutItem in Instance._layoutItems)
            {
                if (layoutItem == originalSource.LayoutItem)
                {
                    layoutItem.EnsureFocused();
                    layoutItem.ActivateCommand.Execute(null);
                    ShowShellContextMenu(originalSource.Model, screen, AnchorableContextMenuProvider.Instance);
                    e.Handled = true;
                }
            }
        }

        private static void OnDocumentTabContextMenu(object sender, DragUndockHeaderContextMenuEventArgs e)
        {
            var originalSource = e.OriginalSource as DragUndockHeader;
            var screen = originalSource.PointToScreen(e.HeaderPoint);

            foreach (var layoutItem in Instance._layoutItems)
            {
                if (layoutItem == originalSource.LayoutItem)
                {
                    layoutItem.EnsureFocused();
                    layoutItem.ActivateCommand.Execute(null);
                    ShowShellContextMenu(originalSource.Model, screen, DocumentTabModelContextMenuProvider.Instance);
                    e.Handled = true;
                }
            }
        }

        private static void ShowShellContextMenu(LayoutContent layout, Point screenPoint, IContextMenuProvider provider)
        {
            var shell = IoC.Get<IMafUIShell>();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input,
                (Action) (() => { shell.ShowContextMenu(screenPoint, null, provider, layout); }));

        }

        internal bool IsDragging { get; set; }

        private void OnViewHeaderDragCompleted(object sender, RoutedEventArgs args)
        {
            if (!(args is DragAbsoluteCompletedEventArgs e))
                return;
            ClearAdorners();
            if (!DraggedTabScope.IsDraggingTab)
            {
                DraggedTabInfo = null;
                IsDragging = false;
            }

            CurrentDragUndockHeader = null;
        }

        private void OnViewHeaderDragAbsolute(object sender, RoutedEventArgs args)
        {
            var originalSource = (DragUndockHeader)args.OriginalSource;
            if (originalSource.IsWindowTitleBar)
            {
                if (NativeMethods.NativeMethods.IsKeyPressed(17))
                    ClearAdorners();
                else
                    _overlayWindow.EnableDropTargets();
            }
            else
            {
                if (DraggedTabInfo == null)
                    return;
                HandleDragAbsoluteMoveTabInPlace(originalSource, args as DragAbsoluteEventArgs);
            }
        }


        private void OnViewHeaderDragStarted(object sender, RoutedEventArgs args)
        {
            var originalSource = (DragUndockHeader)args.OriginalSource;
            CurrentDragUndockHeader = originalSource;
            if (originalSource.Model != null)
            {
                StartDraggingFloatingWindowForContent(originalSource.Model);
            }

            if (!originalSource.IsWindowTitleBar && originalSource.Model != null)
            {
                originalSource.CancelDrag();
                DraggedTabInfo = null;
                var undockedPosition = Rect.Empty;
                var frameworkElement = originalSource.LayoutItem;
                if (frameworkElement != null && frameworkElement.IsConnectedToPresentationSource())
                    undockedPosition = new Rect(frameworkElement.PointToScreen(new Point(0, 0)), frameworkElement.RenderSize.LogicalToDeviceUnits());

                void EventHandler(object localSender, FloatingWindowEventArgs localArgs)
                {
                    var logicalUnits = undockedPosition.Size.DeviceToLogicalUnits();
                    var val11 = localArgs.Window.Width + (logicalUnits.Width - frameworkElement.RenderSize.Width);
                    var val12 = localArgs.Window.Height + (logicalUnits.Height - frameworkElement.RenderSize.Height);
                    localArgs.Window.Width = Math.Max(val11, 0.0);
                    localArgs.Window.Height = Math.Max(val12, 0.0);
                }
                try
                {
                    if (undockedPosition != Rect.Empty)
                        FloatingWindowShown += EventHandler;
                }
                finally
                {
                    FloatingWindowShown -= EventHandler;
                }
            }


            IsDragging = true;
        }

        private void HandleDragAbsoluteMoveTabInPlace(DragUndockHeader header, DragAbsoluteEventArgs args)
        {
            using (new DraggedTabScope())
            {
                var draggedTabInfo = DraggedTabInfo;
                var index = draggedTabInfo.GetTabIndexAt(args.ScreenPoint);
                var flag = index == draggedTabInfo.DraggedTabPosition;
                if (flag)
                    draggedTabInfo.ClearVirtualTabRect();
                if (!draggedTabInfo.TabStripRect.Contains(args.ScreenPoint) || draggedTabInfo.VirtualTabRect.Contains(args.ScreenPoint) || flag)
                    return;
                if (index != -1)
                {
                    draggedTabInfo.SetVirtualTabRect(index);
                    var element = header.Model;
                    if (element.IsPinned)
                    {
                        if (index < element.Parent.Children.OfType<LayoutContent>().Count(x => x.IsPinned))
                            MovePinnedTab(element, index);
                    }
                    else
                        MoveTab(element, index);

                    draggedTabInfo.TabStrip.IsNotificationNeeded = true;
                    draggedTabInfo.DraggedTabPosition = index;
                }
                if (draggedTabInfo.HasBeenReordered)
                    return;
                draggedTabInfo.HasBeenReordered = true;
                draggedTabInfo.ExpandTabStrip();
            }
        }

        public static void MovePinnedTab(LayoutElement tab, int position)
        {
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            if (!(tab is LayoutContent view))
                throw new ArgumentException("Tab must be a LayoutContent.");
            if (!view.IsPinned)
                throw new ArgumentException("Tab must be pinned.");
            var parent = tab.Parent;
            if (parent == null)
                throw new ArgumentException();
            if (!(parent is LayoutGroupBase))
                throw new ArgumentException();
            if (!(parent is ILayoutPane layoutPane))
                return;
            var pinndedViews = view.Parent.Children.OfType<LayoutContent>().Where(x => x.IsPinned).ToList();
            if (position == pinndedViews.IndexOf(view))
                return;
            if (position < 0 || position > pinndedViews.Count - 1)
                throw new ArgumentOutOfRangeException("position: " + position + " group.PinnedViews: " + pinndedViews.Count);

            using (LayoutSynchronizer.BeginLayoutSynchronization())
            {
                var index = pinndedViews.IndexOf(view);
                layoutPane.MoveChild(index, position);
            }
        }

        public static void MoveTab(LayoutElement tab, int newIndex, bool selectAfterMoving = true)
        {
            Validate.IsNotNull(tab, nameof(tab));
            if (!(tab is LayoutContent layoutContent))
                throw new ArgumentException("Tab must be a LayoutContent");
            var parent = tab.Parent;
            if (parent == null)
                throw new ArgumentException();
            if (!(parent is LayoutGroupBase))
                throw new ArgumentException();
            if (!(parent is ILayoutPane layoutPane))
                return;
            if (newIndex < 0 || newIndex > parent.ChildrenCount - 1)
                throw new ArgumentOutOfRangeException();
            var position = parent.Children.ToList().IndexOf(tab);
            if (newIndex == position)
                return;

            using (LayoutSynchronizer.BeginLayoutSynchronization())
            {
                layoutPane.MoveChild(position, newIndex);
                if (!selectAfterMoving)
                    return;
                layoutContent.IsActive = true;
            }
        }

        private void OnViewMouseDown(object sender, RoutedEventArgs args)
        {
            var presenter = sender as ViewPresenter;
            if (presenter == null || !ShouldActivateFromClick(presenter, args as MouseButtonEventArgs))
                return;
            ActivateViewFromPresenter(presenter);
        }

        protected virtual void ActivateViewFromPresenter(ViewPresenter presenter)
        {
            var dataContext = presenter?.DataContext as LayoutContent;
            if (dataContext == null || dataContext == Layout.ActiveContent)
                return;
            dataContext.IsActive = true;
        }

        private void OnViewHeaderClicked(object sender, RoutedEventArgs args)
        {
            var originalSource = args.OriginalSource as DragUndockHeader;
            var ancestor = originalSource.FindAncestor<ReorderTabPanel>();
            if (ancestor != null)
            {
                DraggedTabInfo = new DraggedTabInfo
                {
                    TabStrip = ancestor,
                    DraggedViewElement = originalSource.Model
                };
                DraggedTabInfo.MeasureTabStrip();
            }
            else
                DraggedTabInfo = null;
        }

        internal DraggedTabInfo DraggedTabInfo { get; set; }

        private void OnTabItemMouseDown(object sender, RoutedEventArgs args)
        {
            if (!(sender is TabItem tabItem))
                return;
            if (!ShouldActivateFromClick(tabItem, args as MouseButtonEventArgs))
                return;
            if (!(tabItem.DataContext is LayoutContent layoutContent))
                return;
            tabItem.IsSelected = true;
            if (!tabItem.IsSelected)
                return;
            layoutContent.IsActive = true;
        }

        private void OnTabControlSelectionChanged(object sender, RoutedEventArgs args)
        {
            if (!(sender is TabControl) || !(sender is ILayoutControl layoutControl))
                return;
            var t = layoutControl.Model.Descendents().OfType<LayoutContent>();
            if (!t.Any(x => x == Layout.ActiveContent))
                return;
            if (!(layoutControl.Model is ILayoutContentSelector selector))
                return;
            var selectedElement = selector.SelectedContent;
            if (selectedElement == null || selectedElement == Layout.ActiveContent)
                return;
            selectedElement.IsActive = true;
        }

        private void OnTabControlMouseDown(object sender, RoutedEventArgs args)
        {
            if (!(sender is TabControl tabControl) || !(sender is ILayoutControl layoutControl))
                return;
            if (!ShouldActivateFromClick(tabControl, args as MouseButtonEventArgs) || IsClickWithinTabItem(args as MouseButtonEventArgs))
                return;
            if (!(layoutControl.Model is ILayoutContentSelector selector))
                return;
            selector.SelectedContent.IsActive = true;
        }

        private static bool IsClickWithinTabItem(MouseButtonEventArgs args)
        {
            var originalSource = args.OriginalSource as Visual;
            return originalSource?.FindAncestorOrSelf<TabItem>() != null;
        }

        private static bool ShouldActivateFromClick(DependencyObject activationElement, MouseButtonEventArgs args)
        {
            switch (args.ChangedButton)
            {
                case MouseButton.Left:
                    return ShouldActivateFromClick(activationElement, args.OriginalSource, ViewPresenter.CanActivateFromLeftClickProperty);
                case MouseButton.Middle:
                    return ShouldActivateFromClick(activationElement, args.OriginalSource, ViewPresenter.CanActivateFromMiddleClickProperty);
                default:
                    return true;
            }
        }

        private static bool ShouldActivateFromClick(DependencyObject activationElement, object originalSource, DependencyProperty canActivateFromClickProperty)
        {
            for (var sourceElement = originalSource as DependencyObject; sourceElement != null && sourceElement != activationElement; sourceElement = sourceElement.GetVisualOrLogicalParent())
            {
                if (!(bool)sourceElement.GetValue(canActivateFromClickProperty))
                    return false;
            }
            return true;
        }


        public DockingManager()
        {
            Layout = new LayoutRoot
            {
                RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
            };
            Loaded += DockingManager_Loaded;
            Unloaded += DockingManager_Unloaded;
            _contextMenuHost = IoC.Get<IContextMenuHost>();
            Instance = this;
            MergeResources();
        }

        private static void MergeResources()
        {
            Application.Current.Resources.MergedDictionaries.Add(LoadResourceValue<ResourceDictionary>("Themes/DataTemplates.xaml"));
        }

        internal static T LoadResourceValue<T>(string xamlName)
        {
            return (T)Application.LoadComponent(new Uri(Assembly.GetExecutingAssembly().GetName().Name + ";component/" + xamlName, UriKind.Relative));
        }


        public LayoutItem GetLayoutItemFromModel(LayoutContent content)
        {
            return _layoutItems.FirstOrDefault(item => Equals(item.LayoutElement, content));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetupAutoHideWindow();
        }

        public void RemoveChild(object child)
        {
            RemoveLogicalChild(child);
        }


        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_areas != null)
                return _areas;

            var isDraggingDocuments = draggingWindow.Model is LayoutDocumentFloatingWindow;

            _areas = new List<IDropArea>();

            if (!isDraggingDocuments)
            {
                _areas.Add(new DropArea<DockingManager>(
                    this,
                    DropAreaType.DockingManager));

                foreach (
                    var areaHost in
                    this.FindVisualChildren<LayoutAnchorablePaneControl>()
                        .Where(areaHost => areaHost.Model.Descendents().Any()))
                    _areas.Add(new DropArea<LayoutAnchorablePaneControl>(
                        areaHost,
                        DropAreaType.AnchorablePane));
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneControl>())
                _areas.Add(new DropArea<LayoutDocumentPaneControl>(
                    areaHost,
                    DropAreaType.DocumentPane));

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
                if (areaHost.Model is LayoutDocumentPaneGroup documentGroupModel &&
                    !documentGroupModel.Children.Any(c => c.IsVisible))
                    _areas.Add(new DropArea<LayoutDocumentPaneGroupControl>(
                        areaHost,
                        DropAreaType.DocumentPaneGroup));

            return _areas;
        }

        void IOverlayWindowHost.HideOverlayWindow()
        {
            _areas = null;
            _overlayWindow.Owner = null;
            _overlayWindow.HideDropTargets();
        }

        bool IOverlayWindowHost.HitTest(Point dragPoint)
        {
            var detectionRect = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
                this.TransformActualSizeToAncestor());
            return detectionRect.Contains(dragPoint);
        }

        IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
        {
            CreateOverlayWindow();
            _overlayWindow.Owner = draggingWindow;
            _overlayWindow.EnableDropTargets();
            _overlayWindow.Show();
            return _overlayWindow;
        }

        internal void _ExecuteAddCommand(LayoutContent contentSelected)
        {
        }

        internal void _ExecuteAutoHideCommand(LayoutAnchorable anchorable)
        {
            anchorable.ToggleAutoHide();
        }

        internal void _ExecuteCloseAllButThisCommand(LayoutContent contentSelected)
        {

            var layoutContents = Layout.Descendents().OfType<LayoutContent>()
                .Where(d => !Equals(d, contentSelected) &&
                            (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToList();

            if (layoutContents.Any(x => !x.TestCanClose()))
                return;

            if (DocumentsClosing != null)
            {
                var documents = layoutContents.OfType<LayoutDocument>();
                var evargs = new DocumentsClosingEventArgs(documents);
                DocumentsClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }
            if (AnchorablesClosing != null)
            {
                var anchors = layoutContents.OfType<LayoutAnchorable>();
                var evargs = new AnchorablesClosingEventArgs(anchors, AnchorableCloseMode.Close);
                AnchorablesClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }

            foreach (var contentToClose in layoutContents)
            {
                if (!contentToClose.CanClose)
                    continue;
                if (contentToClose is LayoutDocument document)
                    _ExecuteCloseCommand(document, false);
                else if (contentToClose is LayoutAnchorable anchorable)
                    _ExecuteCloseCommand(anchorable);
            }

            if (DocumentsClosed != null)
            {
                var documents = layoutContents.OfType<LayoutDocument>();
                var evargs = new DocumentsClosedEventArgs(documents);
                DocumentsClosed(this, evargs);
            }

            if (AnchorablesClosed != null)
            {
                var anchors = layoutContents.OfType<LayoutAnchorable>();
                var evargs = new AnchorablesClosedEventArgs(anchors, AnchorableCloseMode.Close);
                AnchorablesClosed(this, evargs);
            }
        }


        public IReadOnlyCollection<LayoutContent> AllOpenDocuments => Layout.Descendents()
            .OfType<LayoutContent>()
            .Where(d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow).ToList();


        internal void _ExecuteCloseAllCommand()
        {
            var layoutContents = AllOpenDocuments;


            if (layoutContents.Any(x => !x.TestCanClose()))
                return;


            if (DocumentsClosing != null)
            {
                var documents = layoutContents.OfType<LayoutDocument>();
                var evargs = new DocumentsClosingEventArgs(documents);
                DocumentsClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }

            if (AnchorablesClosing != null)
            {
                var anchors = layoutContents.OfType<LayoutAnchorable>();
                var evargs = new AnchorablesClosingEventArgs(anchors, AnchorableCloseMode.Close);
                AnchorablesClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }

            foreach (var contentToClose in layoutContents)
            {
                if (!contentToClose.CanClose)
                    continue;
                if (contentToClose is LayoutDocument document)
                    _ExecuteCloseCommand(document, false);
                else if (contentToClose is LayoutAnchorable anchorable)
                    _ExecuteCloseCommand(anchorable, false);
            }
            if (DocumentsClosed != null)
            {
                var documents = layoutContents.OfType<LayoutDocument>();
                var evargs = new DocumentsClosedEventArgs(documents);
                DocumentsClosed(this, evargs);
            }

            if (AnchorablesClosed != null)
            {
                var anchors = layoutContents.OfType<LayoutAnchorable>();
                var evargs = new AnchorablesClosedEventArgs(anchors, AnchorableCloseMode.Close);
                AnchorablesClosed(this, evargs);
            }
        }

        internal void _ExecuteCloseCommand(LayoutAnchorable anchorable, bool raiseAnchorEvents = true)
        {
            if (anchorable == null)
                return;

            if (raiseAnchorEvents && !anchorable.TestCanClose())
                return;

            if (raiseAnchorEvents && AnchorablesClosing != null)
            {
                var evargs = new AnchorablesClosingEventArgs(new List<LayoutAnchorable> { anchorable }, AnchorableCloseMode.Close);
                AnchorablesClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }
            if (anchorable.IsAutoHidden)
                anchorable.ToggleAutoHide();
            RemoveViewFromLogicalChild(anchorable);
            anchorable.Close();

            if (raiseAnchorEvents && AnchorablesClosed != null)
            {
                var evargs = new AnchorablesClosedEventArgs(new List<LayoutAnchorable> { anchorable }, AnchorableCloseMode.Close);
                AnchorablesClosed(this, evargs);
            }
        }

        internal void _ExecuteCloseCommand(LayoutDocument document, bool raiseDocumentsEvents = true)
        {

            if (raiseDocumentsEvents && !document.TestCanClose())
                return;

            if (raiseDocumentsEvents && DocumentsClosing != null)
            {
                var evargs = new DocumentsClosingEventArgs(new List<LayoutDocument> { document });
                DocumentsClosing(this, evargs);
                if (evargs.Cancel)
                    return;
            }

            document.Close();
            RemoveViewFromLogicalChild(document);

            if (raiseDocumentsEvents && DocumentsClosed != null)
            {
                var evargs = new DocumentsClosedEventArgs(new List<LayoutDocument> { document });
                DocumentsClosed(this, evargs);
            }
        }

        internal void _ExecuteContentActivateCommand(LayoutContent content)
        {
            content.IsActive = true;
        }

        internal void _ExecuteDockAsDocumentCommand(LayoutContent content)
        {
            content.DockAsDocument();
        }

        internal void _ExecuteDockCommand(LayoutAnchorable anchorable)
        {
            anchorable.Dock();
        }

        internal void _ExecuteFloatCommand(LayoutContent contentToFloat)
        {
            contentToFloat.Float();
        }

        internal void _ExecuteHideCommand(LayoutAnchorable anchorable)
        {
            if (anchorable == null)
                return;

            if (EnvironmentGeneralOptions.Instance.DockedWinClose)
            {
                if (!anchorable.TestCanHide())
                    return;
                if (AnchorablesClosing != null)
                {
                    var evargs = new AnchorablesClosingEventArgs(new List<LayoutAnchorable> { anchorable }, AnchorableCloseMode.Hide);
                    AnchorablesClosing(this, evargs);
                    if (evargs.Cancel)
                        return;
                }
                anchorable.Hide();
                if (AnchorablesClosed != null)
                {
                    var evargs = new AnchorablesClosedEventArgs(new List<LayoutAnchorable> { anchorable }, AnchorableCloseMode.Hide);
                    AnchorablesClosed(this, evargs);
                }
            }
            else
            {
                if (!(anchorable.Parent is LayoutAnchorablePane pane))
                    return;

                var children = pane.Children.Where(x => x.TestCanHide()).ToList();

                if (AnchorablesClosing != null)
                {
                    var evargs = new AnchorablesClosingEventArgs(children, AnchorableCloseMode.Hide);
                    AnchorablesClosing(this, evargs);
                }
                foreach (var child in children)
                    child.Hide();
                if (AnchorablesClosed != null)
                {
                    var evargs = new AnchorablesClosedEventArgs(children, AnchorableCloseMode.Hide);
                    AnchorablesClosed(this, evargs);
                }
            }

            foreach (var layoutContent in _lastLayoutContentElements)
            {
                if (layoutContent == null)
                    continue;
                if (layoutContent is LayoutAnchorable layoutAnchorable &&
                    (layoutAnchorable.IsHidden || !layoutAnchorable.IsVisible))
                    continue;
                if (layoutContent is LayoutDocument layoutDocument && !layoutDocument.IsVisible)
                    continue;
                InternalSetActiveContent(layoutContent);
                break;
            }
        }


        internal void _ExecutePinCommand(LayoutContent layoutContent)
        {
            if (layoutContent == null)
                return;
            layoutContent.IsPinned = !layoutContent.IsPinned;
        }

        // ReSharper disable once InconsistentNaming
        internal UIElement CreateUIElementForModel(ILayoutElement model)
        {
            if (model is LayoutPanel panel)
                return new LayoutPanelControl(panel);
            if (model is LayoutAnchorablePaneGroup group)
                return new LayoutAnchorablePaneGroupControl(group);
            if (model is LayoutDocumentPaneGroup paneGroup)
                return new LayoutDocumentPaneGroupControl(paneGroup);

            if (model is LayoutAnchorSide side)
            {
                var templateModelView = new LayoutAnchorSideControl(side);
                templateModelView.SetBinding(TemplateProperty, new Binding("AnchorSideTemplate") { Source = this });
                return templateModelView;
            }

            if (model is LayoutAnchorGroup anchorGroup)
            {
                var templateModelView = new LayoutAnchorGroupControl(anchorGroup);
                templateModelView.SetBinding(TemplateProperty, new Binding("AnchorGroupTemplate") { Source = this });
                return templateModelView;
            }

            if (model is LayoutDocumentPane documentPane)
            {
                var templateModelView = new LayoutDocumentPaneControl(documentPane);
                templateModelView.SetBinding(StyleProperty, new Binding("DocumentPaneControlStyle") { Source = this });
                return templateModelView;
            }

            if (model is LayoutAnchorablePane pane)
            {
                var templateModelView = new LayoutAnchorablePaneControl(pane);
                templateModelView.SetBinding(StyleProperty, new Binding("AnchorablePaneControlStyle") { Source = this });
                return templateModelView;
            }

            if (model is LayoutAnchorableFloatingWindow fw)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return null;
                var modelFw = fw;
                var newFw = new LayoutAnchorableFloatingWindowControl(modelFw);
                newFw.SetParentToMainWindowOf(this);

                var paneForExtensions = modelFw.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault();
                if (paneForExtensions != null)
                {
                    //ensure that floating window position is inside current (or nearest) monitor
                    paneForExtensions.KeepInsideNearestMonitor();

                    newFw.Left = paneForExtensions.FloatingLeft;
                    newFw.Top = paneForExtensions.FloatingTop;
                    newFw.Width = paneForExtensions.FloatingWidth;
                    newFw.Height = paneForExtensions.FloatingHeight;
                }

                newFw.ShowInTaskbar = false;
                newFw.Show();

                FloatingWindowShown?.RaiseEvent(this, new FloatingWindowEventArgs(newFw));

                // Do not set the WindowState before showing or it will be lost
                if (paneForExtensions != null && paneForExtensions.IsMaximized)
                    newFw.WindowState = WindowState.Maximized;
                return newFw;
            }

            if (model is LayoutDocumentFloatingWindow window)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return null;
                var modelFw = window;
                var newFw = new LayoutDocumentFloatingWindowControl(modelFw);
                newFw.SetParentToMainWindowOf(this);

                var paneForExtensions = modelFw.RootDocument;
                if (paneForExtensions != null)
                {
                    //ensure that floating window position is inside current (or nearest) monitor
                    paneForExtensions.KeepInsideNearestMonitor();

                    newFw.Left = paneForExtensions.FloatingLeft;
                    newFw.Top = paneForExtensions.FloatingTop;
                    newFw.Width = paneForExtensions.FloatingWidth;
                    newFw.Height = paneForExtensions.FloatingHeight;
                }

                newFw.ShowInTaskbar = true;
                newFw.Show();

                FloatingWindowShown?.RaiseEvent(this, new FloatingWindowEventArgs(newFw));
                // Do not set the WindowState before showing or it will be lost
                if (paneForExtensions != null && paneForExtensions.IsMaximized)
                    newFw.WindowState = WindowState.Maximized;
                return newFw;
            }

            if (model is LayoutDocument document)
            {
                var templateModelView = new LayoutDocumentControl { Model = document };
                return templateModelView;
            }

            return null;
        }

        internal FrameworkElement GetAutoHideAreaElement()
        {
            return _autohideArea;
        }

        internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow == null)
                yield break;

            var windowParentHandle = new WindowInteropHelper(parentWindow).EnsureHandle();

            var currentHandle = NativeMethods.User32.GetWindow(windowParentHandle,
                (int)NativeMethods.NativeMethods.GetWindowCmd.GwHwndfirst);
            while (currentHandle != IntPtr.Zero)
            {
                var ctrl =
                    _fwList.FirstOrDefault(fw => new WindowInteropHelper(fw).EnsureHandle() == currentHandle);
                if (ctrl != null && Equals(ctrl.Model.Root.Manager, this))
                    yield return ctrl;

                currentHandle = NativeMethods.User32.GetWindow(currentHandle, (int)NativeMethods.NativeMethods.GetWindowCmd.GwHwndnext);
            }
        }

        internal void HideAutoHideWindow(LayoutAnchorControl anchor)
        {
            _autoHideWindowManager.HideAutoWindow(anchor);
        }


        internal void InternalAddLogicalChild(object element)
        {
            //System.Diagnostics.Trace.WriteLine("[{0}]InternalAddLogicalChild({1})", this, element);
#if DEBUG
            if (_logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).Contains(element))
                // ReSharper disable once ObjectCreationAsStatement
                new InvalidOperationException();
#endif
            if (_logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).Contains(element))
                return;

            _logicalChildren.Add(new WeakReference(element));
            AddLogicalChild(element);
        }

        internal void InternalRemoveLogicalChild(object element)
        {
            //System.Diagnostics.Trace.WriteLine("[{0}]InternalRemoveLogicalChild({1})", this, element);

            var wrToRemove = _logicalChildren.FirstOrDefault(ch => ch.GetValueOrDefault<object>() == element);
            if (wrToRemove != null)
                _logicalChildren.Remove(wrToRemove);
            RemoveLogicalChild(element);
        }

        internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
        {
            _fwList.Remove(floatingWindow);
            if (floatingWindow?.Model?.Root == null)
                return;
            var layoutItem = GetLayoutItemFromModel(floatingWindow.Model.Root.ActiveContent);
            if (layoutItem != null)
                layoutItem.IsFloating = false;
        }

        internal void SetLastActiveContent(object oldValue)
        {
            var lastLayoutContent =
                Layout.Descendents()
                    .OfType<LayoutContent>()
                    .OrderBy(lc => lc.LastActivationTimeStamp)
                    .FirstOrDefault(lc => Equals(lc, oldValue) || lc.Content == oldValue);

            if (_lastLayoutContentElements.Contains(lastLayoutContent))
                _lastLayoutContentElements.Remove(lastLayoutContent);
            _lastLayoutContentElements.AddFirst(lastLayoutContent);
        }

        internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
        {
            _autoHideWindowManager.ShowAutoHideWindow(anchor);
            //if (_autohideArea == null)
            //    return;

            //if (AutoHideWindow != null && AutoHideWindow.Model == anchor.Model)
            //    return;

            //Trace.WriteLine("ShowAutoHideWindow()");

            //_currentAutohiddenAnchor = new WeakReference(anchor);

            //HideAutoHideWindow(anchor);

            //SetAutoHideWindow(new LayoutAutoHideWindowControl(anchor));
            //AutoHideWindow.Show();
        }

        internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel, bool startDrag = true)
        {
            if (!contentModel.CanFloat)
                return;
            if (contentModel is LayoutAnchorable contentModelAsAnchorable &&
                contentModelAsAnchorable.IsAutoHidden)
                contentModelAsAnchorable.ToggleAutoHide();

            var parentPane = contentModel.Parent as ILayoutPane;
            if (parentPane == null)
                return;
            var parentPaneAsPositionableElement = contentModel.Parent as ILayoutPositionableElement;
            var parentPaneAsWithActualSize = contentModel.Parent as ILayoutPositionableElementWithActualSize;
            // ReSharper disable once PossibleNullReferenceException
            var list = parentPane.Children.ToList();
            var contentModelParentChildrenIndex = list.IndexOf(contentModel);

            if (contentModel.FindParent<LayoutFloatingWindow>() == null)
            {
                ((ILayoutPreviousContainer)contentModel).PreviousContainer = parentPane;
                contentModel.PreviousContainerIndex = contentModelParentChildrenIndex;
            }

            parentPane.RemoveChildAt(contentModelParentChildrenIndex);

            var layoutItem = GetLayoutItemFromModel(contentModel);
            if (layoutItem != null)
                layoutItem.IsFloating = true;

            var fwWidth = contentModel.FloatingWidth;
            var fwHeight = contentModel.FloatingHeight;

            if (fwWidth == 0.0)
                if (parentPaneAsPositionableElement != null)
                    fwWidth = parentPaneAsPositionableElement.FloatingWidth;
            if (fwHeight == 0.0)
                if (parentPaneAsPositionableElement != null)
                    fwHeight = parentPaneAsPositionableElement.FloatingHeight;

            if (fwWidth == 0.0)
                if (parentPaneAsWithActualSize != null)
                    fwWidth = parentPaneAsWithActualSize.ActualWidth;
            if (fwHeight == 0.0)
                if (parentPaneAsWithActualSize != null)
                    fwHeight = parentPaneAsWithActualSize.ActualHeight;

            LayoutFloatingWindow fw;
            LayoutFloatingWindowControl fwc;
            if (contentModel is LayoutAnchorable anchorableContent)
            {
                fw = new LayoutAnchorableFloatingWindow
                {
                    RootPanel = new LayoutAnchorablePaneGroup(
                        new LayoutAnchorablePane(anchorableContent)
                        {
                            // ReSharper disable once PossibleNullReferenceException
                            DockWidth = parentPaneAsPositionableElement.DockWidth,
                            DockHeight = parentPaneAsPositionableElement.DockHeight,
                            DockMinHeight = parentPaneAsPositionableElement.DockMinHeight,
                            DockMinWidth = parentPaneAsPositionableElement.DockMinWidth,
                            FloatingLeft = parentPaneAsPositionableElement.FloatingLeft,
                            FloatingTop = parentPaneAsPositionableElement.FloatingTop,
                            FloatingWidth = parentPaneAsPositionableElement.FloatingWidth,
                            FloatingHeight = parentPaneAsPositionableElement.FloatingHeight
                        })
                };

                Layout.FloatingWindows.Add(fw);

                fwc = new LayoutAnchorableFloatingWindowControl((LayoutAnchorableFloatingWindow)fw)
                {
                    Width = fwWidth,
                    Height = fwHeight,
                    Left = anchorableContent.FloatingLeft,
                    Top = anchorableContent.FloatingTop
                };
            }
            else
            {
                var anchorableDocument = contentModel as LayoutDocument;
                fw = new LayoutDocumentFloatingWindow
                {
                    RootDocument = anchorableDocument
                };

                Layout.FloatingWindows.Add(fw);

                fwc = new LayoutDocumentFloatingWindowControl((LayoutDocumentFloatingWindow)fw)
                {
                    Width = fwWidth,
                    Height = fwHeight,
                    Left = contentModel.FloatingLeft,
                    Top = contentModel.FloatingTop
                };
            }

            _fwList.Add(fwc);

            Layout.CollectGarbage();

            UpdateLayout();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (startDrag)
                    fwc.AttachDrag();
                fwc.Show();
                FloatingWindowShown?.RaiseEvent(this, new FloatingWindowEventArgs(fwc));
            }), DispatcherPriority.Send);
        }

        internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
        {
            if (paneModel.Children.Any(c => !c.CanFloat))
                return;
            var paneAsPositionableElement = paneModel as ILayoutPositionableElement;
            var paneAsWithActualSize = paneModel as ILayoutPositionableElementWithActualSize;

            var fwWidth = paneAsPositionableElement.FloatingWidth;
            var fwHeight = paneAsPositionableElement.FloatingHeight;


            if (fwWidth == 0.0)
                fwWidth = paneAsWithActualSize.ActualWidth;
            if (fwHeight == 0.0)
                fwHeight = paneAsWithActualSize.ActualHeight;

            var destPane = new LayoutAnchorablePane
            {
                DockWidth = paneAsPositionableElement.DockWidth,
                DockHeight = paneAsPositionableElement.DockHeight,
                DockMinHeight = paneAsPositionableElement.DockMinHeight,
                DockMinWidth = paneAsPositionableElement.DockMinWidth,
                FloatingLeft = paneAsPositionableElement.FloatingLeft,
                FloatingTop = paneAsPositionableElement.FloatingTop,
                FloatingWidth = paneAsPositionableElement.FloatingWidth,
                FloatingHeight = paneAsPositionableElement.FloatingHeight
            };

            var savePreviousContainer = paneModel.FindParent<LayoutFloatingWindow>() == null;
            var currentSelectedContentIndex = paneModel.SelectedContentIndex;
            while (paneModel.Children.Count > 0)
            {
                var contentModel = paneModel.Children[paneModel.Children.Count - 1];

                if (savePreviousContainer)
                {
                    var contentModelAsPreviousContainer = (ILayoutPreviousContainer)contentModel;
                    contentModelAsPreviousContainer.PreviousContainer = paneModel;
                    contentModel.PreviousContainerIndex = paneModel.Children.Count - 1;
                }

                paneModel.RemoveChildAt(paneModel.Children.Count - 1);
                destPane.Children.Insert(0, contentModel);
            }

            if (destPane.Children.Count > 0)
                destPane.SelectedContentIndex = currentSelectedContentIndex;


            LayoutFloatingWindow fw = new LayoutAnchorableFloatingWindow
            {
                RootPanel = new LayoutAnchorablePaneGroup(
                    destPane)
                {
                    DockHeight = destPane.DockHeight,
                    DockWidth = destPane.DockWidth,
                    DockMinHeight = destPane.DockMinHeight,
                    DockMinWidth = destPane.DockMinWidth
                }
            };

            Layout.FloatingWindows.Add(fw);

            LayoutFloatingWindowControl fwc = new LayoutAnchorableFloatingWindowControl(
                (LayoutAnchorableFloatingWindow)fw)
            {
                Width = fwWidth,
                Height = fwHeight
            };

            _fwList.Add(fwc);

            Layout.CollectGarbage();

            InvalidateArrange();

            fwc.AttachDrag();
            fwc.Show();
            FloatingWindowShown?.RaiseEvent(this, new FloatingWindowEventArgs(fwc));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _areas = null;
            return base.ArrangeOverride(arrangeBounds);
        }

        protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
            ActiveContentChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAnchorableHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                AnchorableHeaderTemplate = null;
        }

        protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachAnchorablesSource(Layout, e.OldValue as IEnumerable);
            AttachAnchorablesSource(Layout, e.NewValue as IEnumerable);
        }

        protected virtual void OnAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAnchorableTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                AnchorableTitleTemplate != null)
                AnchorableTitleTemplate = null;
        }

        protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected virtual void OnDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnDocumentHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                DocumentHeaderTemplate != null)
                DocumentHeaderTemplate = null;

            if (DocumentPaneMenuItemHeaderTemplateSelector == null)
                DocumentPaneMenuItemHeaderTemplateSelector = DocumentHeaderTemplateSelector;
        }

        protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                DocumentPaneMenuItemHeaderTemplate != null)
                DocumentPaneMenuItemHeaderTemplate = null;
        }

        protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachDocumentsSource(Layout, e.OldValue as IEnumerable);
            AttachDocumentsSource(Layout, e.NewValue as IEnumerable);
        }

        protected virtual void OnDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                DocumentTitleTemplate = null;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (!Equals(Layout.Manager, this))
                return;
            LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
            LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
            TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
            RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
            BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;
        }

        protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
        {
            if (oldLayout != null)
            {
                oldLayout.PropertyChanged -= OnLayoutRootPropertyChanged;
                oldLayout.Updated -= OnLayoutRootUpdated;
            }

            foreach (var fwc in _fwList.ToArray())
            {
                fwc.KeepContentVisibleOnClose = true;
                fwc.InternalClose();
            }

            _fwList.Clear();

            DetachDocumentsSource(oldLayout, DocumentsSource);
            DetachAnchorablesSource(oldLayout, AnchorablesSource);

            if (oldLayout != null &&
                Equals(oldLayout.Manager, this))
                oldLayout.Manager = null;

            ClearLogicalChildrenList();
            DetachLayoutItems();

            Layout.Manager = this;

            AttachLayoutItems();
            AttachDocumentsSource(newLayout, DocumentsSource);
            AttachAnchorablesSource(newLayout, AnchorablesSource);

            if (IsLoaded)
            {
                LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;

                foreach (var fw in Layout.FloatingWindows.ToArray().Where(fw => fw.IsValid))
                    _fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);
            }


            if (newLayout != null)
            {
                newLayout.PropertyChanged += OnLayoutRootPropertyChanged;
                newLayout.Updated += OnLayoutRootUpdated;
            }

            LayoutChanged?.Invoke(this, EventArgs.Empty);

            //if (Layout != null)
            //    Layout.CollectGarbage();

            CommandManager.InvalidateRequerySuggested();
        }

        protected virtual void OnLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        protected virtual void OnLayoutItemContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        protected virtual void OnLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        ///     Provides derived classes an opportunity to handle changes to the LayoutItemTemplateSelector property.
        /// </summary>
        protected virtual void OnLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Trace.WriteLine($"DockingManager.OnMouseLeftButtonDown([{e.GetPosition(this)}])");
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine($"DockingManager.OnPreviewGotKeyboardFocus({e.NewFocus})");
            base.OnPreviewGotKeyboardFocus(e);
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                if (e.IsDown && e.Key == Key.Tab)
                    if (!IsNavigatorWindowActive)
                    {
                        ShowNavigatorWindow();
                        e.Handled = true;
                    }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine($"DockingManager.OnPreviewLostKeyboardFocus({e.OldFocus})");
            base.OnPreviewLostKeyboardFocus(e);
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }

        protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                InternalRemoveLogicalChild(e.OldValue);
            if (e.NewValue != null)
                InternalAddLogicalChild(e.NewValue);
        }

        protected void SetAutoHideWindow(LayoutAutoHideWindowControl value)
        {
            SetValue(AutoHideWindowPropertyKey, value);
        }

        private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(AnchorableHeaderTemplateSelectorProperty) != null)
                return null;

            return value;
        }

        private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(AnchorableTitleTemplateSelectorProperty) != null)
                return null;
            return value;
        }

        private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentHeaderTemplateSelectorProperty) != null)
                return null;
            return value;
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
                return null;
            return value ?? d.GetValue(DocumentHeaderTemplateProperty);
        }

        private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentTitleTemplateSelectorProperty) != null)
                return null;

            return value;
        }

        private static object CoerceLayoutValue(DependencyObject d, object value)
        {
            if (value == null)
                return new LayoutRoot
                {
                    RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
                };

            ((DockingManager)d).OnLayoutChanging(value as LayoutRoot);

            return value;
        }

        private static void DocumentPaneTabPanel_SelectedItemHidden(object sender, SelectedItemHiddenEventArgs e)
        {
            foreach (var indexChange in e.ViewsToMove)
            {
                if (!(indexChange.View.Parent is LayoutGroup<LayoutContent> lg))
                    return;

                lg.MoveChild(indexChange.NewIndex, indexChange.NewIndex - 1);
                break;
            }
        }

        private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).SetLastActiveContent(e.OldValue);
            ((DockingManager)d).InternalSetActiveContent(e.NewValue);
            ((DockingManager)d).OnActiveContentChanged(e);
        }

        private static void OnAnchorableHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableHeaderTemplateChanged(e);
        }

        private static void OnAnchorableHeaderTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableHeaderTemplateSelectorChanged(e);
        }

        private static void OnAnchorablePaneControlStyleChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneControlStyleChanged(e);
        }

        private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneTemplateChanged(e);
        }

        private static void OnAnchorablesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablesSourceChanged(e);
        }

        private static void OnAnchorableTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableTitleTemplateChanged(e);
        }

        private static void OnAnchorableTitleTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableTitleTemplateSelectorChanged(e);
        }

        private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAutoHideWindowChanged(e);
        }

        private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnBottomSidePanelChanged(e);
        }

        private static void OnDocumentHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentHeaderTemplateChanged(e);
        }

        private static void OnDocumentHeaderTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentHeaderTemplateSelectorChanged(e);
        }

        private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneControlStyleChanged(e);
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateChanged(e);
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
        }

        private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneTemplateChanged(e);
        }

        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentsSourceChanged(e);
        }

        private static void OnDocumentTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTitleTemplateChanged(e);
        }

        private static void OnDocumentTitleTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTitleTemplateSelectorChanged(e);
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
        }

        private static void OnLayoutItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemContainerStyleChanged(e);
        }

        private static void OnLayoutItemContainerStyleSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemContainerStyleSelectorChanged(e);

            var control = (DockingManager)d;
            if (control == null)
                return;

            var anchorablesSource = control.AnchorablesSource;
            control.DetachAnchorablesSource(control.Layout, control.AnchorablesSource);
            control.AttachAnchorablesSource(control.Layout, anchorablesSource);

            var documentsSource = control.DocumentsSource;
            control.DetachDocumentsSource(control.Layout, control.DocumentsSource);
            control.AttachDocumentsSource(control.Layout, documentsSource);
        }

        private static void OnLayoutItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemTemplateChanged(e);
        }

        private static void OnLayoutItemTemplateSelectorChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemTemplateSelectorChanged(e);
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutRootPanelChanged(e);
        }

        private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLeftSidePanelChanged(e);
        }

        private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnRightSidePanelChanged(e);
        }

        private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnTopSidePanelChanged(e);
        }

        private void AnchorablesSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Layout == null)
                return;

            //When deserializing documents are created automatically by the deserializer
            if (SuspendAnchorablesSourceBinding)
                return;

            //handle remove
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace)
                if (e.OldItems != null)
                {
                    var anchorablesToRemove =
                        Layout.Descendents()
                            .OfType<LayoutAnchorable>()
                            .Where(d => e.OldItems.Contains(d.Content))
                            .ToArray();
                    foreach (var anchorableToRemove in anchorablesToRemove)
                    {
                        anchorableToRemove.Parent.RemoveChild(
                            anchorableToRemove);
                        RemoveViewFromLogicalChild(anchorableToRemove);
                    }
                }

            //handle add
            if (e.NewItems != null &&
                (e.Action == NotifyCollectionChangedAction.Add ||
                 e.Action == NotifyCollectionChangedAction.Replace))
                if (e.NewItems != null)
                {
                    LayoutAnchorablePane anchorablePane = null;

                    if (Layout.ActiveContent != null)
                        anchorablePane = Layout.ActiveContent.Parent as LayoutAnchorablePane;

                    if (anchorablePane == null)
                        anchorablePane =
                            Layout.Descendents()
                                .OfType<LayoutAnchorablePane>()
                                .FirstOrDefault(
                                    pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right);

                    if (anchorablePane == null)
                        anchorablePane = Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();

                    _suspendLayoutItemCreation = true;
                    foreach (var anchorableContentToImport in e.NewItems)
                    {
                        var anchorableToImport = new LayoutAnchorable
                        {
                            Content = anchorableContentToImport
                        };

                        var added = false;
                        if (LayoutUpdateStrategy != null)
                            added = LayoutUpdateStrategy.BeforeInsertAnchorable(Layout, anchorableToImport,
                                anchorablePane);

                        if (!added)
                        {
                            if (anchorablePane == null)
                            {
                                var mainLayoutPanel = new LayoutPanel { Orientation = Orientation.Horizontal };
                                if (Layout.RootPanel != null)
                                    mainLayoutPanel.Children.Add(Layout.RootPanel);

                                Layout.RootPanel = mainLayoutPanel;
                                anchorablePane = new LayoutAnchorablePane
                                {
                                    DockWidth = new GridLength(200.0, GridUnitType.Pixel)
                                };
                                mainLayoutPanel.Children.Add(anchorablePane);
                            }

                            anchorablePane.Children.Add(anchorableToImport);
                        }

                        LayoutUpdateStrategy?.AfterInsertAnchorable(Layout, anchorableToImport);

                        var root = anchorableToImport.Root;

                        if (root != null && Equals(root.Manager, this))
                            CreateAnchorableLayoutItem(anchorableToImport);
                    }

                    _suspendLayoutItemCreation = false;
                }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //NOTE: I'm going to clear every anchorable present in layout but
                //some anchorable may have been added directly to the layout, for now I clear them too
                var anchorablesToRemove = Layout.Descendents().OfType<LayoutAnchorable>().ToArray();
                foreach (var anchorableToRemove in anchorablesToRemove)
                {
                    anchorableToRemove.Parent.RemoveChild(
                        anchorableToRemove);
                    RemoveViewFromLogicalChild(anchorableToRemove);
                }
            }

            Layout?.CollectGarbage();
        }

        private void ApplyStyleToLayoutItem(LayoutItem layoutItem)
        {
            layoutItem._ClearDefaultBindings();
            if (LayoutItemContainerStyle != null)
                layoutItem.Style = LayoutItemContainerStyle;
            else if (LayoutItemContainerStyleSelector != null)
                layoutItem.Style = LayoutItemContainerStyleSelector.SelectStyle(layoutItem.Model, layoutItem);
            layoutItem._SetDefaultBindings();
        }

        private void AttachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
        {
            if (anchorablesSource == null)
                return;

            if (layout == null)
                return;

            var anchorablesImported = layout.Descendents().OfType<LayoutAnchorable>().Select(d => d.Content).ToArray();
            var anchorables = anchorablesSource;
            var listOfAnchorablesToImport = new List<object>(anchorables.OfType<object>());

            foreach (
                var document in
                listOfAnchorablesToImport.ToArray().Where(document => anchorablesImported.Contains(document)))
                listOfAnchorablesToImport.Remove(document);

            LayoutAnchorablePane anchorablePane = null;
            if (layout.ActiveContent != null)
                anchorablePane = layout.ActiveContent.Parent as LayoutAnchorablePane;

            if (anchorablePane == null)
                anchorablePane =
                    layout.Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .FirstOrDefault(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right);

            if (anchorablePane == null)
                anchorablePane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();

            _suspendLayoutItemCreation = true;
            foreach (var anchorableContentToImport in listOfAnchorablesToImport)
            {
                var anchorableToImport = new LayoutAnchorable
                {
                    Content = anchorableContentToImport
                };

                var added = false;
                if (LayoutUpdateStrategy != null)
                    added = LayoutUpdateStrategy.BeforeInsertAnchorable(layout, anchorableToImport, anchorablePane);

                if (!added)
                {
                    if (anchorablePane == null)
                    {
                        var mainLayoutPanel = new LayoutPanel { Orientation = Orientation.Horizontal };
                        if (layout.RootPanel != null)
                            mainLayoutPanel.Children.Add(layout.RootPanel);

                        layout.RootPanel = mainLayoutPanel;
                        anchorablePane = new LayoutAnchorablePane
                        {
                            DockWidth = new GridLength(200.0, GridUnitType.Pixel)
                        };
                        mainLayoutPanel.Children.Add(anchorablePane);
                    }

                    anchorablePane.Children.Add(anchorableToImport);
                }

                LayoutUpdateStrategy?.AfterInsertAnchorable(layout, anchorableToImport);
                CreateAnchorableLayoutItem(anchorableToImport);
            }

            _suspendLayoutItemCreation = false;

            if (anchorablesSource is INotifyCollectionChanged anchorablesSourceAsNotifier)
                anchorablesSourceAsNotifier.CollectionChanged += AnchorablesSourceElementsChanged;
        }

        private void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
                return;

            if (layout == null)
                return;

            var documentsImported = layout.Descendents().OfType<LayoutDocument>().Select(d => d.Content).ToArray();
            var documents = documentsSource;
            var listOfDocumentsToImport = new List<object>(documents.OfType<object>());

            foreach (
                var document in
                listOfDocumentsToImport.ToArray().Where(document => documentsImported.Contains(document)))
                listOfDocumentsToImport.Remove(document);

            LayoutDocumentPane documentPane = null;
            if (layout.LastFocusedDocument != null)
                documentPane = layout.LastFocusedDocument.Parent as LayoutDocumentPane;

            if (documentPane == null)
                documentPane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

            _suspendLayoutItemCreation = true;
            foreach (var documentContentToImport in listOfDocumentsToImport)
            {
                var documentToImport = new LayoutDocument
                {
                    Content = documentContentToImport
                };

                var added = false;
                if (LayoutUpdateStrategy != null)
                    added = LayoutUpdateStrategy.BeforeInsertDocument(layout, documentToImport, documentPane);

                if (!added)
                {
                    if (documentPane == null)
                        throw new InvalidOperationException(
                            "Layout must contains at least one LayoutDocumentPane in order to host documents");

                    documentPane.Children.Add(documentToImport);

                    //documentPane.Children.Insert(0, documentToImport);
                }

                LayoutUpdateStrategy?.AfterInsertDocument(layout, documentToImport);


                CreateDocumentLayoutItem(documentToImport);
            }

            _suspendLayoutItemCreation = true;


            if (documentsSource is INotifyCollectionChanged documentsSourceAsNotifier)
                documentsSourceAsNotifier.CollectionChanged += DocumentsSourceElementsChanged;
        }


        private void AttachLayoutItems()
        {
            if (Layout == null)
                return;
            foreach (var document in Layout.Descendents().OfType<LayoutDocument>().ToArray())
                CreateDocumentLayoutItem(document);
            foreach (var anchorable in Layout.Descendents().OfType<LayoutAnchorable>().ToArray())
                CreateAnchorableLayoutItem(anchorable);

            Layout.ElementAdded += Layout_ElementAdded;
            Layout.ElementRemoved += Layout_ElementRemoved;
        }

        private void ClearLogicalChildrenList()
        {
            foreach (var child in _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).ToArray())
                RemoveLogicalChild(child);
            _logicalChildren.Clear();
        }

        private void CollectLayoutItemsDeleted()
        {
            if (_collectLayoutItemsOperations != null)
                return;
            _collectLayoutItemsOperations = Dispatcher.BeginInvoke(new Action(() =>
            {
                _collectLayoutItemsOperations = null;
                foreach (
                    var itemToRemove in _layoutItems.Where(item => !Equals(item.LayoutElement.Root, Layout)).ToArray())
                {
                    if (itemToRemove?.Model is UIElement)
                    {
                    }

                    if (itemToRemove == null)
                        continue;
                    itemToRemove.Detach();
                    _layoutItems.Remove(itemToRemove);
                }
            }));
        }

        private void CreateAnchorableLayoutItem(LayoutAnchorable contentToAttach)
        {
            if (_layoutItems.Any(item => Equals(item.LayoutElement, contentToAttach)))
                return;

            var layoutItem = new LayoutAnchorableItem();
            layoutItem.Attach(contentToAttach);
            ApplyStyleToLayoutItem(layoutItem);
            _layoutItems.Add(layoutItem);

            if (contentToAttach?.Content is UIElement)
                InternalAddLogicalChild(contentToAttach.Content);
        }

        private void CreateDocumentLayoutItem(LayoutDocument contentToAttach)
        {
            if (_layoutItems.Any(item => Equals(item.LayoutElement, contentToAttach)))
                return;

            var layoutItem = new LayoutDocumentItem();
            layoutItem.Attach(contentToAttach);
            ApplyStyleToLayoutItem(layoutItem);
            _layoutItems.Add(layoutItem);

            if (contentToAttach?.Content is UIElement)
                InternalAddLogicalChild(contentToAttach.Content);
        }

        private void CreateOverlayWindow()
        {
            if (_overlayWindow == null)
                _overlayWindow = new OverlayWindow(this);
            var rectWindow = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
                this.TransformActualSizeToAncestor());
            _overlayWindow.Left = rectWindow.Left;
            _overlayWindow.Top = rectWindow.Top;
            _overlayWindow.Width = rectWindow.Width;
            _overlayWindow.Height = rectWindow.Height;
        }

        private void DestroyOverlayWindow()
        {
            if (_overlayWindow == null)
                return;
            _overlayWindow.Close();
            _overlayWindow = null;
        }

        private void DetachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
        {
            if (anchorablesSource == null)
                return;

            if (layout == null)
                return;

            var anchorablesToRemove = layout.Descendents().OfType<LayoutAnchorable>()
                .Where(d => anchorablesSource.Contains(d.Content)).ToArray();

            foreach (var anchorableToRemove in anchorablesToRemove)
            {
                anchorableToRemove.Parent.RemoveChild(
                    anchorableToRemove);
                RemoveViewFromLogicalChild(anchorableToRemove);
            }

            if (anchorablesSource is INotifyCollectionChanged anchorablesSourceAsNotifier)
                anchorablesSourceAsNotifier.CollectionChanged -= AnchorablesSourceElementsChanged;
        }

        private void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
                return;

            if (layout == null)
                return;

            var documentsToRemove = layout.Descendents().OfType<LayoutDocument>()
                .Where(d => documentsSource.Contains(d.Content)).ToArray();

            foreach (var documentToRemove in documentsToRemove)
            {
                documentToRemove.Parent.RemoveChild(
                    documentToRemove);
                RemoveViewFromLogicalChild(documentToRemove);
            }

            if (documentsSource is INotifyCollectionChanged documentsSourceAsNotifier)
                documentsSourceAsNotifier.CollectionChanged -= DocumentsSourceElementsChanged;
        }

        private void DetachLayoutItems()
        {
            if (Layout == null)
                return;
            _layoutItems.ForEach<LayoutItem>(i => i.Detach());
            _layoutItems.Clear();
            Layout.ElementAdded -= Layout_ElementAdded;
            Layout.ElementRemoved -= Layout_ElementRemoved;
        }

        private void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            //load windows not already loaded!
            foreach (var fw in Layout.FloatingWindows.Where(fw => !_fwList.Any(fwc => Equals(fwc.Model, fw))).ToList())
            {
                if (CreateUIElementForModel(fw) is LayoutFloatingWindowControl element && element.EnsureHandled())
                    _fwList.Add(element);
            }


            //create the overlaywindow if it's possible
            if (IsVisible)
                CreateOverlayWindow();
            FocusElementManager.SetupFocusManagement(this);
        }

        private void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            _autoHideWindowManager.HideAutoWindow();

            AutoHideWindow?.Dispose();

            foreach (var fw in _fwList.ToArray())
            {
                //fw.Owner = null;
                fw.SetParentWindowToNull();
                fw.KeepContentVisibleOnClose = true;
                fw.Close();
            }

            DestroyOverlayWindow();
            FocusElementManager.FinalizeFocusManagement(this);
        }

        private void DocumentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Layout == null)
                return;

            //When deserializing documents are created automatically by the deserializer
            if (SuspendDocumentsSourceBinding)
                return;

            //handle remove
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Replace)
                if (e.OldItems != null)
                {
                    var documentsToRemove =
                        Layout.Descendents()
                            .OfType<LayoutDocument>()
                            .Where(d => e.OldItems.Contains(d.Content))
                            .ToArray();
                    foreach (var documentToRemove in documentsToRemove)
                    {
                        documentToRemove.Parent.RemoveChild(documentToRemove);
                        RemoveViewFromLogicalChild(documentToRemove);
                    }
                }

            //handle add
            if (e.NewItems != null &&
                (e.Action == NotifyCollectionChangedAction.Add ||
                 e.Action == NotifyCollectionChangedAction.Replace))
                if (e.NewItems != null)
                {
                    LayoutDocumentPane documentPane = null;
                    if (Layout.LastFocusedDocument != null)
                        documentPane = Layout.LastFocusedDocument.Parent as LayoutDocumentPane;

                    if (documentPane == null)
                        documentPane = Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();

                    _suspendLayoutItemCreation = true;

                    foreach (var documentContentToImport in e.NewItems)
                    {
                        var documentToImport = new LayoutDocument
                        {
                            Content = documentContentToImport
                        };

                        var added = false;
                        if (LayoutUpdateStrategy != null)
                            added = LayoutUpdateStrategy.BeforeInsertDocument(Layout, documentToImport, documentPane);

                        if (!added)
                        {
                            if (documentPane == null)
                                throw new InvalidOperationException(
                                    "Layout must contains at least one LayoutDocumentPane in order to host documents");

                            if (DockingManagerPreferences.Instance.DocumentDockPreference ==
                                DockPreference.DockAtBeginning)
                                documentPane.Children.Insert(0, documentToImport);
                            else
                                documentPane.Children.Add(documentToImport);
                        }

                        LayoutUpdateStrategy?.AfterInsertDocument(Layout, documentToImport);

                        var root = documentToImport.Root;

                        if (root != null && Equals(root.Manager, this))
                            CreateDocumentLayoutItem(documentToImport);
                    }

                    _suspendLayoutItemCreation = false;
                }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //NOTE: I'm going to clear every document present in layout but
                //some documents may have been added directly to the layout, for now I clear them too
                var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().ToArray();
                foreach (var documentToRemove in documentsToRemove)
                {
                    documentToRemove.Parent.RemoveChild(
                        documentToRemove);
                    RemoveViewFromLogicalChild(documentToRemove);
                }
            }

            Layout?.CollectGarbage();
        }

        private void InternalSetActiveContent(object contentObject)
        {
            var layoutContent =
                Layout.Descendents()
                    .OfType<LayoutContent>()
                    .OrderBy(lc => lc.LastActivationTimeStamp)
                    .FirstOrDefault(lc => Equals(lc, contentObject) || lc.Content == contentObject);

            _insideInternalSetActiveContent = true;
            Layout.ActiveContent = layoutContent;
            _insideInternalSetActiveContent = false;
        }

        private void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
        {
            if (_suspendLayoutItemCreation)
                return;

            foreach (var content in Layout.Descendents().OfType<LayoutContent>())
                if (content is LayoutDocument document)
                    CreateDocumentLayoutItem(document);
                else //if (content is LayoutAnchorable)
                    CreateAnchorableLayoutItem(content as LayoutAnchorable);

            CollectLayoutItemsDeleted();
        }

        private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
        {
            if (_suspendLayoutItemCreation)
                return;

            CollectLayoutItemsDeleted();
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnLayoutChanging(LayoutRoot newLayout)
        {
            LayoutChanging?.Invoke(this, EventArgs.Empty);
        }

        private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RootPanel":
                    if (IsInitialized)
                    {
                        var layoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                        LayoutRootPanel = layoutRootPanel;
                    }

                    break;
                case "ActiveContent":
                    if (Layout.ActiveContent != null)
                        FocusElementManager.SetFocusOnLastElement(Layout.ActiveContent);

                    if (!_insideInternalSetActiveContent && Layout.ActiveContent != null)
                        ActiveContent = Layout.ActiveContent?.Content;
                    break;
            }
        }

        private void OnLayoutRootUpdated(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }


        private void RemoveViewFromLogicalChild(LayoutContent layoutContent)
        {
            if (layoutContent == null)
                return;

            var layoutItem = GetLayoutItemFromModel(layoutContent);
            if (layoutItem?.Parent == null)
                return;
            var view = layoutItem.View;
            InternalRemoveLogicalChild(view);
        }

        private void SetupAutoHideWindow()
        {
            _autohideArea = GetTemplateChild("PART_AutoHideArea") as FrameworkElement;

            if (_autoHideWindowManager != null)
                _autoHideWindowManager.HideAutoWindow();
            else
                _autoHideWindowManager = new AutoHideWindowManager(this);

            AutoHideWindow?.Dispose();

            SetAutoHideWindow(new LayoutAutoHideWindowControl());
        }

        private void ShowNavigatorWindow()
        {
            if (_navigatorWindow == null)
                _navigatorWindow = new NavigatorWindow(this)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

            _navigatorWindow.ShowDialog();
            _navigatorWindow = null;

            Trace.WriteLine("ShowNavigatorWindow()");
        }

        internal void ComputeTabItemLengths(TabItem tabItem)
        {
            if (tabItem == null)
                throw new ArgumentNullException(nameof(tabItem));
            if (tabItem.DataContext is LayoutContent && tabItem.IsConnectedToPresentationSource())
            {
                if (tabItem.GetVisualOrLogicalParent() is ReorderTabPanel visualOrLogicalParent)
                {
                    var point = visualOrLogicalParent.TransformToDescendant(tabItem).Transform(new Point(0.0, 0.0));
                    double num1;
                    double num2;
                    if (visualOrLogicalParent.IsVerticallyOriented)
                    {
                        num1 = -point.Y;
                        num2 = tabItem.ActualHeight;
                    }
                    else
                    {
                        num1 = -point.X;
                        num2 = tabItem.ActualWidth;
                    }
                    UndockedTabItemOffset = num1;
                    UndockedTabItemLength = num2;
                    return;
                }
            }
            UndockedTabItemOffset = 0.0;
            UndockedTabItemLength = 0.0;
        }

        internal double UndockedTabItemOffset { get; set; }

        internal double UndockedTabItemLength { get; set; }

        private class DraggedTabScope : DisposableObject
        {
            private static int _refCount;

            public DraggedTabScope()
            {
                ++_refCount;
            }

            protected override void DisposeManagedResources()
            {
                --_refCount;
            }

            public static bool IsDraggingTab => _refCount > 0;
        }

        public void ClearAdorners()
        {
            _overlayWindow.HideDropTargets();
        }
    }
}