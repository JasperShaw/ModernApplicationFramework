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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutDocumentTabItem : Control
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof (LayoutContent), typeof (LayoutDocumentTabItem),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof (LayoutItem), typeof (LayoutDocumentTabItem),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;


        private bool _isMouseDown;
        private Point _mouseDownPoint;
        private List<TabItem> _otherTabs;
        private List<Rect> _otherTabsScreenArea;
        private DocumentPaneTabPanel _parentDocumentTabPanel;
        private Rect _parentDocumentTabPanelScreenArea;

        static LayoutDocumentTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentTabItem),
                new FrameworkPropertyMetadata(typeof (LayoutDocumentTabItem)));
        }

        public LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

        public LayoutContent Model
        {
            get => (LayoutContent) GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
            //UpdateLogicalParent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (LayoutItem.CloseCommand.CanExecute(null))
                    LayoutItem.CloseCommand.Execute(null);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _isMouseDown = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseDown = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Model.IsActive = true;

            if (e.ClickCount != 1)
                return;
            _mouseDownPoint = this.PointToScreenDpi(e.GetPosition(this));
            _isMouseDown = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            _isMouseDown = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown)
            {
                Point ptMouseMove = this.PointToScreenDpi(e.GetPosition(this));

                if (Math.Abs(ptMouseMove.X - _mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(ptMouseMove.Y - _mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    UpdateDragDetails();
                    CaptureMouse();
                    _isMouseDown = false;
                }
            }

            if (!IsMouseCaptured)
                return;
            var mousePosInScreenCoord = this.PointToScreenDpi(e.GetPosition(this));
            if (!_parentDocumentTabPanelScreenArea.Contains(mousePosInScreenCoord))
            {
                ReleaseMouseCapture();
                var manager = Model.Root.Manager;
                manager.StartDraggingFloatingWindowForContent(Model);
            }
            else
            {
                int indexOfTabItemWithMouseOver = _otherTabsScreenArea.FindIndex(r => r.Contains(mousePosInScreenCoord));
                if (indexOfTabItemWithMouseOver < 0)
                    return;
                var targetModel = _otherTabs[indexOfTabItemWithMouseOver].Content as LayoutContent;
                var container = Model.Parent;
                var containerPane = Model.Parent as ILayoutPane;
                var childrenList = container.Children.ToList();
                containerPane?.MoveChild(childrenList.IndexOf(Model), childrenList.IndexOf(targetModel));
                Model.IsActive = true;
                _parentDocumentTabPanel.UpdateLayout();
                UpdateDragDetails();
            }
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentTabItem) d).OnModelChanged(e);
        }

        private void UpdateDragDetails()
        {
            _parentDocumentTabPanel = this.FindLogicalAncestor<DocumentPaneTabPanel>();
            _parentDocumentTabPanelScreenArea = _parentDocumentTabPanel.GetScreenArea();
            _otherTabs = _parentDocumentTabPanel.Children.Cast<TabItem>().Where(ch =>
                ch.Visibility != Visibility.Collapsed).ToList();
            Rect currentTabScreenArea = this.FindLogicalAncestor<TabItem>().GetScreenArea();
            _otherTabsScreenArea = _otherTabs.Select(ti =>
            {
                var screenArea = ti.GetScreenArea();
                return new Rect(screenArea.Left, screenArea.Top, currentTabScreenArea.Width, screenArea.Height);
            }).ToList();
        }
    }
}