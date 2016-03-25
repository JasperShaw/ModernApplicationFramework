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
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    [ContentProperty("RootPanel")]
    public class LayoutAnchorableFloatingWindow : LayoutFloatingWindow, ILayoutElementWithVisibility
    {
        [NonSerialized] private bool _isVisible = true;
        private LayoutAnchorablePaneGroup _rootPanel;
        public event EventHandler IsVisibleChanged;

        void ILayoutElementWithVisibility.ComputeVisibility()
        {
            IsVisible = RootPanel != null && RootPanel.IsVisible;
        }

        public override IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (ChildrenCount == 1)
                    yield return RootPanel;
            }
        }

        public override int ChildrenCount => RootPanel == null ? 0 : 1;

        public bool IsSinglePane
        {
            get
            {
                return RootPanel != null &&
                       RootPanel.Descendents().OfType<ILayoutAnchorablePane>().Count(p => p.IsVisible) == 1;
            }
        }

        public override bool IsValid => RootPanel != null;

        public ILayoutAnchorablePane SinglePane
        {
            get
            {
                if (!IsSinglePane)
                    return null;

                var singlePane = RootPanel.Descendents().OfType<LayoutAnchorablePane>().Single(p => p.IsVisible);
                singlePane.UpdateIsDirectlyHostedInFloatingWindow();
                return singlePane;
            }
        }

        [XmlIgnore]
        public bool IsVisible
        {
            get { return _isVisible; }
            private set
            {
                if (_isVisible == value)
                    return;
                RaisePropertyChanging("IsVisible");
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
                IsVisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public LayoutAnchorablePaneGroup RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (Equals(_rootPanel, value))
                    return;
                RaisePropertyChanging("RootPanel");

                if (_rootPanel != null)
                    _rootPanel.ChildrenTreeChanged -= _rootPanel_ChildrenTreeChanged;

                _rootPanel = value;
                if (_rootPanel != null)
                    _rootPanel.Parent = this;

                if (_rootPanel != null)
                    _rootPanel.ChildrenTreeChanged += _rootPanel_ChildrenTreeChanged;

                RaisePropertyChanged("RootPanel");
                RaisePropertyChanged("IsSinglePane");
                RaisePropertyChanged("SinglePane");
                RaisePropertyChanged("Children");
                RaisePropertyChanged("ChildrenCount");
                ((ILayoutElementWithVisibility) this).ComputeVisibility();
            }
        }

#if TRACE
        public override void ConsoleDump(int tab)
        {
            Trace.Write(new string(' ', tab*4));
            Trace.WriteLine("FloatingAnchorableWindow()");

            RootPanel.ConsoleDump(tab + 1);
        }
#endif

        public override void RemoveChild(ILayoutElement element)
        {
            Debug.Assert(Equals(element, RootPanel) && element != null);
            RootPanel = null;
        }

        public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            Debug.Assert(Equals(oldElement, RootPanel) && oldElement != null);
            RootPanel = newElement as LayoutAnchorablePaneGroup;
        }

        protected virtual void XmlDeserializeElement(XmlReader xmlReader)
        {
            if (xmlReader.IsEmptyElement)
                return;
            if (xmlReader.Name != "RootPanel")
                return;
            var layoutAnchorablePaneGroup = new LayoutAnchorablePaneGroup();
            layoutAnchorablePaneGroup.XmlDeserialize(xmlReader);
            RootPanel = layoutAnchorablePaneGroup;
        }

        private void _rootPanel_ChildrenTreeChanged(object sender, ChildrenTreeChangedEventArgs e)
        {
            RaisePropertyChanged("IsSinglePane");
            RaisePropertyChanged("SinglePane");
        }
    }
}