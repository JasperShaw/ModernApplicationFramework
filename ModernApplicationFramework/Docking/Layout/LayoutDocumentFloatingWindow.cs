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
using System.Windows.Markup;
using System.Xml;

namespace ModernApplicationFramework.Docking.Layout
{
	[ContentProperty("RootDocument")]
	[Serializable]
	public class LayoutDocumentFloatingWindow : LayoutFloatingWindow
	{
		private LayoutDocument _rootDocument;
		public event EventHandler RootDocumentChanged;

		public override IEnumerable<ILayoutElement> Children
		{
			get
			{
				if (RootDocument == null)
					yield break;

				yield return RootDocument;
			}
		}

		public override int ChildrenCount => RootDocument != null ? 1 : 0;
		public override bool IsValid => RootDocument != null;

		public LayoutDocument RootDocument
		{
			get { return _rootDocument; }
			set
			{
				if (Equals(_rootDocument, value))
					return;
				RaisePropertyChanging("RootDocument");
				_rootDocument = value;
				if (_rootDocument != null)
					_rootDocument.Parent = this;
				RaisePropertyChanged("RootDocument");

				RootDocumentChanged?.Invoke(this, EventArgs.Empty);
			}
		}

#if TRACE
		public override void ConsoleDump(int tab)
		{
			Trace.Write(new string(' ', tab*4));
			Trace.WriteLine("FloatingDocumentWindow()");

			RootDocument.ConsoleDump(tab + 1);
		}
#endif

		public override void RemoveChild(ILayoutElement element)
		{
			Debug.Assert(Equals(element, RootDocument) && element != null);
			RootDocument = null;
		}

		public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
		{
			Debug.Assert(Equals(oldElement, RootDocument) && oldElement != null);
			RootDocument = newElement as LayoutDocument;
		}

	    protected virtual void XmlDeserializeElement(XmlReader xmlReader)
	    {
	        if (xmlReader.Name != "RootDocument")
                return;
	        LayoutDocument layoutDocument = new LayoutDocument();
	        layoutDocument.XmlDeserialize(xmlReader);
	        RootDocument = layoutDocument;
	    }
	}
}