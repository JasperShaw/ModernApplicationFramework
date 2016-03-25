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
using System.Linq;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorablePane : LayoutPositionableGroup<LayoutAnchorable>, ILayoutAnchorablePane,
        ILayoutContentSelector, ILayoutPaneSerializable
    {
        [XmlIgnore] private bool _autoFixSelectedContent = true;
        private string _id;

        public LayoutAnchorablePane()
        {
        }

        public LayoutAnchorablePane(LayoutAnchorable anchorable)
        {
            Children.Add(anchorable);
        }

        public int IndexOf(LayoutContent content)
        {
            var anchorableChild = content as LayoutAnchorable;
            if (anchorableChild == null)
                return -1;

            return Children.IndexOf(anchorableChild);
        }

        string ILayoutPaneSerializable.Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool CanClose
        {
            get { return Children.All(a => a.CanClose); }
        }

        public bool CanHide
        {
            get { return Children.All(a => a.CanHide); }
        }

        public bool IsDirectlyHostedInFloatingWindow
        {
            get
            {
                var parentFloatingWindow = this.FindParent<LayoutAnchorableFloatingWindow>();
                return parentFloatingWindow != null && parentFloatingWindow.IsSinglePane;

                //return Parent != null && Parent.ChildrenCount == 1 && Parent.Parent is LayoutFloatingWindow;
            }
        }

        public bool IsHostedInFloatingWindow => this.FindParent<LayoutFloatingWindow>() != null;
#if TRACE
        public override void ConsoleDump(int tab)
        {
            System.Diagnostics.Trace.Write(new string(' ', tab*4));
            System.Diagnostics.Trace.WriteLine("AnchorablePane()");

            foreach (LayoutAnchorable child in Children)
                child.ConsoleDump(tab + 1);
        }
#endif

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
                _id = reader.Value;
            if (reader.MoveToAttribute("Name"))
                _name = reader.Value;

            _autoFixSelectedContent = false;
            base.ReadXml(reader);
            _autoFixSelectedContent = true;
            AutoFixSelectedContent();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (_id != null)
                writer.WriteAttributeString("Id", _id);
            if (_name != null)
                writer.WriteAttributeString("Name", _name);

            base.WriteXml(writer);
        }

        protected override bool GetVisibility()
        {
            return Children.Count > 0 && Children.Any(c => c.IsVisible);
        }

        protected override void OnChildrenCollectionChanged()
        {
            AutoFixSelectedContent();
            for (int i = 0; i < Children.Count; i++)
            {
                if (!Children[i].IsSelected)
                    continue;
                SelectedContentIndex = i;
                break;
            }

            RaisePropertyChanged("CanClose");
            RaisePropertyChanged("CanHide");
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
            base.OnChildrenCollectionChanged();
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            var oldGroup = oldValue as ILayoutGroup;
            if (oldGroup != null)
                oldGroup.ChildrenCollectionChanged -= OnParentChildrenCollectionChanged;

            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");

            var newGroup = newValue as ILayoutGroup;
            if (newGroup != null)
                newGroup.ChildrenCollectionChanged += OnParentChildrenCollectionChanged;

            base.OnParentChanged(oldValue, newValue);
        }

        protected override void SetXmlAttributeValue(string name, string valueString)
        {
            switch (name)
            {
                case "Id":
                    _id = valueString;
                    break;
                case "Name":
                    _name = valueString;
                    break;
                default:
                    base.SetXmlAttributeValue(name, valueString);
                    break;
            }
        }

        internal void UpdateIsDirectlyHostedInFloatingWindow()
        {
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
        }

        private void AutoFixSelectedContent()
        {
            if (!_autoFixSelectedContent)
                return;
            if (SelectedContentIndex >= ChildrenCount)
                SelectedContentIndex = Children.Count - 1;

            if (SelectedContentIndex == -1 && ChildrenCount > 0)
                SelectedContentIndex = 0;
        }

        private void OnParentChildrenCollectionChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
        }

        #region SelectedContentIndex

        private int _selectedIndex = -1;

        public int SelectedContentIndex
        {
            get { return _selectedIndex; }
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

        public LayoutContent SelectedContent => _selectedIndex == -1 ? null : Children[_selectedIndex];

        #endregion

        #region Name

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        #endregion
    }
}