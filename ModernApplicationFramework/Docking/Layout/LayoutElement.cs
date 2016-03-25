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
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    public abstract class LayoutElement : DependencyObject, ILayoutElement
    {
        internal LayoutElement()
        {
        }

        public ILayoutRoot Root
        {
            get
            {
                var parent = Parent;

                while (parent != null && (!(parent is ILayoutRoot)))
                {
                    parent = parent.Parent;
                }

                return (ILayoutRoot) parent;
            }
        }

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangingEventHandler PropertyChanging;

#if TRACE
        public virtual void ConsoleDump(int tab)
        {
            System.Diagnostics.Trace.Write(new String(' ', tab*4));
            System.Diagnostics.Trace.WriteLine(ToString());
        }
#endif

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #region Parent

        [NonSerialized] private ILayoutContainer _parent;
        [NonSerialized] private ILayoutRoot _root;

        [XmlIgnore]
        public ILayoutContainer Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;
                ILayoutContainer oldValue = _parent;
                ILayoutRoot oldRoot = _root;
                RaisePropertyChanging("Parent");
                OnParentChanging(oldValue, value);
                _parent = value;
                OnParentChanged(oldValue, value);

                _root = Root;
                if (oldRoot != _root)
                    OnRootChanged(oldRoot, _root);

                RaisePropertyChanged("Parent");

                var root = Root as LayoutRoot;
                root?.FireLayoutUpdated();
            }
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle execute code before to the Parent property changes.
        /// </summary>
        protected virtual void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Parent property.
        /// </summary>
        protected virtual void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
        }


        protected virtual void OnRootChanged(ILayoutRoot oldRoot, ILayoutRoot newRoot)
        {
            if (oldRoot != null)
                ((LayoutRoot) oldRoot).OnLayoutElementRemoved(this);
            if (newRoot != null)
                ((LayoutRoot) newRoot).OnLayoutElementAdded(this);
        }

        #endregion
    }
}