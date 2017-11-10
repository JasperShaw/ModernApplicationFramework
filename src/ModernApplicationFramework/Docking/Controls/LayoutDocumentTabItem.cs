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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutDocumentTabItem : DragUndockHeader
    {
        private bool _isMouseDown;
        private Point _mouseDownPoint;
        private List<TabItem> _otherTabs;
        private List<Rect> _otherTabsScreenArea;
        private DocumentPaneTabPanel _parentDocumentTabPanel;
        private Rect _parentDocumentTabPanelScreenArea;

        static LayoutDocumentTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentTabItem),
                new FrameworkPropertyMetadata(typeof(LayoutDocumentTabItem)));
        }

        protected override void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnModelChanged(e);
            if (e.OldValue is LayoutContent oldModel)
                oldModel.PropertyChanged -= OldModel_PropertyChanged;
            if (e.NewValue is LayoutContent newModel)
                newModel.PropertyChanged += OldModel_PropertyChanged;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                if (LayoutItem.CloseCommand.CanExecute(null))
                    LayoutItem.CloseCommand.Execute(null);
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

            if (e.ClickCount == 2)
            {
                if (NativeMethods.IsKeyPressed(17))
                    Model.Float();
            }
            else if (e.ClickCount == 1)
            {
                _mouseDownPoint = this.PointToScreenDpi(e.GetPosition(this));
                _isMouseDown = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            _isMouseDown = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (IsMouseDirectlyOver)
                Model.IsActive = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown)
            {
                var ptMouseMove = this.PointToScreenDpi(e.GetPosition(this));

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
                var indexOfTabItemWithMouseOver =
                    _otherTabsScreenArea.FindIndex(r => r.Contains(mousePosInScreenCoord));
                if (indexOfTabItemWithMouseOver < 0)
                    return;
                var targetModel = _otherTabs[indexOfTabItemWithMouseOver].Content as LayoutContent;
                if (targetModel != null && Model.IsPinned != targetModel.IsPinned)
                    return;
                var container = Model.Parent;
                var containerPane = Model.Parent as ILayoutPane;
                var childrenList = container.Children.ToList();
                containerPane?.MoveChild(childrenList.IndexOf(Model), childrenList.IndexOf(targetModel));
                _parentDocumentTabPanel.UpdateLayout();
                UpdateDragDetails();
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            LayoutItem.ActivateCommand.Execute(null);
        }

        private void OldModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPinned")
                this.FindLogicalAncestor<DocumentPaneTabPanel>()?.InvalidateMeasure();
        }

        private void UpdateDragDetails()
        {
            _parentDocumentTabPanel = this.FindLogicalAncestor<DocumentPaneTabPanel>();
            _parentDocumentTabPanelScreenArea = _parentDocumentTabPanel.GetScreenArea();
            _otherTabs = _parentDocumentTabPanel.Children.Cast<TabItem>().Where(ch =>
                ch.Visibility != Visibility.Collapsed).ToList();
            var currentTabScreenArea = this.FindLogicalAncestor<TabItem>().GetScreenArea();
            _otherTabsScreenArea = _otherTabs.Select(ti =>
            {
                var screenArea = ti.GetScreenArea();
                return new Rect(screenArea.Left, screenArea.Top, currentTabScreenArea.Width, screenArea.Height);
            }).ToList();
        }
    }
}