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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ModernApplicationFramework.Docking.Layout
{
    [Serializable]
    [XmlInclude(typeof(LayoutAnchorableFloatingWindow))]
    [XmlInclude(typeof(LayoutDocumentFloatingWindow))]
    public abstract class LayoutFloatingWindow : LayoutElement, ILayoutContainer
    {
	    public abstract IEnumerable<ILayoutElement> Children { get; }

        public abstract void RemoveChild(ILayoutElement element);

        public abstract void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

        public abstract int ChildrenCount { get; }

        public abstract bool IsValid { get; }

        protected virtual void SetXmlAttributeValue(string name, string valueString)
		{
			switch (name)
			{
				case "Title":
					//Title = valueString;
					break;
				
			}
		}

		protected virtual void XmlDeserializeElement(XmlReader xmlReader)
		{
		}
 
		internal void XmlDeserialize(XmlReader xmlReader)
		{
			bool isEmptyElement = xmlReader.IsEmptyElement;
			if (xmlReader.HasAttributes)
			{
				xmlReader.MoveToElement();
				while (xmlReader.MoveToNextAttribute())
				{
					string name = xmlReader.Name;
					string valueString = xmlReader.Value;

					MethodInfo methodInfo = GetType().GetMethod("SetXmlAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
					methodInfo.Invoke(this, new object[] { name, valueString });
				}
			}

			if (!isEmptyElement)
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						MethodInfo methodInfo = GetType().GetMethod("XmlDeserializeElement", BindingFlags.Instance | BindingFlags.NonPublic);
						methodInfo.Invoke(this, new object[] { xmlReader });
					}
					else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "LayoutFloatingWindow")
					{
						break;
					}
				}
			}
		}
    }
}
