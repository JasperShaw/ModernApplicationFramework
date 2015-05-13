/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	public class LayoutAnchorableTabItem : Control
	{
		private bool _isMouseDown;

		static LayoutAnchorableTabItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableTabItem),
				new FrameworkPropertyMetadata(typeof (LayoutAnchorableTabItem)));
		}

		public LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

		public LayoutContent Model
		{
			get { return (LayoutContent) GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			SetLayoutItem(Model?.Root.Manager.GetLayoutItemFromModel(Model));
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			if (_draggingItem == null || Equals(_draggingItem, this) || e.LeftButton != MouseButtonState.Pressed)
				return;
			//Trace.WriteLine("Dragging item from {0} to {1}", _draggingItem, this);

			var model = Model;
			var container = model.Parent;
			var containerPane = model.Parent as ILayoutPane;
			var childrenList = container.Children.ToList();
			containerPane?.MoveChild(childrenList.IndexOf(_draggingItem.Model), childrenList.IndexOf(model));
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
			{
				_draggingItem = this;
			}

			_isMouseDown = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			_isMouseDown = true;
			_draggingItem = this;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			_isMouseDown = false;

			base.OnMouseLeftButtonUp(e);

			Model.IsActive = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.LeftButton != MouseButtonState.Pressed)
			{
				_isMouseDown = false;
				_draggingItem = null;
			}
		}

		protected void SetLayoutItem(LayoutItem value)
		{
			SetValue(LayoutItemPropertyKey, value);
		}

		internal static LayoutAnchorableTabItem GetDraggingItem()
		{
			return _draggingItem;
		}

		internal static bool IsDraggingItem()
		{
			return _draggingItem != null;
		}

		internal static void ResetDraggingItem()
		{
			_draggingItem = null;
		}

		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutAnchorableTabItem) d).OnModelChanged(e);
		}

		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof (LayoutContent), typeof (LayoutAnchorableTabItem),
				new FrameworkPropertyMetadata(null, OnModelChanged));

		private static readonly DependencyPropertyKey LayoutItemPropertyKey
			= DependencyProperty.RegisterReadOnly("LayoutItem", typeof (LayoutItem), typeof (LayoutAnchorableTabItem),
				new FrameworkPropertyMetadata((LayoutItem) null));

		public static readonly DependencyProperty LayoutItemProperty
			= LayoutItemPropertyKey.DependencyProperty;

		private static LayoutAnchorableTabItem _draggingItem;
	}
}