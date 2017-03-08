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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("RootPanel")]
    [Serializable]
    public class LayoutRoot : LayoutElement, ILayoutContainer, ILayoutRoot
    {
        [field: NonSerialized] private WeakReference _activeContent;
        private bool _activeContentSet;
        private LayoutAnchorSide _bottomSide;
        private ObservableCollection<LayoutFloatingWindow> _floatingWindows;
        private ObservableCollection<LayoutAnchorable> _hiddenAnchorables;
        [field: NonSerialized] private WeakReference _lastFocusedDocument;
        private LayoutAnchorSide _leftSide;
        [NonSerialized] private DockingManager _manager;
        private LayoutAnchorSide _rightSide;
        private LayoutPanel _rootPanel;
        private LayoutAnchorSide _topSide;

        public LayoutRoot()
        {
            RightSide = new LayoutAnchorSide();
            LeftSide = new LayoutAnchorSide();
            TopSide = new LayoutAnchorSide();
            BottomSide = new LayoutAnchorSide();
            RootPanel = new LayoutPanel(new LayoutDocumentPane());
        }

        public event EventHandler Updated;

        public IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (RootPanel != null)
                    yield return RootPanel;
                if (_floatingWindows != null)
                {
                    foreach (var floatingWindow in _floatingWindows)
                        yield return floatingWindow;
                }
                if (TopSide != null)
                    yield return TopSide;
                if (RightSide != null)
                    yield return RightSide;
                if (BottomSide != null)
                    yield return BottomSide;
                if (LeftSide != null)
                    yield return LeftSide;
                if (_hiddenAnchorables != null)
                {
                    foreach (var hiddenAnchorable in _hiddenAnchorables)
                        yield return hiddenAnchorable;
                }
            }
        }

        public int ChildrenCount => 5 + (_floatingWindows?.Count ?? 0) + (_hiddenAnchorables?.Count ?? 0);

        public void RemoveChild(ILayoutElement element)
        {
            if (Equals(element, RootPanel))
                RootPanel = null;
            else if (_floatingWindows != null && _floatingWindows.Contains(element))
                _floatingWindows.Remove(element as LayoutFloatingWindow);
            else if (_hiddenAnchorables != null && _hiddenAnchorables.Contains(element))
                _hiddenAnchorables.Remove(element as LayoutAnchorable);
            else if (Equals(element, TopSide))
                TopSide = null;
            else if (Equals(element, RightSide))
                RightSide = null;
            else if (Equals(element, BottomSide))
                BottomSide = null;
            else if (Equals(element, LeftSide))
                LeftSide = null;
        }

        public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            if (Equals(oldElement, RootPanel))
                RootPanel = (LayoutPanel) newElement;
            else if (_floatingWindows != null && _floatingWindows.Contains(oldElement))
            {
                int index = _floatingWindows.IndexOf(oldElement as LayoutFloatingWindow);
                _floatingWindows.Remove(oldElement as LayoutFloatingWindow);
                _floatingWindows.Insert(index, newElement as LayoutFloatingWindow);
            }
            else if (_hiddenAnchorables != null && _hiddenAnchorables.Contains(oldElement))
            {
                int index = _hiddenAnchorables.IndexOf(oldElement as LayoutAnchorable);
                _hiddenAnchorables.Remove(oldElement as LayoutAnchorable);
                _hiddenAnchorables.Insert(index, newElement as LayoutAnchorable);
            }
            else if (Equals(oldElement, TopSide))
                TopSide = (LayoutAnchorSide) newElement;
            else if (Equals(oldElement, RightSide))
                RightSide = (LayoutAnchorSide) newElement;
            else if (Equals(oldElement, BottomSide))
                BottomSide = (LayoutAnchorSide) newElement;
            else if (Equals(oldElement, LeftSide))
                LeftSide = (LayoutAnchorSide) newElement;
        }

        [XmlIgnore]
        public LayoutContent ActiveContent
        {
            get => _activeContent.GetValueOrDefault<LayoutContent>();
            set
            {
                var currentValue = ActiveContent;
                if (!Equals(currentValue, value))
                {
                    InternalSetActiveContent(currentValue, value);
                }
            }
        }

        public LayoutAnchorSide BottomSide
        {
            get => _bottomSide;
            set
            {
                if (Equals(_bottomSide, value))
                    return;
                RaisePropertyChanging("BottomSide");
                _bottomSide = value;
                if (_bottomSide != null)
                    _bottomSide.Parent = this;
                RaisePropertyChanged("BottomSide");
            }
        }

        /// <summary>
        /// Removes any empty container not directly referenced by other layout items
        /// </summary>
        public void CollectGarbage()
        {
            bool exitFlag;

            #region collect empty panes

            do
            {
                exitFlag = true;

                //for each content that references via PreviousContainer a disconnected Pane set the property to null
                foreach (
                    var content in
                        this.Descendents().OfType<ILayoutPreviousContainer>().Where(c => c.PreviousContainer != null &&
                                                                                         (c.PreviousContainer.Parent ==
                                                                                          null ||
                                                                                          !Equals(
                                                                                              c.PreviousContainer.Parent
                                                                                                  .Root, this))))
                {
                    content.PreviousContainer = null;
                }

                //for each pane that is empty
                foreach (var emptyPane in this.Descendents().OfType<ILayoutPane>().Where(p => p.ChildrenCount == 0))
                {
                    //...set null any reference coming from contents not yet hosted in a floating window
                    foreach (var contentReferencingEmptyPane in this.Descendents().OfType<LayoutContent>()
                        .Where(c => ((ILayoutPreviousContainer) c).PreviousContainer == emptyPane && !c.IsFloating)
                        .Where(
                            contentReferencingEmptyPane =>
                                !(contentReferencingEmptyPane is LayoutAnchorable) ||
                                ((LayoutAnchorable) contentReferencingEmptyPane).IsVisible))
                    {
                        ((ILayoutPreviousContainer) contentReferencingEmptyPane).PreviousContainer = null;
                        contentReferencingEmptyPane.PreviousContainerIndex = -1;
                    }

                    //...if this pane is the only documentpane present in the layout than skip it
                    if (emptyPane is LayoutDocumentPane &&
                        this.Descendents().OfType<LayoutDocumentPane>().Count(c => !Equals(c, emptyPane)) == 0)
                        continue;

                    //...if this empty panes is not referenced by anyone, than removes it from its parent container
                    if (this.Descendents().OfType<ILayoutPreviousContainer>().Any(c => c.PreviousContainer == emptyPane))
                        continue;
                    var parentGroup = emptyPane.Parent;
                    parentGroup.RemoveChild(emptyPane);
                    exitFlag = false;
                    break;
                }

                if (!exitFlag)
                {
                    //removes any empty anchorable pane group
                    foreach (
                        var emptyPaneGroup in
                            this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        break;
                    }
                }

                if (!exitFlag)
                {
                    //removes any empty layout panel
                    foreach (
                        var emptyPaneGroup in this.Descendents().OfType<LayoutPanel>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        break;
                    }
                }

                if (!exitFlag)
                {
                    //removes any empty floating window
                    foreach (
                        var emptyPaneGroup in
                            this.Descendents().OfType<LayoutFloatingWindow>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        break;
                    }
                }

                if (!exitFlag)
                {
                    //removes any empty anchor group
                    foreach (
                        var emptyPaneGroup in
                            this.Descendents().OfType<LayoutAnchorGroup>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        break;
                    }
                }
            } while (!exitFlag);

            #endregion

            #region collapse single child anchorable pane groups

            do
            {
                exitFlag = true;
                //for each pane that is empty
                foreach (
                    var paneGroupToCollapse in
                        this.Descendents()
                            .OfType<LayoutAnchorablePaneGroup>()
                            .Where(p => p.ChildrenCount == 1 && p.Children[0] is LayoutAnchorablePaneGroup)
                            .ToArray())
                {
                    var singleChild = paneGroupToCollapse.Children[0] as LayoutAnchorablePaneGroup;
                    if (singleChild != null)
                    {
                        paneGroupToCollapse.Orientation = singleChild.Orientation;
                        paneGroupToCollapse.RemoveChild(singleChild);
                        while (singleChild.ChildrenCount > 0)
                        {
                            paneGroupToCollapse.InsertChildAt(
                                paneGroupToCollapse.ChildrenCount, singleChild.Children[0]);
                        }
                    }

                    exitFlag = false;
                    break;
                }
            } while (!exitFlag);

            #endregion

            #region collapse single child document pane groups

            do
            {
                exitFlag = true;
                //for each pane that is empty
                foreach (
                    var paneGroupToCollapse in
                        this.Descendents()
                            .OfType<LayoutDocumentPaneGroup>()
                            .Where(p => p.ChildrenCount == 1 && p.Children[0] is LayoutDocumentPaneGroup)
                            .ToArray())
                {
                    var singleChild = paneGroupToCollapse.Children[0] as LayoutDocumentPaneGroup;
                    if (singleChild != null)
                    {
                        paneGroupToCollapse.Orientation = singleChild.Orientation;
                        paneGroupToCollapse.RemoveChild(singleChild);
                        while (singleChild.ChildrenCount > 0)
                        {
                            paneGroupToCollapse.InsertChildAt(
                                paneGroupToCollapse.ChildrenCount, singleChild.Children[0]);
                        }
                    }

                    exitFlag = false;
                    break;
                }
            } while (!exitFlag);

            #endregion

            UpdateActiveContentProperty();

#if DEBUG
            Debug.Assert(
                !this.Descendents().OfType<LayoutAnchorablePane>().Any(a => a.ChildrenCount == 0 && a.IsVisible));
#if TRACE
            RootPanel.ConsoleDump(4);
#endif
#endif
        }

        public ObservableCollection<LayoutFloatingWindow> FloatingWindows
        {
            get
            {
                if (_floatingWindows != null) return _floatingWindows;
                _floatingWindows = new ObservableCollection<LayoutFloatingWindow>();
                _floatingWindows.CollectionChanged += _floatingWindows_CollectionChanged;

                return _floatingWindows;
            }
        }

        public ObservableCollection<LayoutAnchorable> Hidden
        {
            get
            {
                if (_hiddenAnchorables == null)
                {
                    _hiddenAnchorables = new ObservableCollection<LayoutAnchorable>();
                    _hiddenAnchorables.CollectionChanged += _hiddenAnchorables_CollectionChanged;
                }

                return _hiddenAnchorables;
            }
        }

        public LayoutAnchorSide LeftSide
        {
            get => _leftSide;
            set
            {
                if (Equals(_leftSide, value))
                    return;
                RaisePropertyChanging("LeftSide");
                _leftSide = value;
                if (_leftSide != null)
                    _leftSide.Parent = this;
                RaisePropertyChanged("LeftSide");
            }
        }

        [XmlIgnore]
        public DockingManager Manager
        {
            get => _manager;
            internal set
            {
                if (Equals(_manager, value))
                    return;
                RaisePropertyChanging("Manager");
                _manager = value;
                RaisePropertyChanged("Manager");
            }
        }

        public LayoutAnchorSide RightSide
        {
            get { return _rightSide; }
            set
            {
                if (!Equals(_rightSide, value))
                {
                    RaisePropertyChanging("RightSide");
                    _rightSide = value;
                    if (_rightSide != null)
                        _rightSide.Parent = this;
                    RaisePropertyChanged("RightSide");
                }
            }
        }

        public LayoutPanel RootPanel
        {
            get => _rootPanel;
            set
            {
                if (Equals(_rootPanel, value))
                    return;
                RaisePropertyChanging("RootPanel");
                if (_rootPanel != null &&
                    Equals(_rootPanel.Parent, this))
                    _rootPanel.Parent = null;
                _rootPanel = value ?? new LayoutPanel(new LayoutDocumentPane());

                if (_rootPanel != null)
                    _rootPanel.Parent = this;
                RaisePropertyChanged("RootPanel");
            }
        }

        public LayoutAnchorSide TopSide
        {
            get => _topSide;
            set
            {
                if (Equals(_topSide, value))
                    return;
                RaisePropertyChanging("TopSide");
                _topSide = value;
                if (_topSide != null)
                    _topSide.Parent = this;
                RaisePropertyChanged("TopSide");
            }
        }

        [XmlIgnore]
        public LayoutContent LastFocusedDocument
        {
            get => _lastFocusedDocument.GetValueOrDefault<LayoutContent>();
            private set
            {
                var currentValue = LastFocusedDocument;
                if (Equals(currentValue, value))
                    return;
                RaisePropertyChanging("LastFocusedDocument");
                if (currentValue != null)
                    currentValue.IsLastFocusedDocument = false;
                _lastFocusedDocument = new WeakReference(value);
                currentValue = LastFocusedDocument;
                if (currentValue != null)
                    currentValue.IsLastFocusedDocument = true;
                RaisePropertyChanged("LastFocusedDocument");
            }
        }

