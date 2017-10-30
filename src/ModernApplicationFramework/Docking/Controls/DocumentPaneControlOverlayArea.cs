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

namespace ModernApplicationFramework.Docking.Controls
{
    public class DocumentPaneControlOverlayArea : OverlayArea
    {
        internal DocumentPaneControlOverlayArea(
            IOverlayWindow overlayWindow,
            LayoutDocumentPaneControl documentPaneControl)
            : base(overlayWindow)
        {
            var documentPaneControl1 = documentPaneControl;
            SetScreenDetectionArea(new Rect(
                documentPaneControl1.PointToScreenDpi(new Point()),
                documentPaneControl1.TransformActualSizeToAncestor()));
        }
    }
}