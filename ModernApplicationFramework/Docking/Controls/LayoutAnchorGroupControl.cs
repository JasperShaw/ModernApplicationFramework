/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutAnchorGroupControl : Control, ILayoutControl
    {
        private readonly ObservableCollection<LayoutAnchorControl> _childViews =
            new ObservableCollection<LayoutAnchorControl>();

        private readonly LayoutAnchorGroup _model;

        internal LayoutAnchorGroupControl(LayoutAnchorGroup model)
        {
            _model = model;
            CreateChildrenViews();

            _model.Children.CollectionChanged += (s, e) => OnModelChildrenCollectionChanged(e);
        }

        static LayoutAnchorGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorGroupControl),
                new FrameworkPropertyMetadata(typeof (LayoutAnchorGroupControl)));
        }

        public ILayoutElement Model => _model;
        public ObservableCollection<LayoutAnchorControl> Children => _childViews;

        private void CreateChildrenViews()
        {
            var manager = _model.Root.Manager;
            foreach (var childModel in _model.Children)
            {
                _childViews.Add(new LayoutAnchorControl(childModel) {Template = manager.AnchorTemplate});
            }
        }

        private void OnModelChildrenCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    {
                        foreach (var childModel in e.OldItems)
                            _childViews.Remove(_childViews.First(cv => cv.Model == childModel));
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                _childViews.Clear();

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    var manager = _model.Root.Manager;
                    int insertIndex = e.NewStartingIndex;
                    foreach (LayoutAnchorable childModel in e.NewItems)
                    {
                        _childViews.Insert(insertIndex++,
                            new LayoutAnchorControl(childModel) {Template = manager.AnchorTemplate});
                    }
                }
            }
        }
    }
}