#if TRACE
        public override void ConsoleDump(int tab)
        {
            Trace.Write(new string(' ', tab*4));
            Trace.WriteLine("RootPanel()");

            RootPanel.ConsoleDump(tab + 1);

            Trace.Write(new string(' ', tab*4));
            Trace.WriteLine("FloatingWindows()");

            foreach (LayoutFloatingWindow fw in FloatingWindows)
                fw.ConsoleDump(tab + 1);

            Trace.Write(new string(' ', tab*4));
            Trace.WriteLine("Hidden()");

            foreach (LayoutAnchorable hidden in Hidden)
                hidden.ConsoleDump(tab + 1);
        }
#endif


        public void XmlDeserialize(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "LayoutRoot")
                    {
                        DeserializeLayoutRoot(xmlReader);
                    }
                }
            }
        }

        internal void FireLayoutUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        private void _floatingWindows_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (
                    LayoutFloatingWindow element in
                        e.OldItems.Cast<LayoutFloatingWindow>().Where(element => Equals(element.Parent, this)))
                {
                    element.Parent = null;
                }
            }

            if (e.NewItems != null && (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                                       e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (LayoutFloatingWindow element in e.NewItems)
                    element.Parent = this;
            }
        }

        private void _hiddenAnchorables_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (
                        LayoutAnchorable element in
                            e.OldItems.Cast<LayoutAnchorable>().Where(element => Equals(element.Parent, this)))
                    {
                        element.Parent = null;
                    }
                }
            }

            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add &&
                e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                return;
            if (e.NewItems == null)
                return;
            foreach (
                LayoutAnchorable element in
                    e.NewItems.Cast<LayoutAnchorable>().Where(element => !Equals(element.Parent, this)))
            {
                element.Parent?.RemoveChild(element);
                element.Parent = this;
            }
        }

        void DeserializeFloatingWindows(XmlReader xmlReader)
        {
            if (!xmlReader.IsEmptyElement)
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name == "LayoutFloatingWindow")
                        {
                            xmlReader.MoveToAttribute("xsi:type");
                            string type = xmlReader.Value;
                            if (type == "LayoutAnchorableFloatingWindow")
                            {
                                LayoutAnchorableFloatingWindow layoutAnchorableFloatingWindow =
                                    new LayoutAnchorableFloatingWindow();
                                layoutAnchorableFloatingWindow.XmlDeserialize(xmlReader);
                                FloatingWindows.Add(layoutAnchorableFloatingWindow);
                            }
                            else if (type == "LayoutDocumentFloatingWindow")
                            {
                                LayoutDocumentFloatingWindow layoutDocumentFloatingWindow =
                                    new LayoutDocumentFloatingWindow();
                                layoutDocumentFloatingWindow.XmlDeserialize(xmlReader);
                                FloatingWindows.Add(layoutDocumentFloatingWindow);
                            }
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                }
            }
        }

        void DeserializeHiddenWindows(XmlReader xmlReader)
        {
            if (xmlReader.IsEmptyElement)
                return;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;
                if (xmlReader.Name != "LayoutAnchorable")
                    continue;
                LayoutAnchorable layoutAnchorable = new LayoutAnchorable();
                layoutAnchorable.XmlDeserialize(xmlReader);
                Hidden.Add(layoutAnchorable);
            }
        }

        private void DeserializeLayoutRoot(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "RootPanel")
                    {
                        LayoutPanel layoutPanel = new LayoutPanel();
                        layoutPanel.XmlDeserialize(xmlReader);
                        RootPanel = layoutPanel;
                    }
                    else if (xmlReader.Name == "TopSide")
                    {
                        LayoutAnchorSide layoutAnchorSide = new LayoutAnchorSide();
                        layoutAnchorSide.XmlDeserialize(xmlReader);
                        TopSide = layoutAnchorSide;
                    }
                    else if (xmlReader.Name == "LeftSide")
                    {
                        LayoutAnchorSide layoutAnchorSide = new LayoutAnchorSide();
                        layoutAnchorSide.XmlDeserialize(xmlReader);
                        LeftSide = layoutAnchorSide;
                    }
                    else if (xmlReader.Name == "BottomSide")
                    {
                        LayoutAnchorSide layoutAnchorSide = new LayoutAnchorSide();
                        layoutAnchorSide.XmlDeserialize(xmlReader);
                        BottomSide = layoutAnchorSide;
                    }
                    else if (xmlReader.Name == "RightSide")
                    {
                        LayoutAnchorSide layoutAnchorSide = new LayoutAnchorSide();
                        layoutAnchorSide.XmlDeserialize(xmlReader);
                        RightSide = layoutAnchorSide;
                    }
                    else if (xmlReader.Name == "FloatingWindows")
                    {
                        DeserializeFloatingWindows(xmlReader);
                    }
                    else if (xmlReader.Name == "Hidden")
                    {
                        DeserializeHiddenWindows(xmlReader);
                    }
                    else
                    {
                        // Unhandled case. Fix it
                        Debugger.Break();
                    }
                }
            }
        }

        private void InternalSetActiveContent(LayoutContent currentValue, LayoutContent newActiveContent)
        {
            RaisePropertyChanging("ActiveContent");
            if (currentValue != null)
                currentValue.IsActive = false;
            _activeContent = new WeakReference(newActiveContent);
            currentValue = ActiveContent;
            if (currentValue != null)
                currentValue.IsActive = true;
            RaisePropertyChanged("ActiveContent");
            _activeContentSet = currentValue != null;
            if (currentValue != null)
            {
                if (currentValue.Parent is LayoutDocumentPane || currentValue is LayoutDocument)
                    LastFocusedDocument = currentValue;
            }
            else
                LastFocusedDocument = null;
        }

        private void UpdateActiveContentProperty()
        {
            var activeContent = ActiveContent;
            if (_activeContentSet && (activeContent == null || !Equals(activeContent.Root, this)))
            {
                _activeContentSet = false;
                InternalSetActiveContent(activeContent, null);
            }
        }

        #region LayoutElement Added/Removed events

        internal void OnLayoutElementAdded(LayoutElement element)
        {
            ElementAdded?.Invoke(this, new LayoutElementEventArgs(element));
        }

        public event EventHandler<LayoutElementEventArgs> ElementAdded;

        internal void OnLayoutElementRemoved(LayoutElement element)
        {
            if (element.Descendents().OfType<LayoutContent>().Any(c => Equals(c, LastFocusedDocument)))
                LastFocusedDocument = null;
            if (element.Descendents().OfType<LayoutContent>().Any(c => Equals(c, ActiveContent)))
                ActiveContent = null;
            ElementRemoved?.Invoke(this, new LayoutElementEventArgs(element));
        }

        public event EventHandler<LayoutElementEventArgs> ElementRemoved;

        #endregion
    }
}