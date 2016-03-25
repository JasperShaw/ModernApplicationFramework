/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows.Data;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;

namespace ModernApplicationFramework.Docking.Controls
{
    public class ContextMenuEx : ContextMenu
    {
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MenuItemEx();
        }

        protected override void OnOpened(System.Windows.RoutedEventArgs e)
        {
            var bindingExpression = BindingOperations.GetBindingExpression(this, ItemsSourceProperty);
            bindingExpression?.UpdateTarget();
            base.OnOpened(e);
        }
    }
}