using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking
{
    internal class DraggedTabInfo
    {
        private Rect _tabStripRect;
        private Thickness _expandTabStripMargin = new Thickness(0.0);
        private Rect _virtualTabRect;

        public ReorderTabPanel TabStrip { get; set; }

        public LayoutContent DraggedViewElement { get; set; }

        public List<Rect> TabRects { get; } = new List<Rect>();

        public Rect TabStripRect => _tabStripRect;

        public int DraggedTabPosition { get; set; }

        public Rect VirtualTabRect => _virtualTabRect;

        public bool HasBeenReordered { get; set; }


        static DraggedTabInfo()
        {
            EventManager.RegisterClassHandler(typeof(ReorderTabPanel), ReorderTabPanel.PanelLayoutUpdatedEvent,
                new RoutedEventHandler(OnPanelLayoutUpdated));
        }

        public void ClearVirtualTabRect()
        {
            _virtualTabRect.X = 0.0;
            _virtualTabRect.Y = 0.0;
            _virtualTabRect.Width = 0.0;
            _virtualTabRect.Height = 0.0;
        }

        public void ExpandTabStrip()
        {
            _expandTabStripMargin = TabStrip.ExpandedTearOffMargin;
            ExpandTabStripCore();
        }

        public void SetVirtualTabRect(int position)
        {
            if (position < 0 || position >= TabRects.Count)
                throw new ArgumentOutOfRangeException("position: " + position + " tabRects.count: " + TabRects.Count);
            _virtualTabRect = TabRects[position];
        }


        public Rect MeasureTabStrip()
        {
            if (TabStrip == null)
                throw new InvalidOperationException("TabStrip must be initialized.");
            DraggedTabPosition = -1;
            var topLeft = new Point(double.MaxValue, double.MaxValue);
            var bottomRight = new Point(double.MinValue, double.MinValue);
            TabRects.Clear();
            foreach (UIElement child in TabStrip.Children)
            {
                if (child is TabItem tabChild)
                    MeasureTabItem(tabChild, ref topLeft, ref bottomRight);
            }
            if (topLeft.X == double.MaxValue || topLeft.Y == double.MaxValue || (bottomRight.X == double.MinValue || bottomRight.Y == double.MinValue))
            {
                topLeft.X = 0.0;
                topLeft.Y = 0.0;
                bottomRight.X = 0.0;
                bottomRight.Y = 0.0;
                TabRects.Clear();
            }
            //DockTarget ancestor = TabStrip.FindAncestor<DockTarget>();
            //if (ancestor != null && ancestor.DockTargetType == DockTargetType.Auto && ancestor.IsConnectedToPresentationSource())
            //{
            //    tabStripRect = new Rect(ancestor.PointToScreen(new Point(0.0, 0.0)), new Size(ancestor.ActualWidth, ancestor.ActualHeight).LogicalToDeviceUnits());
            //}
            else
            {
                if (TabStrip is DocumentPaneTabPanel tabStrip && tabStrip.IsConnectedToPresentationSource())
                {
                    Rect rect = DraggedViewElement.IsPinned ? tabStrip.PinnedTabsRect : tabStrip.UnpinnedTabsRect;
                    topLeft = tabStrip.PointToScreen(rect.TopLeft);
                    bottomRight = tabStrip.PointToScreen(rect.BottomRight);
                }
                _tabStripRect = new Rect(topLeft, bottomRight);
            }
            ExpandTabStripCore();
            return _tabStripRect;
        }

        private void MeasureTabItem(TabItem tabChild, ref Point topLeft, ref Point bottomRight)
        {
            if (tabChild.Visibility != Visibility.Visible || !tabChild.IsConnectedToPresentationSource())
                return;
            var screen = tabChild.PointToScreen(new Point(0.0, 0.0));
            var deviceUnits = new Size(tabChild.ActualWidth, tabChild.ActualHeight).LogicalToDeviceUnits();
            var rect = new Rect(screen, deviceUnits);
            topLeft.X = Math.Min(topLeft.X, screen.X);
            topLeft.Y = Math.Min(topLeft.Y, screen.Y);
            bottomRight.X = Math.Max(bottomRight.X, screen.X + deviceUnits.Width);
            bottomRight.Y = Math.Max(bottomRight.Y, screen.Y + deviceUnits.Height);
            int index;
            for (index = 0; index < TabRects.Count; ++index)
            {
                var tabRect = TabRects[index];
                var num = (tabRect.Y + tabRect.Bottom) / 2.0;
                if (num > rect.Bottom || num < rect.Bottom && num > rect.Top && rect.X < tabRect.X)
                    break;
            }
            TabRects.Insert(index, rect);
            if (tabChild.DataContext == DraggedViewElement)
            {
                DraggedTabPosition = index;
            }
            else
            {
                if (DraggedTabPosition < index)
                    return;
                ++DraggedTabPosition;
            }
        }

        private void ExpandTabStripCore()
        {
            _tabStripRect = _tabStripRect.Resize(new Vector(-_expandTabStripMargin.Left, -_expandTabStripMargin.Top), new Vector(_expandTabStripMargin.Left + _expandTabStripMargin.Right, _expandTabStripMargin.Top + _expandTabStripMargin.Bottom), new Size(0.0, 0.0), new Size(double.PositiveInfinity, double.PositiveInfinity));
            NormalizeTabHeight();
        }

        private void NormalizeTabHeight()
        {
            var val1_1 = 0.0;
            var val1_2 = 0.0;
            var num1 = double.MinValue;
            var flag1 = false;
            for (var index1 = 0; index1 < TabRects.Count; ++index1)
            {
                var tabRect1 = TabRects[index1];
                if (num1 <= tabRect1.Top || num1 >= tabRect1.Bottom)
                {
                    num1 = (tabRect1.Top + tabRect1.Bottom) / 2.0;
                    val1_1 = double.MinValue;
                    val1_2 = double.MaxValue;
                    var flag2 = index1 == 0;
                    for (var index2 = index1; index2 < TabRects.Count; ++index2)
                    {
                        var tabRect2 = TabRects[index2];
                        if (num1 > tabRect2.Top && num1 < tabRect2.Bottom)
                        {
                            val1_1 = Math.Max(val1_1, tabRect2.Height);
                            val1_2 = Math.Min(val1_2, tabRect2.Y);
                            flag1 = index2 == TabRects.Count - 1;
                        }
                        else
                            break;
                    }
                    if (flag2)
                    {
                        val1_1 += val1_2 - _tabStripRect.Y;
                        val1_2 = _tabStripRect.Y;
                    }
                    if (flag1)
                        val1_1 += _tabStripRect.Bottom - (val1_2 + val1_1);
                    if (!_virtualTabRect.IsEmpty)
                    {
                        var num2 = (_virtualTabRect.Top + _virtualTabRect.Bottom) / 2.0;
                        if (num2 > tabRect1.Top && num2 < tabRect1.Bottom)
                        {
                            _virtualTabRect.Y = val1_2;
                            _virtualTabRect.Height = Math.Max(val1_1, 0.0);
                        }
                    }
                }
                tabRect1.Height = Math.Max(val1_1, 0.0);
                tabRect1.Y = val1_2;
                TabRects[index1] = tabRect1;
            }
        }


        private static void OnPanelLayoutUpdated(object sender, RoutedEventArgs e)
        {
            var draggedTabInfo = DockingManager.Instance.DraggedTabInfo;
            if (draggedTabInfo == null || draggedTabInfo.TabStrip != sender)
                return;
            draggedTabInfo.MeasureTabStrip();
        }

        public int GetTabIndexAt(Point screenPoint)
        {
            int num = 0;
            foreach (var tabRect in TabRects)
            {
                if (tabRect.Contains(screenPoint))
                    return num;
                ++num;
            }
            return -1;
        }
    }
}
