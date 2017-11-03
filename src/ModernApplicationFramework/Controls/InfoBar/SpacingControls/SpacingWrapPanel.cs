using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.SpacingControls
{
    public class SpacingWrapPanel : WrapPanel
    {
        private static readonly DependencyProperty HorizontalItemSpacingProperty = DependencyProperty.Register(nameof(HorizontalItemSpacing), typeof(double), typeof(SpacingWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), ValidateWidthHeight);
        private static readonly DependencyProperty VerticalItemSpacingProperty = DependencyProperty.Register(nameof(VerticalItemSpacing), typeof(double), typeof(SpacingWrapPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange), ValidateWidthHeight);
        private readonly MeasureInfo _measureInfo;

        public SpacingWrapPanel()
        {
            _measureInfo = new MeasureInfo(this);
        }

        public double HorizontalItemSpacing
        {
            get => (double)GetValue(HorizontalItemSpacingProperty);
            set => SetValue(HorizontalItemSpacingProperty, value);
        }

        private static bool ValidateWidthHeight(object value)
        {
            double d = (double)value;
            if (d < 0.0 || double.IsPositiveInfinity(d))
                return false;
            double.IsNaN(d);
            return true;
        }

        public double VerticalItemSpacing
        {
            get => (double)GetValue(VerticalItemSpacingProperty);
            set => SetValue(VerticalItemSpacingProperty, value);
        }

        private AbstractSize AbstractItemSpacing => MakeAbstractSize(HorizontalItemSpacing, VerticalItemSpacing);

        protected override Size MeasureOverride(Size constraint)
        {
            _measureInfo.Reset();
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool flag1 = !double.IsNaN(itemWidth);
            bool flag2 = !double.IsNaN(itemHeight);
            Size availableSize = new Size(flag1 ? itemWidth : constraint.Width, flag2 ? itemHeight : constraint.Height);
            AbstractSize abstractSize = MakeAbstractSize(constraint);
            RowInfo row = new RowInfo(this, abstractSize.AbstractWidth);
            foreach (UIElement nonCollapsedChild in NonCollapsedChildren)
            {
                nonCollapsedChild.Measure(availableSize);
                AbstractSize childSize = MakeAbstractSize(flag1 ? itemWidth : nonCollapsedChild.DesiredSize.Width, flag2 ? itemHeight : nonCollapsedChild.DesiredSize.Height);
                if (!row.AddChild(childSize))
                {
                    _measureInfo.AddRow(row);
                    row = new RowInfo(this, abstractSize.AbstractWidth);
                    row.AddChild(childSize);
                }
            }
            _measureInfo.AddRow(row);
            return _measureInfo.AbstractSize.RealSize;
        }

        protected override Size ArrangeOverride(Size constraint)
        {
            UIElement[] array = NonCollapsedChildren.ToArray();
            AbstractPoint abstractPoint = MakeAbstractPoint();
            AbstractSize abstractSize1 = MakeAbstractSize(constraint);
            AbstractSize abstractItemSpacing = AbstractItemSpacing;
            ComputeItemSpacing(_measureInfo.RowCount, abstractSize1.AbstractHeight, _measureInfo.AbstractSize.AbstractHeight, abstractItemSpacing.AbstractHeight, out var itemSpacing1, out var remainder1);
            int index1 = 0;
            for (int index2 = 0; index2 < _measureInfo.RowCount; ++index2)
            {
                RowInfo row = _measureInfo.Rows[index2];
                int childCount = row.ChildCount;
                double abstractWidth1 = abstractSize1.AbstractWidth;
                AbstractSize abstractSize2 = row.AbstractSize;
                double abstractWidth2 = abstractSize2.AbstractWidth;
                double abstractWidth3 = abstractItemSpacing.AbstractWidth;
                ComputeItemSpacing(childCount, abstractWidth1, abstractWidth2, abstractWidth3, out var itemSpacing2, out var remainder2);
                int index3 = 0;
                double num = itemSpacing2 + ((double)index3 < remainder2 ? 1.0 : 0.0);
                while (index3 < row.ChildCount)
                {
                    UIElement uiElement = array[index1];
                    AbstractSize childSize = row.ChildSizes[index3];
                    Rect finalRect = new Rect(abstractPoint.RealPoint, childSize.RealSize);
                    uiElement.Arrange(finalRect);
                    abstractPoint.AbstractX += childSize.AbstractWidth + num;
                    ++index3;
                    ++index1;
                }
                double num1 = itemSpacing1 + (index2 < remainder1 ? 1.0 : 0.0);
                abstractPoint.AbstractX = 0.0;
                var local = abstractPoint;
                double abstractY = local.AbstractY;
                abstractSize2 = row.AbstractSize;
                double num2 = abstractSize2.AbstractHeight + num1;
                double num3 = abstractY + num2;
                local.AbstractY = num3;
            }
            return constraint;
        }

        private void ComputeItemSpacing(int itemCount, double totalSpace, double occupiedSpace, double nominalItemSpacing, out double itemSpacing, out double remainder)
        {
            bool flag = !double.IsNaN(nominalItemSpacing);
            itemSpacing = flag ? nominalItemSpacing : 0.0;
            remainder = 0.0;
            if (itemCount <= 1 || flag)
                return;
            double num = totalSpace - occupiedSpace;
            itemSpacing = num / (itemCount - 1);
            if (!UseLayoutRounding)
                return;
            itemSpacing = (int)itemSpacing;
            remainder = (int)(num % (itemCount - 1));
        }

        private IEnumerable<UIElement> NonCollapsedChildren
        {
            get
            {
                return InternalChildren.Cast<UIElement>().Where(e =>
                {
                    if (e != null)
                        return e.Visibility != Visibility.Collapsed;
                    return false;
                });
            }
        }

        private AbstractSize MakeAbstractSize()
        {
            return new AbstractSize(Orientation.Horizontal, Orientation);
        }

        private AbstractSize MakeAbstractSize(Size size)
        {
            return new AbstractSize(Orientation.Horizontal, Orientation, size);
        }

        private AbstractSize MakeAbstractSize(double width, double height)
        {
            return new AbstractSize(Orientation.Horizontal, Orientation, width, height);
        }

        private AbstractPoint MakeAbstractPoint()
        {
            return new AbstractPoint(Orientation.Horizontal, Orientation);
        }

        private class MeasureInfo
        {
            private readonly List<RowInfo> _rows = new List<RowInfo>();
            private readonly SpacingWrapPanel _panel;
            private AbstractSize _size;
            private double _rowSpacing;

            public MeasureInfo(SpacingWrapPanel panel)
            {
                _panel = panel;
            }

            public List<RowInfo> Rows => _rows;

            public int RowCount => _rows.Count;

            public AbstractSize AbstractSize => _size;

            public void Reset()
            {
                AbstractSize abstractItemSpacing = _panel.AbstractItemSpacing;
                _size = _panel.MakeAbstractSize();
                _rowSpacing = double.IsNaN(abstractItemSpacing.AbstractHeight) ? 0.0 : abstractItemSpacing.AbstractHeight;
                _rows.Clear();
            }

            public void AddRow(RowInfo row)
            {
                AbstractSize abstractSize = row.AbstractSize;
                double num1 = _size.AbstractHeight + (abstractSize.AbstractHeight + (_rows.Count > 0 ? _rowSpacing : 0.0));
                _rows.Add(row);
                var local = _size;
                double abstractWidth1 = _size.AbstractWidth;
                abstractSize = row.AbstractSize;
                double abstractWidth2 = abstractSize.AbstractWidth;
                double num2 = Math.Max(abstractWidth1, abstractWidth2);
                local.AbstractWidth = num2;
                _size.AbstractHeight = num1;
            }
        }

        private class RowInfo
        {
            private readonly List<AbstractSize> _childSizes = new List<AbstractSize>();
            private readonly double _itemSpacing;
            private readonly double _maxWidth;
            private AbstractSize _size;

            public RowInfo(SpacingWrapPanel panel, double maxWidth)
            {
                AbstractSize abstractItemSpacing = panel.AbstractItemSpacing;
                _size = panel.MakeAbstractSize();
                _itemSpacing = double.IsNaN(abstractItemSpacing.AbstractWidth) ? 0.0 : abstractItemSpacing.AbstractWidth;
                _maxWidth = maxWidth;
            }

            public int ChildCount => _childSizes.Count;

            public List<AbstractSize> ChildSizes => _childSizes;

            public AbstractSize AbstractSize => _size;

            public bool AddChild(AbstractSize childSize)
            {
                double num = _size.AbstractWidth + (childSize.AbstractWidth + (ChildCount > 0 ? _itemSpacing : 0.0));
                if (ChildCount > 0 && num.GreaterThan(_maxWidth))
                    return false;
                _childSizes.Add(childSize);
                _size.AbstractWidth = num;
                _size.AbstractHeight = Math.Max(_size.AbstractHeight, childSize.AbstractHeight);
                return true;
            }
        }
    }
}
