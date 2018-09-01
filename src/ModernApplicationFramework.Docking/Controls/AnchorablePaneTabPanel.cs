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
using System.Windows;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorablePaneTabPanel : ReorderTabPanel
    {
        private static readonly DependencyPropertyKey IsFirstPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(bool), typeof(AnchorablePaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly DependencyPropertyKey IsLastPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(bool), typeof(AnchorablePaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly DependencyPropertyKey IsImmediatelyBeforeSelectionPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsImmediatelyBeforeSelection", typeof(bool),
                typeof(AnchorablePaneTabPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly DependencyPropertyKey IsImmediatelyAfterSelectionPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsImmediatelyAfterSelection", typeof(bool),
                typeof(AnchorablePaneTabPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse));

        private static readonly DependencyPropertyKey IsTruncatingTabsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsTruncatingTabs", typeof(bool), typeof(AnchorablePaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse,
                    FrameworkPropertyMetadataOptions.Inherits));

        private static readonly DependencyPropertyKey UseCompressedTabStylePropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("UseCompressedTabStyle", typeof(bool),
                typeof(AnchorablePaneTabPanel),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsLastProperty = IsLastPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsImmediatelyBeforeSelectionProperty = IsImmediatelyBeforeSelectionPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsImmediatelyAfterSelectionProperty = IsImmediatelyAfterSelectionPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsTruncatingTabsProperty = IsTruncatingTabsPropertyKey.DependencyProperty;
        public static readonly DependencyProperty UseCompressedTabStyleProperty = UseCompressedTabStylePropertyKey.DependencyProperty;

        private static readonly DependencyProperty CalculatedTabSizeProperty =
            DependencyProperty.RegisterAttached("CalculatedTabSize", typeof(Size), typeof(FrameworkPropertyMetadata));

        public AnchorablePaneTabPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateIndices();
            var num = 0.0;
            var undockingOffset = UndockingOffset;
            var values = new List<double>(InternalChildren.Count);
            var availableSize1 = new Size(double.PositiveInfinity, availableSize.Height);
            foreach (UIElement internalChild in InternalChildren)
            {
                if (UndockingLength != 0.0)
                    availableSize1.Width = UndockingLength;
                internalChild.Measure(availableSize1);
                var desiredSize = internalChild.DesiredSize;
                SetCalculatedTabSize(internalChild, desiredSize);
                num = Math.Max(num, desiredSize.Height);
                values.Add(desiredSize.Width);
                undockingOffset += desiredSize.Width;
            }
            var height = Math.Min(availableSize.Height, num);
            if (undockingOffset > availableSize.Width)
            {
                SetIsTruncatingTabs(this, true);
                CalculateTruncationThreshold(values, undockingOffset - availableSize.Width, out var truncationThreshold, out var truncatedValue);
                SetUseCompressedTabStyle(this, truncatedValue <= 40.0);
                foreach (UIElement internalChild in InternalChildren)
                {
                    var desiredSize = internalChild.DesiredSize;
                    if (desiredSize.Width >= truncationThreshold)
                    {
                        var size = new Size(Math.Max(GetMinimumWidth(internalChild), truncatedValue), desiredSize.Height);
                        SetCalculatedTabSize(internalChild, size);
                        internalChild.Measure(size);
                    }
                }
                return new Size(availableSize.Width, height);
            }
            SetIsTruncatingTabs(this, false);
            SetUseCompressedTabStyle(this, false);
            return new Size(undockingOffset, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double undockingOffset = UndockingOffset;
            foreach (UIElement internalChild in InternalChildren)
            {
                Size calculatedTabSize = GetCalculatedTabSize(internalChild);
                internalChild.Arrange(new Rect(undockingOffset, 0.0, calculatedTabSize.Width, calculatedTabSize.Height));
                undockingOffset += calculatedTabSize.Width;
            }
            return finalSize;
        }

        private void UpdateIndices()
        {
            var count = InternalChildren.Count;
            var num = count - 1;
            var selectedIndex = GetSelectedIndex();
            for (var index = 0; index < count; ++index)
            {
                var internalChild = InternalChildren[index];
                if (internalChild != null)
                {
                    SetIsFirst(internalChild, index == 0);
                    SetIsLast(internalChild, index == num);
                    SetIsImmediatelyBeforeSelection(internalChild, selectedIndex >= 0 && index == selectedIndex - 1);
                    SetIsImmediatelyAfterSelection(internalChild, selectedIndex >= 0 && index == selectedIndex + 1);
                }
            }
        }

        private int GetSelectedIndex()
        {
            for (var index = 0; index < InternalChildren.Count; ++index)
            {
                var internalChild = InternalChildren[index];
                if (internalChild != null && Selector.GetIsSelected(internalChild))
                    return index;
            }
            return -1;
        }

        private static void CalculateTruncationThreshold(List<double> values, double sizeToRemove, out double truncationThreshold, out double truncatedValue)
        {
            values.Sort();
            for (var index = 1; index < values.Count; ++index)
            {
                var num1 = values[values.Count - index];
                var num2 = values[values.Count - index - 1];
                var num3 = num1 - num2;
                if (sizeToRemove - num3 * index < 0.0)
                {
                    truncationThreshold = num1;
                    truncatedValue = num1 - sizeToRemove / index;
                    return;
                }
                sizeToRemove -= num3 * index;
            }
            truncationThreshold = values[0];
            truncatedValue = truncationThreshold - sizeToRemove / values.Count;
        }

        private static double GetMinimumWidth(UIElement child)
        {
            var num = 0.0;
            if (child is FrameworkElement frameworkElement)
                num = frameworkElement.MinWidth;
            return num;
        }


        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);


        //    double finalWidth = finalSize.Width;
        //    var uiElements = visibleChildren as UIElement[] ?? visibleChildren.ToArray();
        //    double desideredWidth = uiElements.Sum(ch => ch.DesiredSize.Width);
        //    double offsetX = 0.0;

        //    if (finalWidth > desideredWidth)
        //    {
        //        foreach (var uiElement in uiElements)
        //        {
        //            var child = (FrameworkElement) uiElement;
        //            double childFinalWidth = child.DesiredSize.Width;
        //            child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

        //            offsetX += childFinalWidth;
        //        }
        //    }
        //    else
        //    {
        //        double childFinalWidth = finalWidth/uiElements.Count();
        //        foreach (var uiElement in uiElements)
        //        {
        //            var child = (FrameworkElement) uiElement;
        //            child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

        //            offsetX += childFinalWidth;
        //        }
        //    }

        //    return finalSize;
        //}

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    double totWidth = 0;
        //    double maxHeight = 0;
        //    var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);
        //    var uiElements = visibleChildren as UIElement[] ?? visibleChildren.ToArray();
        //    foreach (var child in uiElements.Cast<FrameworkElement>())
        //    {
        //        child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
        //        totWidth += child.DesiredSize.Width;
        //        maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        //    }

        //    if (!(totWidth > availableSize.Width))
        //        return new Size(Math.Min(availableSize.Width, totWidth), maxHeight);
        //    double childFinalDesideredWidth = availableSize.Width/uiElements.Count();
        //    foreach (var child in uiElements.Cast<FrameworkElement>())
        //    {
        //        child.Measure(new Size(childFinalDesideredWidth, availableSize.Height));
        //    }

        //    return new Size(Math.Min(availableSize.Width, totWidth), maxHeight);
        //}

        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed &&
        //        LayoutAnchorableTabItem.IsDraggingItem())
        //    {
        //        var contentModel = LayoutAnchorableTabItem.GetDraggingItem().Model as LayoutAnchorable;
        //        if (contentModel != null)
        //        {
        //            var manager = contentModel.Root.Manager;
        //            LayoutAnchorableTabItem.ResetDraggingItem();

        //            manager.StartDraggingFloatingWindowForContent(contentModel);
        //        }
        //    }

        //    base.OnMouseLeave(e);
        //}

        private static void SetCalculatedTabSize(DependencyObject element, Size size)
        {
            element.SetValue(CalculatedTabSizeProperty, size);
        }

        private static void SetIsFirst(DependencyObject element, bool value)
        {
            element.SetValue(IsFirstPropertyKey, Boxes.Box(value));
        }

        private static void SetIsLast(DependencyObject element, bool value)
        {
            element.SetValue(IsLastPropertyKey, Boxes.Box(value));
        }

        private static void SetIsImmediatelyBeforeSelection(DependencyObject element, bool value)
        {
            element.SetValue(IsImmediatelyBeforeSelectionPropertyKey, Boxes.Box(value));
        }

        private static void SetIsImmediatelyAfterSelection(DependencyObject element, bool value)
        {
            element.SetValue(IsImmediatelyAfterSelectionPropertyKey, Boxes.Box(value));
        }

        private static void SetIsTruncatingTabs(DependencyObject element, bool value)
        {
            element.SetValue(IsTruncatingTabsPropertyKey, Boxes.Box(value));
        }

        private static void SetUseCompressedTabStyle(DependencyObject element, bool value)
        {
            element.SetValue(UseCompressedTabStylePropertyKey, Boxes.Box(value));
        }

        private static Size GetCalculatedTabSize(DependencyObject element)
        {
            return (Size)element.GetValue(CalculatedTabSizeProperty);
        }
    }
}