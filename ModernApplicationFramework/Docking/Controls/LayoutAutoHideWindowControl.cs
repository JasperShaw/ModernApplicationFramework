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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	public class LayoutAutoHideWindowControl : HwndHost, ILayoutControl
	{
		public static readonly DependencyProperty AnchorableStyleProperty =
			DependencyProperty.Register("AnchorableStyle", typeof (Style), typeof (LayoutAutoHideWindowControl),
				new FrameworkPropertyMetadata((Style) null));

		public static readonly DependencyProperty BackgroundProperty =
			DependencyProperty.Register("Background", typeof (Brush), typeof (LayoutAutoHideWindowControl),
				new FrameworkPropertyMetadata((Brush) null));

		private readonly ContentPresenter _internalHostPresenter = new ContentPresenter();
		private LayoutAnchorControl _anchor;
		private Vector _initialStartPoint;
		private Grid _internalGrid;
		private LayoutAnchorableControl _internalHost;
		private bool _internalHostContentRendered;
		private HwndSource _internalHwndSource;
		private DockingManager _manager;
		private LayoutAnchorable _model;
		private LayoutGridResizerControl _resizer;
		private Border _resizerGhost;
		private Window _resizerWindowHost;
		private AnchorSide _side;

		internal LayoutAutoHideWindowControl()
		{
		}

		static LayoutAutoHideWindowControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl),
				new FrameworkPropertyMetadata(typeof (LayoutAutoHideWindowControl)));
			FocusableProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl),
				new FrameworkPropertyMetadata(true));
			Control.IsTabStopProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl), new FrameworkPropertyMetadata(true));
			VisibilityProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl),
				new FrameworkPropertyMetadata(Visibility.Hidden));
		}

		public ILayoutElement Model => _model;

		public Style AnchorableStyle
		{
			get { return (Style) GetValue(AnchorableStyleProperty); }
			set { SetValue(AnchorableStyleProperty, value); }
		}

		public Brush Background
		{
			get { return (Brush) GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		protected override System.Collections.IEnumerator LogicalChildren
			=>
				_internalHostPresenter == null
					? new UIElement[] {}.GetEnumerator()
					: new UIElement[] {_internalHostPresenter}.GetEnumerator();

		internal bool IsWin32MouseOver
		{
			get
			{
				var ptMouse = new Win32Helper.Win32Point();
				if (!Win32Helper.GetCursorPos(ref ptMouse))
					return false;

				this.PointToScreenDpi(new Point());

				Rect rectWindow = this.GetScreenArea();
				if (rectWindow.Contains(new Point(ptMouse.X, ptMouse.Y)))
					return true;

				var manager = Model.Root.Manager;
				var anchor = manager.FindVisualChildren<LayoutAnchorControl>().FirstOrDefault(c => c.Model == Model);

				if (anchor == null)
					return false;

				anchor.PointToScreenDpi(new Point());

				if (anchor.IsMouseOver)
					return true;

				return false;
			}
		}

		internal bool IsResizing { get; private set; }

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (_internalHostPresenter == null)
				return base.ArrangeOverride(finalSize);

			_internalHostPresenter.Arrange(new Rect(finalSize));
			return base.ArrangeOverride(finalSize);
			// new Size(_internalHostPresenter.ActualWidth, _internalHostPresenter.ActualHeight);
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			_internalHwndSource = new HwndSource(new HwndSourceParameters()
			{
				ParentWindow = hwndParent.Handle,
				WindowStyle = Win32Helper.WsChild | Win32Helper.WsVisible | Win32Helper.WsClipsiblings | Win32Helper.WsClipchildren,
				Width = 0,
				Height = 0,
			});

			_internalHostContentRendered = false;
			_internalHwndSource.ContentRendered += _internalHwndSource_ContentRendered;
			_internalHwndSource.RootVisual = _internalHostPresenter;
			AddLogicalChild(_internalHostPresenter);
			Win32Helper.BringWindowToTop(_internalHwndSource.Handle);
			return new HandleRef(this, _internalHwndSource.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			if (_internalHwndSource != null)
			{
				_internalHwndSource.ContentRendered -= _internalHwndSource_ContentRendered;
				_internalHwndSource.Dispose();
				_internalHwndSource = null;
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (_internalHostPresenter == null)
				return base.MeasureOverride(constraint);

			_internalHostPresenter.Measure(constraint);
			//return base.MeasureOverride(constraint);
			return _internalHostPresenter.DesiredSize;
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == Win32Helper.WmWindowposchanging)
			{
				if (_internalHostContentRendered)
					Win32Helper.SetWindowPos(_internalHwndSource.Handle, Win32Helper.HwndTop, 0, 0, 0, 0,
						Win32Helper.SetWindowPosFlags.IgnoreMove | Win32Helper.SetWindowPosFlags.IgnoreResize);
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		internal void Hide()
		{
			if (_model == null)
				return;

			_model.PropertyChanged -= _model_PropertyChanged;

			RemoveInternalGrid();
			_anchor = null;
			_model = null;
			_manager = null;
			Visibility = Visibility.Hidden;

			Trace.WriteLine("LayoutAutoHideWindowControl.Hide()");
		}

		internal void Show(LayoutAnchorControl anchor)
		{
			if (_model != null)
				throw new InvalidOperationException();

			_anchor = anchor;
			_model = anchor.Model as LayoutAnchorable;
			var layoutAnchorSide = anchor.Model.Parent.Parent as LayoutAnchorSide;
			if (layoutAnchorSide != null)
				_side = layoutAnchorSide.Side;
			if (_model != null)
			{
				_manager = _model.Root.Manager;
				CreateInternalGrid();

				_model.PropertyChanged += _model_PropertyChanged;
			}

			Visibility = Visibility.Visible;
			InvalidateMeasure();
			UpdateLayout();
			Trace.WriteLine("LayoutAutoHideWindowControl.Show()");
		}

		private void _internalHwndSource_ContentRendered(object sender, EventArgs e)
		{
			_internalHostContentRendered = true;
		}

		private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsAutoHidden")
			{
				if (!_model.IsAutoHidden)
				{
					_manager.HideAutoHideWindow(_anchor);
				}
			}
		}

		private void CreateInternalGrid()
		{
			_internalGrid = new Grid {FlowDirection = FlowDirection.LeftToRight};
			_internalGrid.SetBinding(Panel.BackgroundProperty, new Binding("Background") {Source = this});


			_internalHost = new LayoutAnchorableControl {Model = _model, Style = AnchorableStyle};
			_internalHost.SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection") {Source = this});

			KeyboardNavigation.SetTabNavigation(_internalGrid, KeyboardNavigationMode.Cycle);

			_resizer = new LayoutGridResizerControl();

			_resizer.DragStarted += OnResizerDragStarted;
			_resizer.DragDelta += OnResizerDragDelta;
			_resizer.DragCompleted += OnResizerDragCompleted;

			if (_side == AnchorSide.Right)
			{
				_internalGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(_manager.GridSplitterWidth)});
				_internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width =
						_model.AutoHideWidth == 0.0
							? new GridLength(_model.AutoHideMinWidth)
							: new GridLength(_model.AutoHideWidth, GridUnitType.Pixel)
				});

				Grid.SetColumn(_resizer, 0);
				Grid.SetColumn(_internalHost, 1);

				_resizer.Cursor = Cursors.SizeWE;

				HorizontalAlignment = HorizontalAlignment.Right;
				VerticalAlignment = VerticalAlignment.Stretch;
			}
			else if (_side == AnchorSide.Left)
			{
				_internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
				{
					Width =
						_model.AutoHideWidth == 0.0
							? new GridLength(_model.AutoHideMinWidth)
							: new GridLength(_model.AutoHideWidth, GridUnitType.Pixel),
				});
				_internalGrid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(_manager.GridSplitterWidth)});

				Grid.SetColumn(_internalHost, 0);
				Grid.SetColumn(_resizer, 1);

				_resizer.Cursor = Cursors.SizeWE;

				HorizontalAlignment = HorizontalAlignment.Left;
				VerticalAlignment = VerticalAlignment.Stretch;
			}
			else if (_side == AnchorSide.Top)
			{
				_internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height =
						_model.AutoHideHeight == 0.0
							? new GridLength(_model.AutoHideMinHeight)
							: new GridLength(_model.AutoHideHeight, GridUnitType.Pixel),
				});
				_internalGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(_manager.GridSplitterHeight)});

				Grid.SetRow(_internalHost, 0);
				Grid.SetRow(_resizer, 1);

				_resizer.Cursor = Cursors.SizeNS;

				VerticalAlignment = VerticalAlignment.Top;
				HorizontalAlignment = HorizontalAlignment.Stretch;
			}
			else if (_side == AnchorSide.Bottom)
			{
				_internalGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(_manager.GridSplitterHeight)});
				_internalGrid.RowDefinitions.Add(new RowDefinition()
				{
					Height =
						_model.AutoHideHeight == 0.0
							? new GridLength(_model.AutoHideMinHeight)
							: new GridLength(_model.AutoHideHeight, GridUnitType.Pixel),
				});

				Grid.SetRow(_resizer, 0);
				Grid.SetRow(_internalHost, 1);

				_resizer.Cursor = Cursors.SizeNS;

				VerticalAlignment = VerticalAlignment.Bottom;
				HorizontalAlignment = HorizontalAlignment.Stretch;
			}


			_internalGrid.Children.Add(_resizer);
			_internalGrid.Children.Add(_internalHost);
			_internalHostPresenter.Content = _internalGrid;
		}

		private void HideResizerOverlayWindow()
		{
			if (_resizerWindowHost != null)
			{
				_resizerWindowHost.Close();
				_resizerWindowHost = null;
			}
		}

		private void OnResizerDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			var rootVisual = this.FindVisualTreeRoot() as Visual;

			if (rootVisual != null)
			{
				var trToWnd = TransformToAncestor(rootVisual);
				Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
				                          trToWnd.Transform(new Point());
			}

			double delta;
			if (_side == AnchorSide.Right || _side == AnchorSide.Left)
				delta = Canvas.GetLeft(_resizerGhost) - _initialStartPoint.X;
			else
				delta = Canvas.GetTop(_resizerGhost) - _initialStartPoint.Y;

			if (_side == AnchorSide.Right)
			{
				if (_model.AutoHideWidth == 0.0)
					_model.AutoHideWidth = _internalHost.ActualWidth - delta;
				else
					_model.AutoHideWidth -= delta;

				_internalGrid.ColumnDefinitions[1].Width = new GridLength(_model.AutoHideWidth, GridUnitType.Pixel);
			}
			else if (_side == AnchorSide.Left)
			{
				if (_model.AutoHideWidth == 0.0)
					_model.AutoHideWidth = _internalHost.ActualWidth + delta;
				else
					_model.AutoHideWidth += delta;

				_internalGrid.ColumnDefinitions[0].Width = new GridLength(_model.AutoHideWidth, GridUnitType.Pixel);
			}
			else if (_side == AnchorSide.Top)
			{
				if (_model.AutoHideHeight == 0.0)
					_model.AutoHideHeight = _internalHost.ActualHeight + delta;
				else
					_model.AutoHideHeight += delta;

				_internalGrid.RowDefinitions[0].Height = new GridLength(_model.AutoHideHeight, GridUnitType.Pixel);
			}
			else if (_side == AnchorSide.Bottom)
			{
				if (_model.AutoHideHeight == 0.0)
					_model.AutoHideHeight = _internalHost.ActualHeight - delta;
				else
					_model.AutoHideHeight -= delta;

				_internalGrid.RowDefinitions[1].Height = new GridLength(_model.AutoHideHeight, GridUnitType.Pixel);
			}

			HideResizerOverlayWindow();

			IsResizing = false;
			InvalidateMeasure();
		}

		private void OnResizerDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
		{
			var rootVisual = this.FindVisualTreeRoot() as Visual;

			if (rootVisual == null)
				return;
			var trToWnd = TransformToAncestor(rootVisual);
			Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
			                          trToWnd.Transform(new Point());

			if (_side == AnchorSide.Right || _side == AnchorSide.Left)
			{
				if (GetFlowDirection(_internalHost) == FlowDirection.RightToLeft)
					transformedDelta.X = -transformedDelta.X;
				Canvas.SetLeft(_resizerGhost,
					MathHelper.MinMax(_initialStartPoint.X + transformedDelta.X, 0.0, _resizerWindowHost.Width - _resizerGhost.Width));
			}
			else
			{
				Canvas.SetTop(_resizerGhost,
					MathHelper.MinMax(_initialStartPoint.Y + transformedDelta.Y, 0.0, _resizerWindowHost.Height - _resizerGhost.Height));
			}
		}

		private void OnResizerDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			var resizer = sender as LayoutGridResizerControl;
			ShowResizerOverlayWindow(resizer);
			IsResizing = true;
		}

		private void RemoveInternalGrid()
		{
			_resizer.DragStarted -= OnResizerDragStarted;
			_resizer.DragDelta -= OnResizerDragDelta;
			_resizer.DragCompleted -= OnResizerDragCompleted;

			_internalHostPresenter.Content = null;
		}

		private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
		{
			_resizerGhost = new Border()
			{
				Background = splitter.BackgroundWhileDragging,
				Opacity = splitter.OpacityWhileDragging
			};

			var areaElement = _manager.GetAutoHideAreaElement();
			_internalHost.TransformActualSizeToAncestor();

			Point ptTopLeftScreen = areaElement.PointToScreenDpiWithoutFlowDirection(new Point());

			var managerSize = areaElement.TransformActualSizeToAncestor();

			Size windowSize;

			if (_side == AnchorSide.Right || _side == AnchorSide.Left)
			{
				windowSize = new Size(
					managerSize.Width - 25.0 + splitter.ActualWidth,
					managerSize.Height);

				_resizerGhost.Width = splitter.ActualWidth;
				_resizerGhost.Height = windowSize.Height;
				ptTopLeftScreen.Offset(25, 0.0);
			}
			else
			{
				windowSize = new Size(
					managerSize.Width,
					managerSize.Height - _model.AutoHideMinHeight - 25.0 + splitter.ActualHeight);

				_resizerGhost.Height = splitter.ActualHeight;
				_resizerGhost.Width = windowSize.Width;
				ptTopLeftScreen.Offset(0.0, 25.0);
			}

			_initialStartPoint = splitter.PointToScreenDpiWithoutFlowDirection(new Point()) - ptTopLeftScreen;

			if (_side == AnchorSide.Right || _side == AnchorSide.Left)
			{
				Canvas.SetLeft(_resizerGhost, _initialStartPoint.X);
			}
			else
			{
				Canvas.SetTop(_resizerGhost, _initialStartPoint.Y);
			}

			Canvas panelHostResizer = new Canvas()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			panelHostResizer.Children.Add(_resizerGhost);


			_resizerWindowHost = new Window()
			{
				ResizeMode = ResizeMode.NoResize,
				WindowStyle = WindowStyle.None,
				ShowInTaskbar = false,
				AllowsTransparency = true,
				Background = null,
				Width = windowSize.Width,
				Height = windowSize.Height,
				Left = ptTopLeftScreen.X,
				Top = ptTopLeftScreen.Y,
				ShowActivated = false,
				Owner = Window.GetWindow(this),
				Content = panelHostResizer
			};

			_resizerWindowHost.Show();
		}
	}
}