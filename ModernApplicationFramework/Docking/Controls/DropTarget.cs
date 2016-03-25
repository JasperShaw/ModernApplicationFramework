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
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    internal abstract class DropTarget<T> : DropTargetBase, IDropTarget where T : FrameworkElement
    {
        private readonly Rect[] _detectionRect;

        protected DropTarget(T targetElement, Rect detectionRect, DropTargetType type)
        {
            TargetElement = targetElement;
            _detectionRect = new[] {detectionRect};
            Type = type;
        }

        protected DropTarget(T targetElement, IEnumerable<Rect> detectionRects, DropTargetType type)
        {
            TargetElement = targetElement;
            _detectionRect = detectionRects.ToArray();
            Type = type;
        }

        public void DragEnter()
        {
            SetIsDraggingOver(TargetElement, true);
        }

        public void DragLeave()
        {
            SetIsDraggingOver(TargetElement, false);
        }

        public void Drop(LayoutFloatingWindow floatingWindow)
        {
            var currentActiveContent = floatingWindow.Root.ActiveContent;
            var fwAsAnchorable = floatingWindow as LayoutAnchorableFloatingWindow;

            if (fwAsAnchorable != null)
            {
                Drop(fwAsAnchorable);
            }
            else
            {
                var fwAsDocument = floatingWindow as LayoutDocumentFloatingWindow;
                Drop(fwAsDocument);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                currentActiveContent.IsSelected = false;
                currentActiveContent.IsActive = false;
                currentActiveContent.IsActive = true;
            }), DispatcherPriority.Background);
        }

        public abstract Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

        public virtual bool HitTest(Point dragPoint)
        {
            return _detectionRect.Any(dr => dr.Contains(dragPoint));
        }

        public DropTargetType Type { get; }
        public Rect[] DetectionRects => _detectionRect;
        public T TargetElement { get; }

        protected virtual void Drop(LayoutAnchorableFloatingWindow floatingWindow)
        {
        }

        protected virtual void Drop(LayoutDocumentFloatingWindow floatingWindow)
        {
        }
    }
}