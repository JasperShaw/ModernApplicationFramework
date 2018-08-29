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
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    public abstract class LayoutGroupBase : LayoutElement
    {
        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler ChildrenCollectionChanged;

        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;

        protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
        {
            OnChildrenTreeChanged(change);
            var parentGroup = Parent as LayoutGroupBase;
            parentGroup?.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
        }

        protected virtual void OnChildrenCollectionChanged()
        {
            ChildrenCollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
        {
            ChildrenTreeChanged?.Invoke(this, new ChildrenTreeChangedEventArgs(change));
        }
    }
}