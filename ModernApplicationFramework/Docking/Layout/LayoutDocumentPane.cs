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
using System.Linq;
using System.Windows.Markup;

namespace ModernApplicationFramework.Docking.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutDocumentPane : LayoutPositionableGroup<LayoutContent>, ILayoutDocumentPane, ILayoutContentSelector,
		ILayoutPaneSerializable
	{
		private string _id;
		private int _selectedIndex = -1;

		public LayoutDocumentPane()
		{
		}

		public LayoutDocumentPane(LayoutContent firstChild)
		{
			Children.Add(firstChild);
		}

		public int IndexOf(LayoutContent content)
		{
			return Children.IndexOf(content);
		}

		public LayoutContent SelectedContent => _selectedIndex == -1 ? null : Children[_selectedIndex];

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

		string ILayoutPaneSerializable.Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IEnumerable<LayoutContent> ChildrenSorted
		{
			get
			{
				var listSorted = Children.ToList();
				listSorted.Sort();
				return listSorted;
			}
		}

#if TRACE
		public override void ConsoleDump(int tab)
		{
			System.Diagnostics.Trace.Write(new string(' ', tab*4));
			System.Diagnostics.Trace.WriteLine("DocumentPane()");

			foreach (LayoutContent child in Children)
				child.ConsoleDump(tab + 1);
		}
#endif

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			if (reader.MoveToAttribute("Id"))
				_id = reader.Value;


			base.ReadXml(reader);
		}

		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			if (_id != null)
				writer.WriteAttributeString("Id", _id);

			base.WriteXml(writer);
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
			{
				if (Root == null) //if I'm not yet connected just switch to first document
					SelectedContentIndex = 0;
				else
				{
					var childrenToSelect = Children.OrderByDescending(c => c.LastActivationTimeStamp.GetValueOrDefault()).First();
					SelectedContentIndex = Children.IndexOf(childrenToSelect);
					childrenToSelect.IsActive = true;
				}
			}

			base.OnChildrenCollectionChanged();

			RaisePropertyChanged("ChildrenSorted");
		}

		protected override void OnIsVisibleChanged()
		{
			UpdateParentVisibility();
			base.OnIsVisibleChanged();
		}

		private void UpdateParentVisibility()
		{
			var parentPane = Parent as ILayoutElementWithVisibility;
			parentPane?.ComputeVisibility();
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
	}
}