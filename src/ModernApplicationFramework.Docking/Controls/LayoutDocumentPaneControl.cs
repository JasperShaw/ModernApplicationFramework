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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutDocumentPaneControl : GroupControl, ILayoutControl //, ILogicalChildrenContainer
    {
        private static readonly DependencyPropertyKey AccessOrderPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("AccessOrder", typeof(uint), typeof(LayoutDocumentPaneControl),
                new PropertyMetadata(Boxes.UInt32Zero));

        private static ResourceKey _tabItemStyleKey;
        private uint _lastAccessOrder = 1;

        public readonly List<object> _logicalChildren = new List<object>();
        private readonly LayoutDocumentPane _model;

        public static object ItemTemplateKey { get; set; } = "DocumentGroupControlItemTemplate";

        public static ResourceKey TabItemStyleKey => _tabItemStyleKey ?? (_tabItemStyleKey = new StyleKey<LayoutDocumentPaneControl>());


        internal LayoutDocumentPaneControl(LayoutDocumentPane model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            SetBinding(ItemsSourceProperty, new Binding("Model.Children") {Source = this});
            SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection") {Source = this});

            LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        static LayoutDocumentPaneControl()
        {
            FocusableProperty.OverrideMetadata(typeof (LayoutDocumentPaneControl), new FrameworkPropertyMetadata(false));
        }

        public ILayoutElement Model => _model;

        protected override System.Collections.IEnumerator LogicalChildren => _logicalChildren.GetEnumerator();

        public void TouchAccessOrder(TabItem tab)
        {
            SetAccessOrder(tab, ++_lastAccessOrder);
        }

        //protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        //{
        //    base.OnGotKeyboardFocus(e);
        //    System.Diagnostics.Trace.WriteLine($"OnGotKeyboardFocus({e.Source}, {e.NewFocus})");


        //    //if (_model.SelectedContent != null)
        //    //    _model.SelectedContent.IsActive = true;
        //}

        //protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);

        //    if (!e.Handled && _model.SelectedContent != null)
        //        _model.SelectedContent.IsActive = true;
        //}

        //protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseRightButtonDown(e);

        //    if (!e.Handled && _model.SelectedContent != null)
        //        _model.SelectedContent.IsActive = true;
        //}

        //protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        //{
        //    base.OnSelectionChanged(e);

        //    if (_model.SelectedContent != null)
        //        _model.SelectedContent.IsActive = true;
        //}

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var modelWithActualSize = _model as ILayoutPositionableElementWithActualSize;
            modelWithActualSize.ActualWidth = ActualWidth;
            modelWithActualSize.ActualHeight = ActualHeight;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var tab = (TabItem)element;
            tab.SetResourceReference(StyleProperty, TabItemStyleKey);
            SetAccessOrder(tab, _lastAccessOrder - 1U);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DocumentTabItem();
        }


        private static void SetAccessOrder(TabItem tab, uint value)
        {
            Validate.IsNotNull(tab, nameof(tab));
            tab.SetValue(AccessOrderPropertyKey, value);
        }
    }

    public class DocumentTabItem : GroupControlTabItem
    {
        public static readonly DependencyProperty TabStateProperty = DependencyProperty.Register(nameof(TabState), typeof(TabState), typeof(DocumentTabItem), new FrameworkPropertyMetadata(TabState.Normal, null, CoerceTabState));
        public static readonly DependencyProperty EffectiveTabStateProperty = DependencyProperty.RegisterAttached("EffectiveTabState", typeof(TabState), typeof(DocumentTabItem), new FrameworkPropertyMetadata(TabState.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.RegisterAttached("RowIndex", typeof(int), typeof(DocumentTabItem), new FrameworkPropertyMetadata(Boxes.Int32Zero, OnRowIndexChanged));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(DocumentTabItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        //public static readonly DependencyProperty IsPreviewTabProperty = DependencyProperty.Register(nameof(IsPreviewTab), typeof(bool), typeof(DocumentTabItem), (PropertyMetadata)new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(DocumentTabItem.OnIsPreviewTabChanged)));
        public static readonly DependencyProperty IsTabVisibleProperty = DependencyProperty.RegisterAttached("IsTabVisible", typeof(bool), typeof(DocumentTabItem), new PropertyMetadata(Boxes.BooleanFalse));


        public static event EventHandler<RowIndexChangedEventArgs> RowIndexChanged;

        public TabState TabState
        {
            get => (TabState)GetValue(TabStateProperty);
            set => SetValue(TabStateProperty, value);
        }

        public bool IsActive => (bool)GetValue(IsActiveProperty);

        public LayoutContent View => DataContext as LayoutContent;


        public DocumentTabItem()
        {
            DataContextChanged += OnDataContextChanged;
        }

        public static TabState GetEffectiveTabState(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (TabState)element.GetValue(EffectiveTabStateProperty);
        }

        public static int GetRowIndex(LayoutContent view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return (int)view.GetValue(RowIndexProperty);
        }

        public static void SetRowIndex(LayoutContent view, int row)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            view.SetValue(RowIndexProperty, row);
        }

        public static bool GetIsTabVisible(LayoutContent view)
        {
            Validate.IsNotNull(view, nameof(view));
            return (bool)view.GetValue(IsTabVisibleProperty);
        }

        public static void SetIsTabVisible(LayoutContent view, bool value)
        {
            Validate.IsNotNull(view, nameof(view));
            view.SetValue(IsTabVisibleProperty, Boxes.Box(value));
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            (ItemsControl.ItemsControlFromItemContainer(this) as LayoutDocumentPaneControl)?.TouchAccessOrder((TabItem)this);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (DataContext is LayoutContent dataContext && (dataContext.IsPinned /*|| DocumentGroup.GetIsPreviewView(dataContext)*/))
                dataContext.IsSelected = true;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindingBase binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(TabStateProperty),
                Mode = BindingMode.OneWay
            };
            if (e.NewValue is LayoutContent view)
            {
                binding = new MultiBinding
                {
                    Converter = EffectiveTabStateConverter.Instance,
                    Bindings =
                    {
                        binding,
                        new Binding
                        {
                            Source = view,
                            Path = new PropertyPath(nameof(LayoutContent.IsSelected)),
                            Mode = BindingMode.OneWay
                        }
                    }
                };
                BindingOperations.SetBinding(this, IsActiveProperty, new Binding
                {
                    Source = view,
                    Path = new PropertyPath(nameof(View.IsActive)),
                    Mode = BindingMode.OneWay
                });
                BindingOperations.SetBinding(this, VisibilityProperty, new Binding
                {
                    Source = view,
                    Path = new PropertyPath(IsTabVisibleProperty),
                    Mode = BindingMode.OneWayToSource,
                    Converter = new BooleanToVisibilityConverter()
                });
            }
            else
            {
                BindingOperations.ClearBinding(this, IsActiveProperty);
                BindingOperations.ClearBinding(this, VisibilityProperty);
            }
            BindingOperations.SetBinding(this, EffectiveTabStateProperty, binding);
        }

        private static object CoerceTabState(DependencyObject d, object value)
        {
            return CoerceTabState((TabState)value);
        }

        private static object CoerceTabState(TabState value)
        {
            return TabState.Normal;
        }

        private static void OnRowIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var view = (LayoutContent)obj;
            RowIndexChanged.RaiseEvent(null, new RowIndexChangedEventArgs(view));
        }

        private class EffectiveTabStateConverter : MultiValueConverter<TabState, bool, TabState>
        {
            private static EffectiveTabStateConverter _instance;

            public static EffectiveTabStateConverter Instance => _instance ?? (_instance = new EffectiveTabStateConverter());

            protected override TabState Convert(TabState tabState, bool selected, object parameter, CultureInfo culture)
            {
                return selected ? TabState.Normal : tabState;
            }
        }

    }

    public class RowIndexChangedEventArgs : EventArgs
    {
        public RowIndexChangedEventArgs(LayoutContent view)
        {
            View = view;
        }

        public LayoutContent View { get; }
    }

    public enum TabState
    {
        Normal,
        Minimized,
    }
}