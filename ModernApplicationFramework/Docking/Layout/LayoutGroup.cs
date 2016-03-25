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
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    public abstract class LayoutGroup<T> : LayoutGroupBase, ILayoutGroup, IXmlSerializable
        where T : class, ILayoutElement
    {
        private readonly ObservableCollection<T> _children = new ObservableCollection<T>();
        private bool _isVisible = true;

        internal LayoutGroup()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        IEnumerable<ILayoutElement> ILayoutContainer.Children => _children;
        public int ChildrenCount => _children.Count;

        public void RemoveChild(ILayoutElement element)
        {
            _children.Remove((T) element);
        }

        public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            int index = _children.IndexOf((T) oldElement);
            _children.Insert(index, (T) newElement);
            _children.RemoveAt(index + 1);
        }

        public int IndexOfChild(ILayoutElement element)
        {
            return _children.Cast<ILayoutElement>().ToList().IndexOf(element);
        }

        public void InsertChildAt(int index, ILayoutElement element)
        {
            _children.Insert(index, (T) element);
        }

        public void RemoveChildAt(int childIndex)
        {
            _children.RemoveAt(childIndex);
        }

        public void ReplaceChildAt(int index, ILayoutElement element)
        {
            _children[index] = (T) element;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                ComputeVisibility();
                return;
            }
            string localName = reader.LocalName;
            reader.Read();
            while (true)
            {
                if (reader.LocalName == localName &&
                    reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }

                XmlSerializer serializer = null;
                if (reader.LocalName == "LayoutAnchorablePaneGroup")
                    serializer = new XmlSerializer(typeof (LayoutAnchorablePaneGroup));
                else if (reader.LocalName == "LayoutAnchorablePane")
                    serializer = new XmlSerializer(typeof (LayoutAnchorablePane));
                else if (reader.LocalName == "LayoutAnchorable")
                    serializer = new XmlSerializer(typeof (LayoutAnchorable));
                else if (reader.LocalName == "LayoutDocumentPaneGroup")
                    serializer = new XmlSerializer(typeof (LayoutDocumentPaneGroup));
                else if (reader.LocalName == "LayoutDocumentPane")
                    serializer = new XmlSerializer(typeof (LayoutDocumentPane));
                else if (reader.LocalName == "LayoutDocument")
                    serializer = new XmlSerializer(typeof (LayoutDocument));
                else if (reader.LocalName == "LayoutAnchorGroup")
                    serializer = new XmlSerializer(typeof (LayoutAnchorGroup));
                else if (reader.LocalName == "LayoutPanel")
                    serializer = new XmlSerializer(typeof (LayoutPanel));

                if (serializer != null)
                    Children.Add((T) serializer.Deserialize(reader));
            }

            reader.ReadEndElement();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            foreach (var child in Children)
            {
                var type = child.GetType();
                XmlSerializer serializer = new XmlSerializer(type);
                serializer.Serialize(writer, child);
            }
        }

        public ObservableCollection<T> Children => _children;

        public bool IsVisible
        {
            get { return _isVisible; }
            protected set
            {
                if (_isVisible == value)
                    return;
                RaisePropertyChanging("IsVisible");
                _isVisible = value;
                OnIsVisibleChanged();
                RaisePropertyChanged("IsVisible");
            }
        }

        public void ComputeVisibility()
        {
            IsVisible = GetVisibility();
        }

        public void MoveChild(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;
            _children.Move(oldIndex, newIndex);
            ChildMoved(oldIndex, newIndex);
        }

        protected virtual void ChildMoved(int oldIndex, int newIndex)
        {
        }

        protected abstract bool GetVisibility();

        protected virtual void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            base.OnParentChanged(oldValue, newValue);

            ComputeVisibility();
        }

        protected virtual void SetXmlAttributeValue(string name, string valueString)
        {
        }

        internal void XmlDeserialize(XmlReader xmlReader)
        {
            bool isEmptyElement = xmlReader.IsEmptyElement;
            if (xmlReader.HasAttributes)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    string name = xmlReader.Name;
                    string valueString = xmlReader.Value;

                    MethodInfo methodInfo = GetType()
                        .GetMethod("SetXmlAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
                    methodInfo.Invoke(this, new object[]
                    {
                        name, valueString
                    });
                }
                xmlReader.MoveToElement();
            }

            if (!isEmptyElement)
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name == "LayoutDocumentPaneGroup")
                        {
                            LayoutDocumentPaneGroup layoutDocumentPaneGroup = new LayoutDocumentPaneGroup();
                            layoutDocumentPaneGroup.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutDocumentPaneGroup);
                        }
                        else if (xmlReader.Name == "LayoutDocumentPane")
                        {
                            LayoutDocumentPane layoutDocumentPane = new LayoutDocumentPane();
                            layoutDocumentPane.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutDocumentPane);
                        }
                        else if (xmlReader.Name == "LayoutDocument")
                        {
                            LayoutDocument layoutDocument = new LayoutDocument();
                            layoutDocument.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutDocument);
                        }
                        else if (xmlReader.Name == "LayoutAnchorablePaneGroup")
                        {
                            LayoutAnchorablePaneGroup layoutAnchorablePaneGroup = new LayoutAnchorablePaneGroup();
                            layoutAnchorablePaneGroup.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutAnchorablePaneGroup);
                        }
                        else if (xmlReader.Name == "LayoutAnchorablePane")
                        {
                            LayoutAnchorablePane layoutAnchorablePane = new LayoutAnchorablePane();
                            layoutAnchorablePane.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutAnchorablePane);
                        }
                        else if (xmlReader.Name == "LayoutAnchorable")
                        {
                            LayoutAnchorable layoutAnchorable = new LayoutAnchorable();
                            layoutAnchorable.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutAnchorable);
                        }
                        else if (xmlReader.Name == "LayoutAnchorGroup")
                        {
                            LayoutAnchorGroup layoutAnchorGroup = new LayoutAnchorGroup();
                            layoutAnchorGroup.XmlDeserialize(xmlReader);
                            Children.Add((T) (object) layoutAnchorGroup);
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                }
            }
        }

        private void _children_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (
                        LayoutElement element in
                            e.OldItems.Cast<LayoutElement>().Where(element => Equals(element.Parent, this)))
                    {
                        element.Parent = null;
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (
                        LayoutElement element in
                            e.NewItems.Cast<LayoutElement>().Where(element => !Equals(element.Parent, this)))
                    {
                        element.Parent?.RemoveChild(element);
                        element.Parent = this;
                    }
                }
            }

            ComputeVisibility();
            OnChildrenCollectionChanged();
            NotifyChildrenTreeChanged(ChildrenTreeChange.DirectChildrenChanged);
            RaisePropertyChanged("ChildrenCount");
        }

        private void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            parentPane?.ComputeVisibility();
        }
    }
}