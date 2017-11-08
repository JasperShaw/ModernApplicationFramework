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

        private static readonly DependencyProperty SeparatePinnedTabsFromUnpinnedTabsProperty =
            DependencyProperty.Register(nameof(SeparatePinnedTabsFromUnpinnedTabs), typeof(bool),
                typeof(DocumentPaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

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

        public static event EventHandler<SelectedItemHiddenEventArgs> SelectedItemHidden;


        private int _indexOfFirstVisibleElement;
        private int _indexOfLastVisibleElement;
        private Size _oldFinalSize;
        private int _selectedIndex;


        private bool MeasureInProgress { get; set; }

        private bool DependentCollectionChangedDuringMeasure { get; set; }

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


        public DocumentPaneTabPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            FlowDirection = FlowDirection.LeftToRight;
            ClipToBounds = true;
        }

        private static bool HasZeroArea(Rect rect)
        {
            if (!rect.IsEmpty && !rect.Width.IsNearlyEqual(0.0))
                return rect.Height.IsNearlyEqual(0.0);
            return true;
        }

        protected override void OnDependentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MeasureInProgress)
                DependentCollectionChangedDuringMeasure = true;
            base.OnDependentCollectionChanged(sender, e);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {


            new ArrangeHelper(this, finalSize).Arrange();
            return finalSize;




            //var tabs = Children.Cast<TabItem>().ToList();
            //var offset = 0.0;

            //var selectedItem = tabs.Where(item => item.Content is LayoutContent)
            //    .FirstOrDefault(item => item.IsSelected);
            //var selectedIndex = Children.IndexOf(selectedItem);

            //var indexOfFirstVisibleElement = 0;
            //var indexOfLastVisibleElement = 0;

            //// check visible range of tab items
            //foreach (var tab in tabs)
            //{
            //    var desiredWidth = Math.Min(tab.ActualWidth, tab.DesiredSize.Width) + tab.Margin.Left +
            //                       tab.Margin.Right;
            //    if (offset + desiredWidth > finalSize.Width)
            //    {
            //        if (indexOfLastVisibleElement > selectedIndex)
            //        {
            //            indexOfLastVisibleElement--;
            //            break;
            //        }
            //    }

            //    //remove leading elements until current element comes to view
            //    while (offset + desiredWidth > finalSize.Width)
            //    {
            //        offset -=
            //            Math.Min(tabs[indexOfFirstVisibleElement].ActualWidth,
            //                tabs[indexOfFirstVisibleElement].DesiredSize.Width) -
            //            tabs[indexOfFirstVisibleElement].Margin.Left - tabs[indexOfFirstVisibleElement].Margin.Right;

            //        if (indexOfFirstVisibleElement <= selectedIndex && indexOfLastVisibleElement >= selectedIndex)
            //            break;
            //    }
            //    offset += desiredWidth;
            //    indexOfLastVisibleElement++;
            //}

            //// don't move the visible range, if not necessary. User can select a item in the middle
            //if (selectedIndex >= _indexOfFirstVisibleElement && selectedIndex <= _indexOfLastVisibleElement &&
            //    _oldFinalSize.Width == finalSize.Width && _selectedIndex != selectedIndex)
            //{
            //    indexOfFirstVisibleElement = _indexOfFirstVisibleElement;
            //    indexOfLastVisibleElement = _indexOfLastVisibleElement;
            //}

            //ShowHideTabs(tabs, finalSize, indexOfFirstVisibleElement, indexOfLastVisibleElement, selectedIndex);

            //_indexOfFirstVisibleElement = indexOfFirstVisibleElement;
            //_indexOfLastVisibleElement = indexOfLastVisibleElement;
            //_oldFinalSize = finalSize;
            //_selectedIndex = selectedIndex;

            //return finalSize;
        }

        public class ArrangeHelper : LayoutHelper
        {
            private Point _origin;

            public ArrangeHelper(DocumentPaneTabPanel panel, Size panelConstraint) : base(panel, panelConstraint)
            {
            }

            internal void Arrange()
            {
                ArrangeUnpinnedTabs();
            }

            private void ArrangeUnpinnedTabs()
            {
                var origin = _origin;
                var num = 0.0;
                foreach (var unpinnedTab in UnpinnedTabs)
                {
                    ArrangeTab(unpinnedTab);
                    num = Math.Max(num, unpinnedTab.DesiredSize.Height);
                }
                var size = new Size(Math.Max(PanelConstraint.Width - origin.X, 0.0), num);
                var rect = new Rect(origin, size);
                Panel.UnpinnedTabsRect = HasZeroArea(rect) ? Rect.Empty : rect;
            }

            private void ArrangeTab(TabItem tab)
            {
                if (tab.Visibility != Visibility.Collapsed)
                {
                    tab.Arrange(new Rect(_origin, tab.DesiredSize));
                    _origin.X += tab.DesiredSize.Width;
                }
            }

            protected override double RemainingWidth => Math.Max(PanelConstraint.Width - _origin.X, 0.0);
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            var size = Size.Empty;
            MeasureInProgress = true;

            try
            {
                int num = 0;
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



            //Size desideredSize = new Size();
            //foreach (FrameworkElement child in Children)
            //{
            //    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //    desideredSize.Width += child.DesiredSize.Width;

            //    desideredSize.Height = Math.Max(desideredSize.Height, child.DesiredSize.Height);
            //}

            //return new Size(Math.Min(desideredSize.Width, availableSize.Width), desideredSize.Height);
        }

        private static void ShowHideTabs(IList<TabItem> tabs, Size finalSize, int indexOfFirstVisibleElement,
            int indexOfLastVisibleElement, int selectedIndex)
        {
            var offset = 0.0;
            for (var i = 0; i < tabs.Count; i++)
            {
                if (i < indexOfFirstVisibleElement || i > indexOfLastVisibleElement)
                {
                    // have to be hidden, not collapsed. If collapsed the ArrangeOverride function will be called endless!
                    tabs[i].Visibility = Visibility.Hidden;
                    continue;
                }
                if (indexOfFirstVisibleElement != indexOfLastVisibleElement)
                {
                    tabs[i].Visibility = Visibility.Visible;
                    tabs[i].Arrange(new Rect(offset, 0.0, tabs[i].DesiredSize.Width, finalSize.Height));
                    offset += Math.Min(tabs[i].ActualWidth, tabs[i].DesiredSize.Width) + tabs[i].Margin.Left +
                              tabs[i].Margin.Right;
                }
            }

            //Make visible of selected tab as first item if the space is less instead of hiding.            
            var chkHideAllTab = tabs.Any(x => x.Visibility == Visibility.Visible);
            var tab = tabs.FirstOrDefault(x => x.IsSelected);
            if (!chkHideAllTab && tabs.Count >= 1 || indexOfFirstVisibleElement == indexOfLastVisibleElement ||
                tabs[selectedIndex].DesiredSize.Width >= finalSize.Width)
            {
                if (tab != null)
                {
                    tabs[selectedIndex].Arrange(new Rect(0.0, 0.0, tabs[selectedIndex].DesiredSize.Width,
                        finalSize.Height));
                    tabs[selectedIndex].Visibility = Visibility.Visible;
                }
            }
        }

        public abstract class LayoutHelper
        {
            protected readonly DocumentPaneTabPanel Panel;
            protected readonly Size PanelConstraint;
            protected readonly List<TabItem> PinnedTabs;
            protected readonly List<TabItem> UnpinnedTabs;
            protected readonly TabItem PreviewTab;

            protected LayoutHelper(DocumentPaneTabPanel panel, Size panelConstraint)
            {
                Validate.IsNotNull(panel, nameof(panel));
                Panel = panel;
                PanelConstraint = panelConstraint;

                //IObservableCollection<object> observableCollection = panel.DocumentGroup != null ? panel.DocumentGroup.PinnedViews : null;

                //int count = panel.InternalChildren.Count;
                //int capacity1 = observableCollection?.Count ?? 0;
                //int capacity2 = Math.Max(count - capacity1, 0);

                UnpinnedTabs = new List<TabItem>();
                PinnedTabs = new List<TabItem>();


                foreach (TabItem internalChild in panel.InternalChildren)
                {
                    UnpinnedTabs.Add(internalChild);
                }
            }

            protected int RowIndex { get; private set; }

            protected abstract double RemainingWidth { get; }

            protected bool TabOverflowsCurrentRow(TabItem tab)
            {
                return tab.DesiredSize.Width.IsSignificantlyGreaterThan(RemainingWidth);
            }

            protected bool TabFitsOnCurrentRow(TabItem tab)
            {
                return !TabOverflowsCurrentRow(tab);
            }

            protected void StartNewRow()
            {
                RowIndex = RowIndex + 1;
            }

            [Flags]
            protected enum TracedTabItemState
            {
                None = 0,
                Selected = 1,
                Hidden = 2,
                Minimized = 4,
                Pinned = 8,
                StartNewRow = 16,
            }
        }

        private class MeasureHelper : LayoutHelper
        {
            private readonly Size _tabConstraint;
            private int _currentTabIndex;
            private int _tabsInCurrentRow;
            private Size _cumulativePanelSize;
            private Size _currentRowSize;

            public MeasureHelper(DocumentPaneTabPanel panel, Size panelConstraint) 
                : base(panel, panelConstraint)
            {
                _tabConstraint = new Size(double.PositiveInfinity, PanelConstraint.Height);
            }

            protected override double RemainingWidth => Math.Max(PanelConstraint.Width - _currentRowSize.Width, 0.0);

            internal Size Measure()
            {
                //this.MeasurePinnedTabs();
                //this.MeasurePreviewTab();
                MeasureUnpinnedTabs();
                _cumulativePanelSize.Width = PreviewTab == null ? Math.Min(_cumulativePanelSize.Width + 0.0, PanelConstraint.Width) : PanelConstraint.Width;
                return _cumulativePanelSize;
            }

            private void MeasureUnpinnedTabs()
            {
                bool flag = false;
                bool isFirstUnpinnedTab = true;
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
                        AddToCurrentRow(unpinnedTab);
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
                {
                    var w = GetView(selected);

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
                }
                SelectedItemHidden.RaiseEvent(null, new SelectedItemHiddenEventArgs(viewsToMove));
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

            private Size RemainingSize => new Size(RemainingWidth, PanelConstraint.Height);

            private void AddToCurrentRow(UIElement tab)
            {
                _currentRowSize.Width += tab.DesiredSize.Width;
                _currentRowSize.Height = Math.Max(_currentRowSize.Height, tab.DesiredSize.Height);
                _tabsInCurrentRow = _tabsInCurrentRow + 1;
                _cumulativePanelSize.Width = Math.Max(_cumulativePanelSize.Width, _currentRowSize.Width);
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
        public static RoutedEvent PanelLayoutUpdatedEvent = EventManager.RegisterRoutedEvent("PanelLayoutUpdated",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ReorderTabPanel));

        public static readonly DependencyProperty ExpandedTearOffMarginProperty =
            DependencyProperty.Register(nameof(ExpandedTearOffMargin), typeof(Thickness), typeof(ReorderTabPanel),
                new FrameworkPropertyMetadata(new Thickness(0.0)));

        private bool _layoutUpdatedHandlerAdded;

        public Thickness ExpandedTearOffMargin
        {
            get => (Thickness)GetValue(ExpandedTearOffMarginProperty);
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
            DataContextChanged += ReorderTabPanel_DataContextChanged;
        }

        protected static LayoutContent GetView(UIElement child)
        {
            if (child is FrameworkElement frameworkElement)
                return frameworkElement.DataContext as LayoutContent;
            return null;
        }

        private void ReorderTabPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(PanelLayoutUpdatedEvent));
            IsNotificationNeeded = false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(CollectionChangedEventManager)))
                return false;
            OnDependentCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
            return true;
        }

        protected virtual void OnDependentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateMeasure();
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
            public LayoutContent View { get; }

            public int NewIndex { get; }

            public ViewIndexChange(LayoutContent view, int newIndex)
            {
                View = view;
                NewIndex = newIndex;
            }
        }
    }
}