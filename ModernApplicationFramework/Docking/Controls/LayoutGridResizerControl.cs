/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutGridResizerControl : Thumb
    {
        public static readonly DependencyProperty BackgroundWhileDraggingProperty =
            DependencyProperty.Register("BackgroundWhileDragging", typeof (Brush), typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty OpacityWhileDraggingProperty =
            DependencyProperty.Register("OpacityWhileDragging", typeof (double), typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(0.5));

        static LayoutGridResizerControl()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(typeof (LayoutGridResizerControl)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            VerticalAlignmentProperty.OverrideMetadata(typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(VerticalAlignment.Stretch,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            BackgroundProperty.OverrideMetadata(typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof (LayoutGridResizerControl),
                new FrameworkPropertyMetadata(true, null));
        }

        public Brush BackgroundWhileDragging
        {
            get { return (Brush) GetValue(BackgroundWhileDraggingProperty); }
            set { SetValue(BackgroundWhileDraggingProperty, value); }
        }

        public double OpacityWhileDragging
        {
            get { return (double) GetValue(OpacityWhileDraggingProperty); }
            set { SetValue(OpacityWhileDraggingProperty, value); }
        }
    }
}