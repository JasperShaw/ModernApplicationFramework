/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class AnchorablePaneDropTarget : DropTarget<LayoutAnchorablePaneControl>
    {
        private readonly int _tabIndex = -1;
        private readonly LayoutAnchorablePaneControl _targetPane;

        internal AnchorablePaneDropTarget(LayoutAnchorablePaneControl paneControl, Rect detectionRect,
            DropTargetType type)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
        }

        internal AnchorablePaneDropTarget(LayoutAnchorablePaneControl paneControl, Rect detectionRect,
            DropTargetType type,
            int tabIndex)
            : base(paneControl, detectionRect, type)
        {
            _targetPane = paneControl;
            _tabIndex = tabIndex;
        }

        public override Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindowModel)
        {
            //var anchorablePaneDropTarget = target as AnchorablePaneDropTarget;

            switch (Type)
            {
                case DropTargetType.AnchorablePaneDockBottom:
                {
                    var targetScreenRect = TargetElement.GetScreenArea();
                    targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                    targetScreenRect.Offset(0.0, targetScreenRect.Height/2.0);
                    targetScreenRect.Height /= 2.0;

                    return new RectangleGeometry(targetScreenRect);
                }
                case DropTargetType.AnchorablePaneDockTop:
                {
                    var targetScreenRect = TargetElement.GetScreenArea();
                    targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                    targetScreenRect.Height /= 2.0;

                    return new RectangleGeometry(targetScreenRect);
                }
                case DropTargetType.AnchorablePaneDockLeft:
                {
                    var targetScreenRect = TargetElement.GetScreenArea();
                    targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                    targetScreenRect.Width /= 2.0;

                    return new RectangleGeometry(targetScreenRect);
                }
                case DropTargetType.AnchorablePaneDockRight:
                {
                    var targetScreenRect = TargetElement.GetScreenArea();
                    targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                    targetScreenRect.Offset(targetScreenRect.Width/2.0, 0.0);
                    targetScreenRect.Width /= 2.0;

                    return new RectangleGeometry(targetScreenRect);
                }
                case DropTargetType.AnchorablePaneDockInside:
                {
                    var targetScreenRect = TargetElement.GetScreenArea();
                    targetScreenRect.Offset(-overlayWindow.Left, -overlayWindow.Top);

                    if (_tabIndex == -1)
                    {
                        return new RectangleGeometry(targetScreenRect);
                    }
                    var translatedDetectionRect = new Rect(DetectionRects[0].TopLeft, DetectionRects[0].BottomRight);
                    translatedDetectionRect.Offset(-overlayWindow.Left, -overlayWindow.Top);
                    var pathFigure = new PathFigure {StartPoint = targetScreenRect.TopLeft};
                    pathFigure.Segments.Add(new LineSegment()
                    {
                        Point = new Point(targetScreenRect.Left, translatedDetectionRect.Top)
                    });
                    pathFigure.Segments.Add(new LineSegment() {Point = translatedDetectionRect.TopLeft});
                    pathFigure.Segments.Add(new LineSegment() {Point = translatedDetectionRect.BottomLeft});
                    pathFigure.Segments.Add(new LineSegment() {Point = translatedDetectionRect.BottomRight});
                    pathFigure.Segments.Add(new LineSegment() {Point = translatedDetectionRect.TopRight});
                    pathFigure.Segments.Add(new LineSegment()
                    {
                        Point = new Point(targetScreenRect.Right, translatedDetectionRect.Top)
                    });
                    pathFigure.Segments.Add(new LineSegment() {Point = targetScreenRect.TopRight});
                    pathFigure.IsClosed = true;
                    pathFigure.IsFilled = true;
                    pathFigure.Freeze();
                    return new PathGeometry(new[] {pathFigure});
                }
            }

            return null;
        }

        protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
            ILayoutAnchorablePane targetModel = _targetPane.Model as ILayoutAnchorablePane;
            LayoutAnchorable anchorableActive = floatingWindow.Descendents().OfType<LayoutAnchorable>().FirstOrDefault();

            switch (Type)
            {
                case DropTargetType.AnchorablePaneDockBottom:
                {
                    if (targetModel != null)
                    {
                        var parentModel = targetModel.Parent as ILayoutGroup;
                        var parentModelOrientable = targetModel.Parent as ILayoutOrientableGroup;
                        if (parentModel != null)
                        {
                            int insertToIndex = parentModel.IndexOfChild(targetModel);

                            if (parentModelOrientable != null &&
                                (parentModelOrientable.Orientation != System.Windows.Controls.Orientation.Vertical &&
                                 parentModel.ChildrenCount == 1))
                                parentModelOrientable.Orientation = System.Windows.Controls.Orientation.Vertical;

                            if (parentModelOrientable != null &&
                                parentModelOrientable.Orientation == System.Windows.Controls.Orientation.Vertical)
                            {
                                var layoutAnchorablePaneGroup = floatingWindow.RootPanel;
                                if (layoutAnchorablePaneGroup != null &&
                                    (layoutAnchorablePaneGroup.Children.Count == 1 ||
                                     layoutAnchorablePaneGroup.Orientation ==
                                     System.Windows.Controls.Orientation.Vertical))
                                {
                                    var anchorablesToMove = layoutAnchorablePaneGroup.Children.ToArray();
                                    for (int i = 0; i < anchorablesToMove.Length; i++)
                                        parentModel.InsertChildAt(insertToIndex + 1 + i, anchorablesToMove[i]);
                                }
                                else
                                    parentModel.InsertChildAt(insertToIndex + 1, floatingWindow.RootPanel);
                            }
                            else
                            {
                                var targetModelAsPositionableElement = targetModel as ILayoutPositionableElement;
                                if (targetModelAsPositionableElement != null)
                                {
                                    var newOrientedPanel = new LayoutAnchorablePaneGroup
                                    {
                                        Orientation = System.Windows.Controls.Orientation.Vertical,
                                        DockWidth = targetModelAsPositionableElement.DockWidth,
                                        DockHeight = targetModelAsPositionableElement.DockHeight,
                                    };

                                    parentModel.InsertChildAt(insertToIndex, newOrientedPanel);
                                    newOrientedPanel.Children.Add(targetModel);
                                    newOrientedPanel.Children.Add(floatingWindow.RootPanel);
                                }
                            }
                        }
                    }
                }
                    break;
                case DropTargetType.AnchorablePaneDockTop:
                {
                    if (targetModel != null)
                    {
                        var parentModel = targetModel.Parent as ILayoutGroup;
                        var parentModelOrientable = targetModel.Parent as ILayoutOrientableGroup;
                        if (parentModel != null)
                        {
                            int insertToIndex = parentModel.IndexOfChild(targetModel);

                            if (parentModelOrientable != null &&
                                (parentModelOrientable.Orientation != System.Windows.Controls.Orientation.Vertical &&
                                 parentModel.ChildrenCount == 1))
                                parentModelOrientable.Orientation = System.Windows.Controls.Orientation.Vertical;

                            if (parentModelOrientable != null &&
                                parentModelOrientable.Orientation == System.Windows.Controls.Orientation.Vertical)
                            {
                                var layoutAnchorablePaneGroup = floatingWindow.RootPanel;
                                if (layoutAnchorablePaneGroup != null &&
                                    (layoutAnchorablePaneGroup.Children.Count == 1 ||
                                     layoutAnchorablePaneGroup.Orientation ==
                                     System.Windows.Controls.Orientation.Vertical))
                                {
                                    var anchorablesToMove = layoutAnchorablePaneGroup.Children.ToArray();
                                    for (int i = 0; i < anchorablesToMove.Length; i++)
                                        parentModel.InsertChildAt(insertToIndex + i, anchorablesToMove[i]);
                                }
                                else
                                    parentModel.InsertChildAt(insertToIndex, floatingWindow.RootPanel);
                            }
                            else
                            {
                                var targetModelAsPositionableElement = targetModel as ILayoutPositionableElement;
                                if (targetModelAsPositionableElement != null)
                                {
                                    var newOrientedPanel = new LayoutAnchorablePaneGroup
                                    {
                                        Orientation = System.Windows.Controls.Orientation.Vertical,
                                        DockWidth = targetModelAsPositionableElement.DockWidth,
                                        DockHeight = targetModelAsPositionableElement.DockHeight,
                                    };

                                    parentModel.InsertChildAt(insertToIndex, newOrientedPanel);
                                    //the floating window must be added after the target modal as it could be raise a CollectGarbage call
                                    newOrientedPanel.Children.Add(targetModel);
                                    newOrientedPanel.Children.Insert(0, floatingWindow.RootPanel);
                                }
                            }
                        }
                    }
                }
                    break;
                case DropTargetType.AnchorablePaneDockLeft:
                {
                    if (targetModel != null)
                    {
                        var parentModel = targetModel.Parent as ILayoutGroup;
                        var parentModelOrientable = targetModel.Parent as ILayoutOrientableGroup;
                        if (parentModel != null)
                        {
                            int insertToIndex = parentModel.IndexOfChild(targetModel);

                            if (parentModelOrientable != null &&
                                (parentModelOrientable.Orientation != System.Windows.Controls.Orientation.Horizontal &&
                                 parentModel.ChildrenCount == 1))
                                parentModelOrientable.Orientation = System.Windows.Controls.Orientation.Horizontal;

                            if (parentModelOrientable != null &&
                                parentModelOrientable.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                var layoutAnchorablePaneGroup = floatingWindow.RootPanel;
                                if (layoutAnchorablePaneGroup != null &&
                                    (layoutAnchorablePaneGroup.Children.Count == 1 ||
                                     layoutAnchorablePaneGroup.Orientation ==
                                     System.Windows.Controls.Orientation.Horizontal))
                                {
                                    var anchorablesToMove = layoutAnchorablePaneGroup.Children.ToArray();
                                    for (int i = 0; i < anchorablesToMove.Length; i++)
                                        parentModel.InsertChildAt(insertToIndex + i, anchorablesToMove[i]);
                                }
                                else
                                    parentModel.InsertChildAt(insertToIndex, floatingWindow.RootPanel);
                            }
                            else
                            {
                                var targetModelAsPositionableElement = targetModel as ILayoutPositionableElement;
                                if (targetModelAsPositionableElement != null)
                                {
                                    var newOrientedPanel = new LayoutAnchorablePaneGroup
                                    {
                                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                                        DockWidth = targetModelAsPositionableElement.DockWidth,
                                        DockHeight = targetModelAsPositionableElement.DockHeight,
                                    };

                                    parentModel.InsertChildAt(insertToIndex, newOrientedPanel);
                                    //the floating window must be added after the target modal as it could be raise a CollectGarbage call
                                    newOrientedPanel.Children.Add(targetModel);
                                    newOrientedPanel.Children.Insert(0, floatingWindow.RootPanel);
                                }
                            }
                        }
                    }
                }
                    break;
                case DropTargetType.AnchorablePaneDockRight:
                {
                    if (targetModel != null)
                    {
                        var parentModel = targetModel.Parent as ILayoutGroup;
                        var parentModelOrientable = targetModel.Parent as ILayoutOrientableGroup;
                        if (parentModel != null)
                        {
                            int insertToIndex = parentModel.IndexOfChild(targetModel);

                            if (parentModelOrientable != null &&
                                (parentModelOrientable.Orientation != System.Windows.Controls.Orientation.Horizontal &&
                                 parentModel.ChildrenCount == 1))
                                parentModelOrientable.Orientation = System.Windows.Controls.Orientation.Horizontal;

                            if (parentModelOrientable != null &&
                                parentModelOrientable.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                var layoutAnchorablePaneGroup = floatingWindow.RootPanel;
                                if (layoutAnchorablePaneGroup != null &&
                                    (layoutAnchorablePaneGroup.Children.Count == 1 ||
                                     layoutAnchorablePaneGroup.Orientation ==
                                     System.Windows.Controls.Orientation.Horizontal))
                                {
                                    var anchorablesToMove = layoutAnchorablePaneGroup.Children.ToArray();
                                    for (int i = 0; i < anchorablesToMove.Length; i++)
                                        parentModel.InsertChildAt(insertToIndex + 1 + i, anchorablesToMove[i]);
                                }
                                else
                                    parentModel.InsertChildAt(insertToIndex + 1, floatingWindow.RootPanel);
                            }
                            else
                            {
                                var targetModelAsPositionableElement = targetModel as ILayoutPositionableElement;
                                if (targetModelAsPositionableElement != null)
                                {
                                    var newOrientedPanel = new LayoutAnchorablePaneGroup
                                    {
                                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                                        DockWidth = targetModelAsPositionableElement.DockWidth,
                                        DockHeight = targetModelAsPositionableElement.DockHeight,
                                    };

                                    parentModel.InsertChildAt(insertToIndex, newOrientedPanel);
                                    newOrientedPanel.Children.Add(targetModel);
                                    newOrientedPanel.Children.Add(floatingWindow.RootPanel);
                                }
                            }
                        }
                    }
                }
                    break;
                case DropTargetType.AnchorablePaneDockInside:
                {
                    var paneModel = targetModel as LayoutAnchorablePane;
                    var layoutAnchorablePaneGroup = floatingWindow.RootPanel;

                    int i = _tabIndex == -1 ? 0 : _tabIndex;
                    foreach (var anchorableToImport in 
                        layoutAnchorablePaneGroup.Descendents().OfType<LayoutAnchorable>().ToArray())
                    {
                        paneModel?.Children.Insert(i, anchorableToImport);
                        i++;
                    }
                }
                    break;
            }
            if (anchorableActive != null)
                anchorableActive.IsActive = true;
            base.Drop(floatingWindow);
        }
    }
}