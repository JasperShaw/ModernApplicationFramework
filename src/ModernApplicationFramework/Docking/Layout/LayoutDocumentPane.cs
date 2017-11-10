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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using Caliburn.Micro;
using Action = System.Action;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutDocumentPane : LayoutPositionableGroup<LayoutContent>, ILayoutDocumentPane,
        ILayoutContentSelector,
        ILayoutPaneSerializable
    {
        private readonly ObservableCollection<LayoutContent> _pinnedViews;

        private readonly BindableCollection<LayoutContent> _readOnlyPinnedViews;


        private DispatcherOperation _asyncChildReorderOperation;
        private string _id;
        private int _selectedIndex = -1;

        public IEnumerable<LayoutContent> ChildrenSorted
        {
            get
            {
                var listSorted = Children.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }

        public IObservableCollection<LayoutContent> PinnedViews => _readOnlyPinnedViews;

        public LayoutContent SelectedContent => _selectedIndex == -1 ? null : Children[_selectedIndex];

        public int SelectedContentIndex
        {
            get => _selectedIndex;
            set
            {
                if (value < 0 ||
                    value >= Children.Count)
                    value = -1;

                if (_selectedIndex != value)
                {
                    RaisePropertyChanging("SelectedContentIndex");
                    RaisePropertyChanging("SelectedContent");
                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                        Children[_selectedIndex].IsSelected = false;

                    _selectedIndex = value;

                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                        Children[_selectedIndex].IsSelected = true;

                    RaisePropertyChanged("SelectedContentIndex");
                    RaisePropertyChanged("SelectedContent");
                }
            }
        }

        string ILayoutPaneSerializable.Id
        {
            get => _id;
            set => _id = value;
        }


        public LayoutDocumentPane()
        {
            _pinnedViews = new ObservableCollection<LayoutContent>();
            _readOnlyPinnedViews = new BindableCollection<LayoutContent>(_pinnedViews);
            _pinnedViews.CollectionChanged += OnPinnedViewsCollectionChanged;
        }


        public LayoutDocumentPane(LayoutContent firstChild) : this()
        {
            Children.Add(firstChild);
        }

#if TRACE
        public override void ConsoleDump(int tab)
        {
            Trace.Write(new string(' ', tab * 4));
            Trace.WriteLine("DocumentPane()");

            foreach (var child in Children)
                child.ConsoleDump(tab + 1);
        }
#endif

        public int IndexOf(LayoutContent content)
        {
            return Children.IndexOf(content);
        }

        public override void ReadXml(XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
                _id = reader.Value;


            base.ReadXml(reader);
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (_id != null)
                writer.WriteAttributeString("Id", _id);

            base.WriteXml(writer);
        }


        internal void OnChildPinnedStatusChanged(LayoutContent element)
        {
            if (!(element is LayoutContent view))
                return;
            if (view.IsPinned)
                _pinnedViews.Add(view);
            else
                _pinnedViews.Remove(view);
        }


        internal void SetNextSelectedIndex()
        {
            SelectedContentIndex = -1;
            for (var i = 0; i < Children.Count; ++i)
            {
                if (!Children[i].IsEnabled)
                    continue;
                SelectedContentIndex = i;
                return;
            }
        }


        protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnChildrenChanged(args);
            var arrayList1 = new ArrayList();
            var arrayList2 = new ArrayList();
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    arrayList1.AddRange(args.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //if (this.PreviewView != null && args.OldItems.Contains((object)this.PreviewView))
                    //    this.PreviewView = (View)null;
                    arrayList2.AddRange(args.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    arrayList1.AddRange(args.NewItems);
                    goto case NotifyCollectionChangedAction.Remove;
                case NotifyCollectionChangedAction.Move:
                    //if (PreviewView != null && Children[PreviewViewIndex] != PreviewView)
                    //{
                    //    ScheduleAsyncChildReorder();
                    //    break;
                    //}
                    break;
                case NotifyCollectionChangedAction.Reset:
                    //this.PreviewView = (View)null;
                    _pinnedViews.Clear();
                    if (args.NewItems != null)
                        arrayList1.AddRange(args.NewItems);
                    break;
            }
            for (var index1 = 0; index1 < arrayList1.Count; ++index1)
                if (arrayList1[index1] is LayoutContent view)
                {
                    var index2 = args.NewStartingIndex + index1;
                    //if (IsPastPreviewIndex(index2))
                    //    ScheduleAsyncChildReorder();
                    if (view.IsPinned)
                        if (index2 > _pinnedViews.Count)
                            _pinnedViews.Add(view);
                        else
                            _pinnedViews.Insert(index2, view);
                    else if (IsPinnedIndex(index2))
                        ScheduleAsyncChildReorder();
                }
            foreach (LayoutContent viewElement in arrayList2)
                if (viewElement is LayoutContent view && view.IsPinned)
                {
                    if (!DockingManager.Instance.Preferences.MaintainPinStatus)
                        view.IsPinned = false;
                    _pinnedViews.Remove(view);
                }

        }

        protected override void ChildMoved(int oldIndex, int newIndex)
        {
            if (_selectedIndex == oldIndex)
            {
                RaisePropertyChanging("SelectedContentIndex");
                _selectedIndex = newIndex;
                RaisePropertyChanged("SelectedContentIndex");
            }


            base.ChildMoved(oldIndex, newIndex);
        }

        protected override bool GetVisibility()
        {
            if (Parent is LayoutDocumentPaneGroup)
                return ChildrenCount > 0;
            return true;
        }

        protected override void OnChildrenCollectionChanged()
        {
            if (SelectedContentIndex >= ChildrenCount)
                SelectedContentIndex = Children.Count - 1;
            if (SelectedContentIndex == -1 && ChildrenCount > 0)
                if (Root == null) //if I'm not yet connected just switch to first document
                {
                    SelectedContentIndex = 0;
                }
                else
                {
                    var childrenToSelect =
                        Children.OrderByDescending(c => c.LastActivationTimeStamp.GetValueOrDefault()).First();
                    SelectedContentIndex = Children.IndexOf(childrenToSelect);
                    childrenToSelect.IsActive = true;
                }

            base.OnChildrenCollectionChanged();

            RaisePropertyChanged("ChildrenSorted");
        }

        protected override void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
            base.OnIsVisibleChanged();
        }

        protected override void SetXmlAttributeValue(string name, string valueString)
        {
            switch (name)
            {
                case "Id":
                    _id = valueString;
                    break;
                default:
                    base.SetXmlAttributeValue(name, valueString);
                    break;
            }
        }

        private void HandleAddedPinnedViews(IList newViews, int newIndex)
        {
            var num1 = 0;
            if (newIndex > 0)
            {
                var num2 = Children.IndexOf(_pinnedViews[newIndex]);
                var num3 = Children.IndexOf(_pinnedViews[newIndex - 1]);
                num1 = num2 >= num3 ? num3 + 1 : num3;
            }
            var enumerator = newViews.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    if (MoveChild(Children.ToList().IndexOf((LayoutContent) enumerator.Current), num1++) !=
                        MoveResult.Scheduled)
                        break;
                    ScheduleAsyncChildReorder();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        private void HandleMovedPinnedViews(IList movedViews, int oldIndex, int newIndex)
        {
            var count = movedViews.Count;
            while (count > 0)
            {
                if (MoveChild(oldIndex++, newIndex++) != MoveResult.Scheduled)
                    break;
                ScheduleAsyncChildReorder();
                --count;
            }
        }

        private void HandleRemovedPinnedViews(IEnumerable oldViews)
        {
            if (_pinnedViews.Count == 0)
                return;
            var count = _pinnedViews.Count;
            if (oldViews.Cast<LayoutContent>().Select(oldView => Children.IndexOf(oldView))
                .Where(oldIndex => oldIndex != -1).Any(oldIndex =>
                    oldIndex != -1 && MoveChild(oldIndex, count++) == MoveResult.Scheduled))
                ScheduleAsyncChildReorder();
        }

        private bool IsPinnedIndex(int index)
        {
            if (index >= 0)
                return index < _pinnedViews.Count;
            return false;
        }

        private void OnPinnedViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    HandleAddedPinnedViews(e.NewItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    HandleRemovedPinnedViews(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    HandleRemovedPinnedViews(e.OldItems);
                    if (e.NewItems != null)
                        HandleAddedPinnedViews(e.NewItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    HandleMovedPinnedViews(e.NewItems, e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        private void ReorderChildren()
        {
            var source = new List<IndexBundle>();
            var flag = false;
            for (var index = 0; index < Children.Count; ++index)
            {
                var child = Children[index];
                if (child != null)
                    if (child.IsPinned)
                    {
                        if (flag)
                            source.Add(new IndexBundle(index, _pinnedViews.IndexOf(child)));
                    }
                    else
                    {
                        if (index < PinnedViews.Count)
                            flag = true;
                        //if (child == PreviewView && index < Children.Count - 1)
                        //{
                        //    MoveChild(index, Children.Count - 1);
                        //}
                    }
            }
            var array = source.OrderBy(bundle => bundle.PinnedIndex).ToArray();
            for (var index1 = 0; index1 < array.Length - 1; ++index1)
            for (var index2 = index1 + 1; index2 < array.Length; ++index2)
                if (array[index1].ChildIndex > array[index2].ChildIndex)
                    ++array[index2].ChildIndex;
            var num1 = PinnedViews.Count - source.Count;
            foreach (var oldIndex in array.Select(bundle => bundle.ChildIndex))
                MoveChild(oldIndex, num1++);
        }

        private void ScheduleAsyncChildReorder()
        {
            if (_asyncChildReorderOperation != null)
                return;
            var suspendValidationScope = SuspendChildValidation();
            try
            {
                _asyncChildReorderOperation = Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
                {
                    using (suspendValidationScope)
                    {
                        _asyncChildReorderOperation = null;
                        ReorderChildren();
                    }
                }));
            }
            catch (Exception)
            {
                suspendValidationScope.Dispose();
                throw;
            }
        }

        private IDisposable SuspendChildValidation()
        {
            return null;
        }

        private void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            parentPane?.ComputeVisibility();
        }


        private struct IndexBundle
        {
            public readonly int PinnedIndex;
            public int ChildIndex;

            public IndexBundle(int childIndex, int pinnedIndex)
            {
                ChildIndex = childIndex;
                PinnedIndex = pinnedIndex;
            }
        }
    }
}