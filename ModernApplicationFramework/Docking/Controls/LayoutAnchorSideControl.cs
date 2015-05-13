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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	public class LayoutAnchorSideControl : Control, ILayoutControl
	{
		private readonly ObservableCollection<LayoutAnchorGroupControl> _childViews =
			new ObservableCollection<LayoutAnchorGroupControl>();

		private readonly LayoutAnchorSide _model;

		internal LayoutAnchorSideControl(LayoutAnchorSide model)
		{
			if (model == null)
				throw new ArgumentNullException("model");


			_model = model;

			CreateChildrenViews();

			_model.Children.CollectionChanged += (s, e) => OnModelChildrenCollectionChanged(e);

			UpdateSide();
		}

		static LayoutAnchorSideControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorSideControl),
				new FrameworkPropertyMetadata(typeof (LayoutAnchorSideControl)));
		}

		public ILayoutElement Model => _model;
		public ObservableCollection<LayoutAnchorGroupControl> Children => _childViews;
		public bool IsBottomSide => (bool) GetValue(IsBottomSideProperty);
		public bool IsLeftSide => (bool) GetValue(IsLeftSideProperty);
		public bool IsRightSide => (bool) GetValue(IsRightSideProperty);
		public bool IsTopSide => (bool) GetValue(IsTopSideProperty);

		protected void SetIsBottomSide(bool value)
		{
			SetValue(IsBottomSidePropertyKey, value);
		}

		protected void SetIsLeftSide(bool value)
		{
			SetValue(IsLeftSidePropertyKey, value);
		}

		protected void SetIsRightSide(bool value)
		{
			SetValue(IsRightSidePropertyKey, value);
		}

		protected void SetIsTopSide(bool value)
		{
			SetValue(IsTopSidePropertyKey, value);
		}

		private void CreateChildrenViews()
		{
			var manager = _model.Root.Manager;
			foreach (var childModel in _model.Children)
			{
				_childViews.Add(manager.CreateUIElementForModel(childModel) as LayoutAnchorGroupControl);
			}
		}

		private void OnModelChildrenCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null &&
			    (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
			     e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
			{
				foreach (var childModel in e.OldItems)
					_childViews.Remove(_childViews.First(cv => cv.Model == childModel));
			}

			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
				_childViews.Clear();

			if (e.NewItems != null &&
			    (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
			     e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
			{
				var manager = _model.Root.Manager;
				int insertIndex = e.NewStartingIndex;
				foreach (LayoutAnchorGroup childModel in e.NewItems)
				{
					_childViews.Insert(insertIndex++, manager.CreateUIElementForModel(childModel) as LayoutAnchorGroupControl);
				}
			}
		}

		private void UpdateSide()
		{
			switch (_model.Side)
			{
				case AnchorSide.Left:
					SetIsLeftSide(true);
					break;
				case AnchorSide.Top:
					SetIsTopSide(true);
					break;
				case AnchorSide.Right:
					SetIsRightSide(true);
					break;
				case AnchorSide.Bottom:
					SetIsBottomSide(true);
					break;
			}
		}

		private static readonly DependencyPropertyKey IsLeftSidePropertyKey
			= DependencyProperty.RegisterReadOnly("IsLeftSide", typeof (bool), typeof (LayoutAnchorSideControl),
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsLeftSideProperty
			= IsLeftSidePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsTopSidePropertyKey
			= DependencyProperty.RegisterReadOnly("IsTopSide", typeof (bool), typeof (LayoutAnchorSideControl),
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsTopSideProperty
			= IsTopSidePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsRightSidePropertyKey
			= DependencyProperty.RegisterReadOnly("IsRightSide", typeof (bool), typeof (LayoutAnchorSideControl),
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsRightSideProperty
			= IsRightSidePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsBottomSidePropertyKey
			= DependencyProperty.RegisterReadOnly("IsBottomSide", typeof (bool), typeof (LayoutAnchorSideControl),
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsBottomSideProperty
			= IsBottomSidePropertyKey.DependencyProperty;
	}
}