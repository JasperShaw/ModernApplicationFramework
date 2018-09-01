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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public class DocumentPaneTabPanel : ReorderTabPanel
    {
        private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasOverflowItems), typeof(bool), typeof(DocumentPaneTabPanel),
                new PropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty HasOverflowItemsProperty =
            HasOverflowItemsPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsAdjacentToDocumentWellPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsAdjacentToDocumentWell", typeof(bool),
                typeof(DocumentPaneTabPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty IsAdjacentToDocumentWellProperty =
            IsAdjacentToDocumentWellPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey PinnedTabsRectPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(PinnedTabsRect), typeof(Rect), typeof(DocumentPaneTabPanel),
                new PropertyMetadata(Rect.Empty));

        public static readonly DependencyProperty PinnedTabsRectProperty = PinnedTabsRectPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey StartNewRowAfterPinnedTabsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(StartNewRowAfterPinnedTabs), typeof(bool),
                typeof(DocumentPaneTabPanel), new PropertyMetadata(Boxes.BooleanFalse));

        public static readonly DependencyProperty StartNewRowAfterPinnedTabsProperty =
            StartNewRowAfterPinnedTabsPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey UnpinnedTabsRectPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(UnpinnedTabsRect), typeof(Rect), typeof(DocumentPaneTabPanel),
                new PropertyMetadata(Rect.Empty));

        public static readonly DependencyProperty UnpinnedTabsRectProperty =
            UnpinnedTabsRectPropertyKey.DependencyProperty;


        private static readonly DependencyProperty SeparatePinnedTabsFromUnpinnedTabsProperty =
            DependencyProperty.Register(nameof(SeparatePinnedTabsFromUnpinnedTabs), typeof(bool),
                typeof(DocumentPaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));


        public static event EventHandler<SelectedItemHiddenEventArgs> SelectedItemHidden;

        public LayoutDocumentPane DocumentPane
        {
            get
            {
                var i = this.FindLogicalAncestor<LayoutDocumentPaneControl>();
                return i.Model as LayoutDocumentPane;
                ;
            }
        }

        public bool HasOverflowItems
        {
            get => (bool) GetValue(HasOverflowItemsProperty);
            private set => SetValue(HasOverflowItemsPropertyKey, Boxes.Box(value));
        }

        public Rect PinnedTabsRect
        {
            get => (Rect) GetValue(PinnedTabsRectProperty);
            private set => SetValue(PinnedTabsRectPropertyKey, value);
        }

        public bool SeparatePinnedTabsFromUnpinnedTabs => (bool) GetValue(SeparatePinnedTabsFromUnpinnedTabsProperty);

        public bool StartNewRowAfterPinnedTabs
        {
            get => (bool) GetValue(StartNewRowAfterPinnedTabsProperty);
            private set => SetValue(StartNewRowAfterPinnedTabsPropertyKey, Boxes.Box(value));
        }

        public Rect UnpinnedTabsRect
        {
            get => (Rect) GetValue(UnpinnedTabsRectProperty);
            private set => SetValue(UnpinnedTabsRectPropertyKey, value);
        }

        private bool DependentCollectionChangedDuringMeasure { get; set; }


        private bool MeasureInProgress { get; set; }


        public DocumentPaneTabPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            BindingOperations.SetBinding(this, SeparatePinnedTabsFromUnpinnedTabsProperty, new Binding
            {
                Source = DockingManagerPreferences.Instance,
                Path = new PropertyPath(DockingManagerPreferences.IsPinnedTabPanelSeparateProperty),
                Mode = BindingMode.OneWay
            });

        }

        public static bool GetIsAdjacentToDocumentWell(LayoutContent view)
        {
            Validate.IsNotNull(view, nameof(view));
            return (bool)view.GetValue(IsAdjacentToDocumentWellProperty);
        }

        private static void SetIsAdjacentToDocumentWell(LayoutContent view, bool value)
        {
            Validate.IsNotNull(view, nameof(view));
            view.SetValue(IsAdjacentToDocumentWellPropertyKey, Boxes.Box(value));
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            new ArrangeHelper(this, finalSize).Arrange();
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size;
            MeasureInProgress = true;

            try
            {
                var num = 0;
                do
                {
                    DependentCollectionChangedDuringMeasure = false;
                    size = new MeasureHelper(this, availableSize).Measure();
                    if (!DependentCollectionChangedDuringMeasure)
                        break;
                } while (num++ < 5);
            }
            finally
            {
                MeasureInProgress = false;
            }
            return size;
        }

        protected override void OnDependentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MeasureInProgress)
                DependentCollectionChangedDuringMeasure = true;
            base.OnDependentCollectionChanged(sender, e);
        }

        private static bool HasZeroArea(Rect rect)
        {
            if (!rect.IsEmpty && !rect.Width.IsNearlyEqual(0.0))
                return rect.Height.IsNearlyEqual(0.0);
            return true;
        }

        public class ArrangeHelper : LayoutHelper
        {
            private Point _origin;

            protected override double RemainingWidth => Math.Max(PanelConstraint.Width - _origin.X, 0.0);

            public ArrangeHelper(DocumentPaneTabPanel panel, Size panelConstraint) : base(panel, panelConstraint)
            {
            }

            internal void Arrange()
            {
                ArrangePinnedTabs();
                ArrangeUnpinnedTabs();
            }

            private void ArrangePinnedTabs()
            {
                var num1 = 0;
                var num2 = 0.0;
                TabItem tabItem = null;
                foreach (var pinnedTab in PinnedTabs)
                {
                    if (_origin.X == 0.0)
                        tabItem = pinnedTab;
                    if (TabOverflowsCurrentRow(pinnedTab))
                    {
                        StartNewRow(num2);
                        num2 = 0.0;
                        num1 = 0;
                    }
                    ArrangeTab(pinnedTab);
                    num2 = Math.Max(num2, pinnedTab.DesiredSize.Height);
                    ++num1;
                }
                var flag1 = false;
                var flag2 = Panel.StartNewRowAfterPinnedTabs & (UnpinnedTabs.Count > 0);
                foreach (var pinnedTab in PinnedTabs)
                {
                    if (pinnedTab == tabItem && !flag2)
                        flag1 = true;
                    SetIsAdjacentToDocumentWell(GetView(pinnedTab), flag1);
                }             
                if (Panel.StartNewRowAfterPinnedTabs)
                    StartNewRow(num2);
                var rect = _origin.Y == 0.0
                    ? new Rect(0.0, 0.0, _origin.X, num2)
                    : new Rect(0.0, 0.0, PanelConstraint.Width, _origin.Y);
                Panel.PinnedTabsRect = HasZeroArea(rect) ? Rect.Empty : rect;
            }

            private void ArrangeTab(TabItem tab)
            {
                if (tab.Visibility != Visibility.Collapsed)
                {
                    tab.Arrange(new Rect(_origin, tab.DesiredSize));
                    _origin.X += tab.DesiredSize.Width;
                }
            }

            private void ArrangeUnpinnedTabs()
            {
                var origin = _origin;
                var num = 0.0;
                foreach (var unpinnedTab in UnpinnedTabs)
                {
                    ArrangeTab(unpinnedTab);
                    SetIsAdjacentToDocumentWell(GetView(unpinnedTab), true);
                    num = Math.Max(num, unpinnedTab.DesiredSize.Height);
                }
                var size = new Size(Math.Max(PanelConstraint.Width - origin.X, 0.0), num);
                var rect = new Rect(origin, size);
                Panel.UnpinnedTabsRect = HasZeroArea(rect) ? Rect.Empty : rect;
            }

            private void StartNewRow(double currentRowHeight)
            {
                _origin.X = 0.0;
                _origin.Y += currentRowHeight;
                StartNewRow();
            }
        }

        public abstract class LayoutHelper
        {
            protected readonly DocumentPaneTabPanel Panel;
            protected readonly Size PanelConstraint;
            protected readonly List<TabItem> PinnedTabs;
            protected readonly TabItem PreviewTab;
            protected readonly List<TabItem> UnpinnedTabs;

            protected abstract double RemainingWidth { get; }

            protected int RowIndex { get; private set; }

            protected LayoutHelper(DocumentPaneTabPanel panel, Size panelConstraint)
            {
                Validate.IsNotNull(panel, nameof(panel));
                Panel = panel;
                PanelConstraint = panelConstraint;

                var observableCollection = panel.DocumentPane?.PinnedViews;

                var count = panel.InternalChildren.Count;
                var capacity1 = observableCollection?.Count ?? 0;
                var capacity2 = Math.Max(count - capacity1, 0);

                UnpinnedTabs = new List<TabItem>(capacity1);
                PinnedTabs = new List<TabItem>(capacity2);


                foreach (TabItem internalChild in panel.InternalChildren)
                {
                    var content = GetView(internalChild);
                    if (content.IsPinned)
                        PinnedTabs.Add(internalChild);
                    else
                        UnpinnedTabs.Add(internalChild);
                }
            }

            [Flags]
            protected enum TracedTabItemState
            {
                None = 0,
                Selected = 1,
                Hidden = 2,
                Minimized = 4,
                Pinned = 8,
                StartNewRow = 16
            }

            protected void StartNewRow()
            {
                RowIndex = RowIndex + 1;
            }

            protected bool TabFitsOnCurrentRow(TabItem tab)
            {
                return !TabOverflowsCurrentRow(tab);
            }

            protected bool TabOverflowsCurrentRow(TabItem tab)
            {
                return tab.DesiredSize.Width.IsSignificantlyGreaterThan(RemainingWidth);
            }
        }

        private class MeasureHelper : LayoutHelper
        {
            private readonly Size _tabConstraint;
            private Size _cumulativePanelSize;
            private Size _currentRowSize;
            private int _currentTabIndex;
            private int _tabsInCurrentRow;

            protected override double RemainingWidth => Math.Max(PanelConstraint.Width - _currentRowSize.Width, 0.0);

            private Size RemainingSize => new Size(RemainingWidth, PanelConstraint.Height);

            public MeasureHelper(DocumentPaneTabPanel panel, Size panelConstraint)
                : base(panel, panelConstraint)
            {
                _tabConstraint = new Size(double.PositiveInfinity, PanelConstraint.Height);
            }

            internal Size Measure()
            {
                MeasurePinnedTabs();
                //this.MeasurePreviewTab();
                MeasureUnpinnedTabs();
                _cumulativePanelSize.Width = PreviewTab == null
                    ? Math.Min(_cumulativePanelSize.Width + 0.0, PanelConstraint.Width)
                    : PanelConstraint.Width;
                return _cumulativePanelSize;
            }

            private void AddToCurrentRow(UIElement tab)
            {
                _currentRowSize.Width += tab.DesiredSize.Width;
                _currentRowSize.Height = Math.Max(_currentRowSize.Height, tab.DesiredSize.Height);
                _tabsInCurrentRow = _tabsInCurrentRow + 1;
                _cumulativePanelSize.Width = Math.Max(_cumulativePanelSize.Width, _currentRowSize.Width);
            }

            private void MeasurePinnedTabs()
            {
                Panel.StartNewRowAfterPinnedTabs = false;
                foreach (var pinnedTab in PinnedTabs)
                {
                    var num = _currentTabIndex + 1;
                    _currentTabIndex = num;
                    pinnedTab.Visibility = Visibility.Visible;
                    pinnedTab.Measure(_tabConstraint);
                    if (TabOverflowsCurrentRow(pinnedTab))
                        if (_tabsInCurrentRow == 0)
                            pinnedTab.Measure(RemainingSize);
                        else
                            StartNewRow();
                    AddToCurrentRow(pinnedTab);
                }
                if (!Panel.StartNewRowAfterPinnedTabs)
                    Panel.StartNewRowAfterPinnedTabs =
                        Panel.SeparatePinnedTabsFromUnpinnedTabs && _tabsInCurrentRow > 0;
                if (!Panel.StartNewRowAfterPinnedTabs)
                    return;
                StartNewRow();
            }

            private void MeasureUnpinnedTab(TabItem tab, bool isFirstUnpinnedTab)
            {
                tab.Visibility = Visibility.Visible;
                tab.Measure(_tabConstraint);
                if (TabFitsOnCurrentRow(tab))
                    return;
                if (!isFirstUnpinnedTab)
                    return;
                tab.Measure(RemainingSize);
            }

            private void MeasureUnpinnedTabs()
            {
                var flag = false;
                var isFirstUnpinnedTab = true;
                foreach (var unpinnedTab in UnpinnedTabs)
                {
                    if (!flag)
                    {
                        MeasureUnpinnedTab(unpinnedTab, isFirstUnpinnedTab);
                        isFirstUnpinnedTab = false;
                    }
                    var visibility = Visibility.Visible;
                    if (flag || TabOverflowsCurrentRow(unpinnedTab))
                    {
                        visibility = Visibility.Collapsed;
                        flag = true;
                        if (GetView(unpinnedTab).IsSelected)
                            RaiseSelectedItemHidden(unpinnedTab);
                    }
                    else
                    {
                        AddToCurrentRow(unpinnedTab);
                    }
                    unpinnedTab.Visibility = visibility;
                }
                Panel.HasOverflowItems = flag;
                _cumulativePanelSize.Height += _currentRowSize.Height;
            }

            private void RaiseSelectedItemHidden(TabItem selected)
            {
                var viewsToMove = new List<SelectedItemHiddenEventArgs.ViewIndexChange>();
                var source = UnpinnedTabs.TakeWhile(tab =>
                {
                    if (tab != selected)
                        return tab.Visibility == Visibility.Visible;
                    return false;
                });
                var newIndex = PinnedTabs.Count + source.Count();
                viewsToMove.Add(new SelectedItemHiddenEventArgs.ViewIndexChange(GetView(selected), newIndex));
                if (!TabFitsOnCurrentRow(selected))
                    foreach (var tabItem in source.ToList())
                    {
                        var view = GetView(tabItem);
                        if (view?.Parent == null)
                            continue;
                        viewsToMove.Add(new SelectedItemHiddenEventArgs.ViewIndexChange(view, newIndex--));
                        _currentRowSize.Width = Math.Max(_currentRowSize.Width - tabItem.DesiredSize.Width, 0.0);
                        if (TabFitsOnCurrentRow(selected))
                            break;
                    }
                SelectedItemHidden.RaiseEvent(null, new SelectedItemHiddenEventArgs(viewsToMove));
            }

            private new void StartNewRow()
            {
                _cumulativePanelSize.Height += _currentRowSize.Height;
                _currentRowSize.Width = 0.0;
                _currentRowSize.Height = 0.0;
                _tabsInCurrentRow = 0;
                Panel.StartNewRowAfterPinnedTabs = true;
                base.StartNewRow();
            }
        }
    }

    public class ReorderTabPanel : Panel, IWeakEventListener
    {
        public static readonly DependencyProperty ExpandedTearOffMarginProperty =
            DependencyProperty.Register(nameof(ExpandedTearOffMargin), typeof(Thickness), typeof(ReorderTabPanel),
                new FrameworkPropertyMetadata(new Thickness(0.0)));

        public static RoutedEvent PanelLayoutUpdatedEvent = EventManager.RegisterRoutedEvent("PanelLayoutUpdated",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ReorderTabPanel));

        private bool _layoutUpdatedHandlerAdded;

        public Thickness ExpandedTearOffMargin
        {
            get => (Thickness) GetValue(ExpandedTearOffMarginProperty);
            set => SetValue(ExpandedTearOffMarginProperty, value);
        }

        public bool IsNotificationNeeded
        {
            get => _layoutUpdatedHandlerAdded;
            set
            {
                if (value == _layoutUpdatedHandlerAdded)
                    return;
                if (value)
                    LayoutUpdated += OnLayoutUpdated;
                else
                    LayoutUpdated -= OnLayoutUpdated;
                _layoutUpdatedHandlerAdded = value;
            }
        }


        public ReorderTabPanel()
        {
            _layoutUpdatedHandlerAdded = false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(CollectionChangedEventManager)))
                return false;
            OnDependentCollectionChanged(sender, (NotifyCollectionChangedEventArgs) e);
            return true;
        }

        protected static LayoutContent GetView(UIElement child)
        {
            if (child is FrameworkElement frameworkElement)
                return frameworkElement.DataContext as LayoutContent;
            return null;
        }

        protected virtual void OnDependentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(PanelLayoutUpdatedEvent));
            IsNotificationNeeded = false;
        }
    }

    public class SelectedItemHiddenEventArgs : EventArgs
    {
        public List<ViewIndexChange> ViewsToMove { get; }

        public SelectedItemHiddenEventArgs(List<ViewIndexChange> viewsToMove)
        {
            ViewsToMove = viewsToMove;
        }

        public class ViewIndexChange
        {
            public int NewIndex { get; }
            public LayoutContent View { get; }

            public ViewIndexChange(LayoutContent view, int newIndex)
            {
                View = view;
                NewIndex = newIndex;
            }
        }
    }
}