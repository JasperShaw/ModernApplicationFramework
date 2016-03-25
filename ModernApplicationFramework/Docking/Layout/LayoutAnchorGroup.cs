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
using System.Windows.Markup;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorGroup : LayoutGroup<LayoutAnchorable>, ILayoutPreviousContainer, ILayoutPaneSerializable
    {
        private string _id;
        [field: NonSerialized] private ILayoutContainer _previousContainer;

        string ILayoutPaneSerializable.Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlIgnore]
        ILayoutContainer ILayoutPreviousContainer.PreviousContainer
        {
            get { return _previousContainer; }
            set
            {
                if (_previousContainer == value)
                    return;
                _previousContainer = value;
                RaisePropertyChanged("PreviousContainer");
                var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                if (paneSerializable != null &&
                    paneSerializable.Id == null)
                    paneSerializable.Id = Guid.NewGuid().ToString();
            }
        }

        string ILayoutPreviousContainer.PreviousContainerId { get; set; }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
                _id = reader.Value;
            if (reader.MoveToAttribute("PreviousContainerId"))
                ((ILayoutPreviousContainer) this).PreviousContainerId = reader.Value;
            base.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (_id != null)
                writer.WriteAttributeString("Id", _id);
            var paneSerializable = _previousContainer as ILayoutPaneSerializable;
            if (paneSerializable != null)
            {
                writer.WriteAttributeString("PreviousContainerId", paneSerializable.Id);
            }

            base.WriteXml(writer);
        }

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }

        protected override void SetXmlAttributeValue(string name, string valueString)
        {
            switch (name)
            {
                case "Id":
                    _id = valueString;
                    break;
                case "PreviousContainerId":
                    ((ILayoutPreviousContainer) this).PreviousContainerId = valueString;
                    break;
                default:
                    base.SetXmlAttributeValue(name, valueString);
                    break;
            }
        }
    }
}