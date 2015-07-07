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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking
{
	[ContentProperty("Layout")]
	[TemplatePart(Name = "PART_AutoHideArea")]
	public class DockingManager : Control, IOverlayWindowHost //, ILogicalChildrenContainer
	{
		public DockingManager()
		{
			Layout = new LayoutRoot
			{
				RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
			};
			Loaded += DockingManager_Loaded;
			Unloaded += DockingManager_Unloaded;
		}

		static DockingManager()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (DockingManager),
				new FrameworkPropertyMetadata(typeof (DockingManager)));
			FocusableProperty.OverrideMetadata(typeof (DockingManager), new FrameworkPropertyMetadata(false));
			HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			SetupAutoHideWindow();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			if (!Equals(Layout.Manager, this))
				return;
			LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
			LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
			TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
			RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
			BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Trace.WriteLine($"DockingManager.OnMouseLeftButtonDown([{e.GetPosition(this)}])");
			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			Trace.WriteLine($"DockingManager.OnPreviewGotKeyboardFocus({e.NewFocus})");
			base.OnPreviewGotKeyboardFocus(e);
		}

		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			Trace.WriteLine($"DockingManager.OnPreviewLostKeyboardFocus({e.OldFocus})");
			base.OnPreviewLostKeyboardFocus(e);
		}

		internal void _ExecuteAutoHideCommand(LayoutAnchorable anchorable)
		{
			anchorable.ToggleAutoHide();
		}

		internal void _ExecuteCloseAllButThisCommand(LayoutContent contentSelected)
		{
			foreach (
				var contentToClose in
					Layout.Descendents()
						.OfType<LayoutContent>()
						.Where(
							d =>
								!Equals(d, contentSelected) &&
								(d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow))
						.ToArray())
			{
				if (!contentToClose.CanClose)
					continue;

				var layoutItem = GetLayoutItemFromModel(contentToClose);
				if (layoutItem.CloseCommand != null)
				{
					if (layoutItem.CloseCommand.CanExecute(null))
						layoutItem.CloseCommand.Execute(null);
				}
				else
				{
					if (contentToClose is LayoutDocument)
						_ExecuteCloseCommand(contentToClose as LayoutDocument);
					else if (contentToClose is LayoutAnchorable)
						_ExecuteCloseCommand(contentToClose as LayoutAnchorable);
				}
			}
		}

		internal void _ExecuteCloseCommand(LayoutAnchorable anchorable)
		{
			var model = anchorable;
			if (model == null || !model.TestCanClose())
				return;
			if (model.IsAutoHidden)
				model.ToggleAutoHide();

			model.Close();
		}

		internal void _ExecuteContentActivateCommand(LayoutContent content)
		{
			content.IsActive = true;
		}

		internal void _ExecuteDockAsDocumentCommand(LayoutContent content)
		{
			content.DockAsDocument();
		}

		internal void _ExecuteDockCommand(LayoutAnchorable anchorable)
		{
			anchorable.Dock();
		}

		internal void _ExecuteFloatCommand(LayoutContent contentToFloat)
		{
			contentToFloat.Float();
		}

		internal void _ExecuteHideCommand(LayoutAnchorable anchorable)
		{
			var model = anchorable;
			//by default hide the anchorable
			model?.Hide();
		}

		// ReSharper disable once InconsistentNaming
		internal UIElement CreateUIElementForModel(ILayoutElement model)
		{
			var panel = model as LayoutPanel;
			if (panel != null)
				return new LayoutPanelControl(panel);
			var @group = model as LayoutAnchorablePaneGroup;
			if (@group != null)
				return new LayoutAnchorablePaneGroupControl(@group);
			var paneGroup = model as LayoutDocumentPaneGroup;
			if (paneGroup != null)
				return new LayoutDocumentPaneGroupControl(paneGroup);

			var side = model as LayoutAnchorSide;
			if (side != null)
			{
				var templateModelView = new LayoutAnchorSideControl(side);
				templateModelView.SetBinding(TemplateProperty, new Binding("AnchorSideTemplate") {Source = this});
				return templateModelView;
			}
			var anchorGroup = model as LayoutAnchorGroup;
			if (anchorGroup != null)
			{
				var templateModelView = new LayoutAnchorGroupControl(anchorGroup);
				templateModelView.SetBinding(TemplateProperty, new Binding("AnchorGroupTemplate") {Source = this});
				return templateModelView;
			}

			var documentPane = model as LayoutDocumentPane;
			if (documentPane != null)
			{
				var templateModelView = new LayoutDocumentPaneControl(documentPane);
				templateModelView.SetBinding(StyleProperty, new Binding("DocumentPaneControlStyle") {Source = this});
				return templateModelView;
			}
			var pane = model as LayoutAnchorablePane;
			if (pane != null)
			{
				var templateModelView = new LayoutAnchorablePaneControl(pane);
				templateModelView.SetBinding(StyleProperty, new Binding("AnchorablePaneControlStyle") {Source = this});
				return templateModelView;
			}

			var fw = model as LayoutAnchorableFloatingWindow;
			if (fw != null)
			{
				if (DesignerProperties.GetIsInDesignMode(this))
					return null;
				var modelFw = fw;
				var newFw = new LayoutAnchorableFloatingWindowControl(modelFw);
				newFw.SetParentToMainWindowOf(this);

				var paneForExtensions = modelFw.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault();
				if (paneForExtensions != null)
				{
					//ensure that floating window position is inside current (or nearest) monitor
					paneForExtensions.KeepInsideNearestMonitor();

					newFw.Left = paneForExtensions.FloatingLeft;
					newFw.Top = paneForExtensions.FloatingTop;
					newFw.Width = paneForExtensions.FloatingWidth;
					newFw.Height = paneForExtensions.FloatingHeight;
				}

				newFw.ShowInTaskbar = false;
				newFw.Show();
				// Do not set the WindowState before showing or it will be lost
				if (paneForExtensions != null && paneForExtensions.IsMaximized)
				{
					newFw.WindowState = WindowState.Maximized;
				}
				return newFw;
			}

			var window = model as LayoutDocumentFloatingWindow;
			if (window != null)
			{
				if (DesignerProperties.GetIsInDesignMode(this))
					return null;
				var modelFw = window;
				var newFw = new LayoutDocumentFloatingWindowControl(modelFw);
				newFw.SetParentToMainWindowOf(this);

				var paneForExtensions = modelFw.RootDocument;
				if (paneForExtensions != null)
				{
					//ensure that floating window position is inside current (or nearest) monitor
					paneForExtensions.KeepInsideNearestMonitor();

					newFw.Left = paneForExtensions.FloatingLeft;
					newFw.Top = paneForExtensions.FloatingTop;
					newFw.Width = paneForExtensions.FloatingWidth;
					newFw.Height = paneForExtensions.FloatingHeight;
				}

				newFw.ShowInTaskbar = true;
				newFw.Show();
				// Do not set the WindowState before showing or it will be lost
				if (paneForExtensions != null && paneForExtensions.IsMaximized)
				{
					newFw.WindowState = WindowState.Maximized;
				}
				return newFw;
			}

			var document = model as LayoutDocument;
			if (document != null)
			{
				var templateModelView = new LayoutDocumentControl {Model = document};
				return templateModelView;
			}

			return null;
		}

		private void DockingManager_Loaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			//load windows not already loaded!
			foreach (var fw in Layout.FloatingWindows.Where(fw => !_fwList.Any(fwc => Equals(fwc.Model, fw))))
				_fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);

			//create the overlaywindow if it's possible
			if (IsVisible)
				CreateOverlayWindow();
			FocusElementManager.SetupFocusManagement(this);
		}

		private void DockingManager_Unloaded(object sender, RoutedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			_autoHideWindowManager.HideAutoWindow();

			foreach (var fw in _fwList.ToArray())
			{
				//fw.Owner = null;
				fw.SetParentWindowToNull();
				fw.KeepContentVisibleOnClose = true;
				fw.Close();
			}

			DestroyOverlayWindow();
			FocusElementManager.FinalizeFocusManagement(this);
		}

		public static readonly DependencyProperty LayoutProperty =
			DependencyProperty.Register("Layout", typeof (LayoutRoot), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutChanged, CoerceLayoutValue));

		public LayoutRoot Layout
		{
			get { return (LayoutRoot) GetValue(LayoutProperty); }
			set { SetValue(LayoutProperty, value); }
		}

		private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
		}

		protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
		{
			if (oldLayout != null)
			{
				oldLayout.PropertyChanged -= OnLayoutRootPropertyChanged;
				oldLayout.Updated -= OnLayoutRootUpdated;
			}

			foreach (var fwc in _fwList.ToArray())
			{
				fwc.KeepContentVisibleOnClose = true;
				fwc.InternalClose();
			}

			_fwList.Clear();

			DetachDocumentsSource(oldLayout, DocumentsSource);
			DetachAnchorablesSource(oldLayout, AnchorablesSource);

			if (oldLayout != null &&
			    Equals(oldLayout.Manager, this))
				oldLayout.Manager = null;

			ClearLogicalChildrenList();
			DetachLayoutItems();

			Layout.Manager = this;

			AttachLayoutItems();
			AttachDocumentsSource(newLayout, DocumentsSource);
			AttachAnchorablesSource(newLayout, AnchorablesSource);

			if (IsLoaded)
			{
				LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
				LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
				TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
				RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
				BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;

				foreach (var fw in Layout.FloatingWindows.ToArray().Where(fw => fw.IsValid))
				{
					_fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);
				}
			}


			if (newLayout != null)
			{
				newLayout.PropertyChanged += OnLayoutRootPropertyChanged;
				newLayout.Updated += OnLayoutRootUpdated;
			}

			LayoutChanged?.Invoke(this, EventArgs.Empty);

			//if (Layout != null)
			//    Layout.CollectGarbage();

			CommandManager.InvalidateRequerySuggested();
		}

		private DispatcherOperation _setFocusAsyncOperation;

		private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "RootPanel":
					if (IsInitialized)
					{
						var layoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
						LayoutRootPanel = layoutRootPanel;
					}
					break;
				case "ActiveContent":
					if (Layout.ActiveContent != null)
					{
						//Debug.WriteLine(new StackTrace().ToString());

						//set focus on active element only after a layout pass is completed
						//it's possible that it is not yet visible in the visual tree
						if (_setFocusAsyncOperation == null)
						{
							_setFocusAsyncOperation = Dispatcher.BeginInvoke(new Action(() =>
							{
								if (Layout.ActiveContent != null)
									FocusElementManager.SetFocusOnLastElement(Layout.ActiveContent);
								_setFocusAsyncOperation = null;
							}), DispatcherPriority.Background);
						}
					}

					if (!_insideInternalSetActiveContent)
						ActiveContent = Layout.ActiveContent?.Content;
					break;
			}
		}

		private void OnLayoutRootUpdated(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}


		public event EventHandler LayoutChanged;

		private static object CoerceLayoutValue(DependencyObject d, object value)
		{
			if (value == null)
				return new LayoutRoot()
				{
					RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
				};

			((DockingManager) d).OnLayoutChanging(value as LayoutRoot);

			return value;
		}

		public event EventHandler LayoutChanging;

		// ReSharper disable once UnusedParameter.Local
		private void OnLayoutChanging(LayoutRoot newLayout)
		{
			LayoutChanging?.Invoke(this, EventArgs.Empty);
		}


		public static readonly DependencyProperty LayoutUpdateStrategyProperty =
			DependencyProperty.Register("LayoutUpdateStrategy", typeof (ILayoutUpdateStrategy), typeof (DockingManager),
				new FrameworkPropertyMetadata((ILayoutUpdateStrategy) null));

		public ILayoutUpdateStrategy LayoutUpdateStrategy
		{
			get { return (ILayoutUpdateStrategy) GetValue(LayoutUpdateStrategyProperty); }
			set { SetValue(LayoutUpdateStrategyProperty, value); }
		}

		public static readonly DependencyProperty DocumentPaneTemplateProperty =
			DependencyProperty.Register("DocumentPaneTemplate", typeof (ControlTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentPaneTemplateChanged));

		public ControlTemplate DocumentPaneTemplate
		{
			get { return (ControlTemplate) GetValue(DocumentPaneTemplateProperty); }
			set { SetValue(DocumentPaneTemplateProperty, value); }
		}

		private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentPaneTemplateChanged(e);
		}

		protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty AnchorablePaneTemplateProperty =
			DependencyProperty.Register("AnchorablePaneTemplate", typeof (ControlTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnAnchorablePaneTemplateChanged));

		public ControlTemplate AnchorablePaneTemplate
		{
			get { return (ControlTemplate) GetValue(AnchorablePaneTemplateProperty); }
			set { SetValue(AnchorablePaneTemplateProperty, value); }
		}

		private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorablePaneTemplateChanged(e);
		}

		protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty AnchorSideTemplateProperty =
			DependencyProperty.Register("AnchorSideTemplate", typeof (ControlTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata((ControlTemplate) null));

		public ControlTemplate AnchorSideTemplate
		{
			get { return (ControlTemplate) GetValue(AnchorSideTemplateProperty); }
			set { SetValue(AnchorSideTemplateProperty, value); }
		}


		public static readonly DependencyProperty AnchorGroupTemplateProperty =
			DependencyProperty.Register("AnchorGroupTemplate", typeof (ControlTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata((ControlTemplate) null));

		public ControlTemplate AnchorGroupTemplate
		{
			get { return (ControlTemplate) GetValue(AnchorGroupTemplateProperty); }
			set { SetValue(AnchorGroupTemplateProperty, value); }
		}

		public static readonly DependencyProperty AnchorTemplateProperty =
			DependencyProperty.Register("AnchorTemplate", typeof (ControlTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata((ControlTemplate) null));

		public ControlTemplate AnchorTemplate
		{
			get { return (ControlTemplate) GetValue(AnchorTemplateProperty); }
			set { SetValue(AnchorTemplateProperty, value); }
		}

		public static readonly DependencyProperty DocumentPaneControlStyleProperty =
			DependencyProperty.Register("DocumentPaneControlStyle", typeof (Style), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentPaneControlStyleChanged));

		public Style DocumentPaneControlStyle
		{
			get { return (Style) GetValue(DocumentPaneControlStyleProperty); }
			set { SetValue(DocumentPaneControlStyleProperty, value); }
		}

		private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentPaneControlStyleChanged(e);
		}

		protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty AnchorablePaneControlStyleProperty =
			DependencyProperty.Register("AnchorablePaneControlStyle", typeof (Style), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnAnchorablePaneControlStyleChanged));

		public Style AnchorablePaneControlStyle
		{
			get { return (Style) GetValue(AnchorablePaneControlStyleProperty); }
			set { SetValue(AnchorablePaneControlStyleProperty, value); }
		}

		private static void OnAnchorablePaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorablePaneControlStyleChanged(e);
		}

		protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty DocumentHeaderTemplateProperty =
			DependencyProperty.Register("DocumentHeaderTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateChanged, CoerceDocumentHeaderTemplateValue));

		public DataTemplate DocumentHeaderTemplate
		{
			get { return (DataTemplate) GetValue(DocumentHeaderTemplateProperty); }
			set { SetValue(DocumentHeaderTemplateProperty, value); }
		}

		private static void OnDocumentHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentHeaderTemplateChanged(e);
		}

		protected virtual void OnDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null &&
			    d.GetValue(DocumentHeaderTemplateSelectorProperty) != null)
				return null;
			return value;
		}

		public static readonly DependencyProperty DocumentHeaderTemplateSelectorProperty =
			DependencyProperty.Register("DocumentHeaderTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateSelectorChanged,
					CoerceDocumentHeaderTemplateSelectorValue));

		public DataTemplateSelector DocumentHeaderTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(DocumentHeaderTemplateSelectorProperty); }
			set { SetValue(DocumentHeaderTemplateSelectorProperty, value); }
		}

		private static void OnDocumentHeaderTemplateSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null &&
			    DocumentHeaderTemplate != null)
				DocumentHeaderTemplate = null;

			if (DocumentPaneMenuItemHeaderTemplateSelector == null)
				DocumentPaneMenuItemHeaderTemplateSelector = DocumentHeaderTemplateSelector;
		}

		private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		public static readonly DependencyProperty DocumentTitleTemplateProperty =
			DependencyProperty.Register("DocumentTitleTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentTitleTemplateChanged, CoerceDocumentTitleTemplateValue));

		public DataTemplate DocumentTitleTemplate
		{
			get { return (DataTemplate) GetValue(DocumentTitleTemplateProperty); }
			set { SetValue(DocumentTitleTemplateProperty, value); }
		}

		private static void OnDocumentTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentTitleTemplateChanged(e);
		}

		protected virtual void OnDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
		{
			if (value != null &&
			    d.GetValue(DocumentTitleTemplateSelectorProperty) != null)
				return null;

			return value;
		}

		public static readonly DependencyProperty DocumentTitleTemplateSelectorProperty =
			DependencyProperty.Register("DocumentTitleTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null,
					OnDocumentTitleTemplateSelectorChanged,
					CoerceDocumentTitleTemplateSelectorValue));

		public DataTemplateSelector DocumentTitleTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(DocumentTitleTemplateSelectorProperty); }
			set { SetValue(DocumentTitleTemplateSelectorProperty, value); }
		}

		private static void OnDocumentTitleTemplateSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentTitleTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
				DocumentTitleTemplate = null;
		}

		private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		public static readonly DependencyProperty AnchorableTitleTemplateProperty =
			DependencyProperty.Register("AnchorableTitleTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null,
					OnAnchorableTitleTemplateChanged,
					CoerceAnchorableTitleTemplateValue));

		public DataTemplate AnchorableTitleTemplate
		{
			get { return (DataTemplate) GetValue(AnchorableTitleTemplateProperty); }
			set { SetValue(AnchorableTitleTemplateProperty, value); }
		}

		private static void OnAnchorableTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorableTitleTemplateChanged(e);
		}

		protected virtual void OnAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
		{
			if (value != null &&
			    d.GetValue(AnchorableTitleTemplateSelectorProperty) != null)
				return null;
			return value;
		}

		public static readonly DependencyProperty AnchorableTitleTemplateSelectorProperty =
			DependencyProperty.Register("AnchorableTitleTemplateSelector", typeof (DataTemplateSelector), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnAnchorableTitleTemplateSelectorChanged));

		public DataTemplateSelector AnchorableTitleTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(AnchorableTitleTemplateSelectorProperty); }
			set { SetValue(AnchorableTitleTemplateSelectorProperty, value); }
		}

		private static void OnAnchorableTitleTemplateSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorableTitleTemplateSelectorChanged(e);
		}

		protected virtual void OnAnchorableTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null &&
			    AnchorableTitleTemplate != null)
				AnchorableTitleTemplate = null;
		}

		public static readonly DependencyProperty AnchorableHeaderTemplateProperty =
			DependencyProperty.Register("AnchorableHeaderTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateChanged, CoerceAnchorableHeaderTemplateValue));

		public DataTemplate AnchorableHeaderTemplate
		{
			get { return (DataTemplate) GetValue(AnchorableHeaderTemplateProperty); }
			set { SetValue(AnchorableHeaderTemplateProperty, value); }
		}

		private static void OnAnchorableHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorableHeaderTemplateChanged(e);
		}

		protected virtual void OnAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null &&
			    d.GetValue(AnchorableHeaderTemplateSelectorProperty) != null)
				return null;

			return value;
		}

		public static readonly DependencyProperty AnchorableHeaderTemplateSelectorProperty =
			DependencyProperty.Register("AnchorableHeaderTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager), new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateSelectorChanged));

		public DataTemplateSelector AnchorableHeaderTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(AnchorableHeaderTemplateSelectorProperty); }
			set { SetValue(AnchorableHeaderTemplateSelectorProperty, value); }
		}

		private static void OnAnchorableHeaderTemplateSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorableHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnAnchorableHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
				AnchorableHeaderTemplate = null;
		}

		public static readonly DependencyProperty LayoutRootPanelProperty =
			DependencyProperty.Register("LayoutRootPanel", typeof (LayoutPanelControl), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutRootPanelChanged));

		public LayoutPanelControl LayoutRootPanel
		{
			get { return (LayoutPanelControl) GetValue(LayoutRootPanelProperty); }
			set { SetValue(LayoutRootPanelProperty, value); }
		}

		private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutRootPanelChanged(e);
		}

		protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		public static readonly DependencyProperty RightSidePanelProperty =
			DependencyProperty.Register("RightSidePanel", typeof (LayoutAnchorSideControl), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnRightSidePanelChanged));

		public LayoutAnchorSideControl RightSidePanel
		{
			get { return (LayoutAnchorSideControl) GetValue(RightSidePanelProperty); }
			set { SetValue(RightSidePanelProperty, value); }
		}

		private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnRightSidePanelChanged(e);
		}

		protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		public static readonly DependencyProperty LeftSidePanelProperty =
			DependencyProperty.Register("LeftSidePanel", typeof (LayoutAnchorSideControl), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLeftSidePanelChanged));

		public LayoutAnchorSideControl LeftSidePanel
		{
			get { return (LayoutAnchorSideControl) GetValue(LeftSidePanelProperty); }
			set { SetValue(LeftSidePanelProperty, value); }
		}

		private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLeftSidePanelChanged(e);
		}

		protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		public static readonly DependencyProperty TopSidePanelProperty =
			DependencyProperty.Register("TopSidePanel", typeof (LayoutAnchorSideControl), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnTopSidePanelChanged));

		public LayoutAnchorSideControl TopSidePanel
		{
			get { return (LayoutAnchorSideControl) GetValue(TopSidePanelProperty); }
			set { SetValue(TopSidePanelProperty, value); }
		}

		private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnTopSidePanelChanged(e);
		}

		protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		public static readonly DependencyProperty BottomSidePanelProperty =
			DependencyProperty.Register("BottomSidePanel", typeof (LayoutAnchorSideControl), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnBottomSidePanelChanged));

		public LayoutAnchorSideControl BottomSidePanel
		{
			get { return (LayoutAnchorSideControl) GetValue(BottomSidePanelProperty); }
			set { SetValue(BottomSidePanelProperty, value); }
		}

		private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnBottomSidePanelChanged(e);
		}

		protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		private readonly List<WeakReference> _logicalChildren = new List<WeakReference>();

		protected override IEnumerator LogicalChildren
		{
			get { return _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).GetEnumerator(); }
		}


		internal void InternalAddLogicalChild(object element)
		{
			//System.Diagnostics.Trace.WriteLine("[{0}]InternalAddLogicalChild({1})", this, element);
#if DEBUG
			if (_logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).Contains(element))
				// ReSharper disable once ObjectCreationAsStatement
				new InvalidOperationException();
#endif
			if (_logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).Contains(element))
				return;

			_logicalChildren.Add(new WeakReference(element));
			AddLogicalChild(element);
		}

		internal void InternalRemoveLogicalChild(object element)
		{
			//System.Diagnostics.Trace.WriteLine("[{0}]InternalRemoveLogicalChild({1})", this, element);

			var wrToRemove = _logicalChildren.FirstOrDefault(ch => ch.GetValueOrDefault<object>() == element);
			if (wrToRemove != null)
				_logicalChildren.Remove(wrToRemove);
			RemoveLogicalChild(element);
		}

		private void ClearLogicalChildrenList()
		{
			foreach (var child in _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).ToArray())
				RemoveLogicalChild(child);
			_logicalChildren.Clear();
		}

		internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
		{
			_autoHideWindowManager.ShowAutoHideWindow(anchor);
			//if (_autohideArea == null)
			//    return;

			//if (AutoHideWindow != null && AutoHideWindow.Model == anchor.Model)
			//    return;

			//Trace.WriteLine("ShowAutoHideWindow()");

			//_currentAutohiddenAnchor = new WeakReference(anchor);

			//HideAutoHideWindow(anchor);

			//SetAutoHideWindow(new LayoutAutoHideWindowControl(anchor));
			//AutoHideWindow.Show();
		}

		internal void HideAutoHideWindow(LayoutAnchorControl anchor)
		{
			_autoHideWindowManager.HideAutoWindow(anchor);
		}

		private void SetupAutoHideWindow()
		{
			_autohideArea = GetTemplateChild("PART_AutoHideArea") as FrameworkElement;

			if (_autoHideWindowManager != null)
				_autoHideWindowManager.HideAutoWindow();
			else
				_autoHideWindowManager = new AutoHideWindowManager(this);

			AutoHideWindow?.Dispose();

			SetAutoHideWindow(new LayoutAutoHideWindowControl());
		}

		private AutoHideWindowManager _autoHideWindowManager;

		private FrameworkElement _autohideArea;

		internal FrameworkElement GetAutoHideAreaElement()
		{
			return _autohideArea;
		}

		private static readonly DependencyPropertyKey AutoHideWindowPropertyKey
			= DependencyProperty.RegisterReadOnly("AutoHideWindow", typeof (LayoutAutoHideWindowControl),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnAutoHideWindowChanged));

		public static readonly DependencyProperty AutoHideWindowProperty
			= AutoHideWindowPropertyKey.DependencyProperty;

		public LayoutAutoHideWindowControl AutoHideWindow => (LayoutAutoHideWindowControl) GetValue(AutoHideWindowProperty);

		protected void SetAutoHideWindow(LayoutAutoHideWindowControl value)
		{
			SetValue(AutoHideWindowPropertyKey, value);
		}

		private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAutoHideWindowChanged(e);
		}

		protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != null)
				InternalRemoveLogicalChild(e.OldValue);
			if (e.NewValue != null)
				InternalAddLogicalChild(e.NewValue);
		}

		private readonly List<LayoutFloatingWindowControl> _fwList = new List<LayoutFloatingWindowControl>();

		internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel, bool startDrag = true)
		{
			if (!contentModel.CanFloat)
				return;
			var contentModelAsAnchorable = contentModel as LayoutAnchorable;
			if (contentModelAsAnchorable != null &&
			    contentModelAsAnchorable.IsAutoHidden)
				contentModelAsAnchorable.ToggleAutoHide();

			var parentPane = contentModel.Parent as ILayoutPane;
			var parentPaneAsPositionableElement = contentModel.Parent as ILayoutPositionableElement;
			var parentPaneAsWithActualSize = contentModel.Parent as ILayoutPositionableElementWithActualSize;
			// ReSharper disable once PossibleNullReferenceException
			List<ILayoutElement> list = parentPane.Children.ToList();
			var contentModelParentChildrenIndex = list.IndexOf(contentModel);

			if (contentModel.FindParent<LayoutFloatingWindow>() == null)
			{
				((ILayoutPreviousContainer) contentModel).PreviousContainer = parentPane;
				contentModel.PreviousContainerIndex = contentModelParentChildrenIndex;
			}

			parentPane.RemoveChildAt(contentModelParentChildrenIndex);

			double fwWidth = contentModel.FloatingWidth;
			double fwHeight = contentModel.FloatingHeight;

			if (fwWidth == 0.0)
				if (parentPaneAsPositionableElement != null) fwWidth = parentPaneAsPositionableElement.FloatingWidth;
			if (fwHeight == 0.0)
				if (parentPaneAsPositionableElement != null) fwHeight = parentPaneAsPositionableElement.FloatingHeight;

			if (fwWidth == 0.0)
				if (parentPaneAsWithActualSize != null) fwWidth = parentPaneAsWithActualSize.ActualWidth;
			if (fwHeight == 0.0)
				if (parentPaneAsWithActualSize != null) fwHeight = parentPaneAsWithActualSize.ActualHeight;

			LayoutFloatingWindow fw;
			LayoutFloatingWindowControl fwc;
			if (contentModel is LayoutAnchorable)
			{
				var anchorableContent = contentModel as LayoutAnchorable;
				fw = new LayoutAnchorableFloatingWindow
				{
					RootPanel = new LayoutAnchorablePaneGroup(
						new LayoutAnchorablePane(anchorableContent)
						{
							// ReSharper disable once PossibleNullReferenceException
							DockWidth = parentPaneAsPositionableElement.DockWidth,
							DockHeight = parentPaneAsPositionableElement.DockHeight,
							DockMinHeight = parentPaneAsPositionableElement.DockMinHeight,
							DockMinWidth = parentPaneAsPositionableElement.DockMinWidth,
							FloatingLeft = parentPaneAsPositionableElement.FloatingLeft,
							FloatingTop = parentPaneAsPositionableElement.FloatingTop,
							FloatingWidth = parentPaneAsPositionableElement.FloatingWidth,
							FloatingHeight = parentPaneAsPositionableElement.FloatingHeight,
						})
				};

				Layout.FloatingWindows.Add(fw);

				fwc = new LayoutAnchorableFloatingWindowControl((LayoutAnchorableFloatingWindow) fw)
				{
					Width = fwWidth,
					Height = fwHeight,
					Left = contentModel.FloatingLeft,
					Top = contentModel.FloatingTop
				};
			}
			else
			{
				var anchorableDocument = contentModel as LayoutDocument;
				fw = new LayoutDocumentFloatingWindow
				{
					RootDocument = anchorableDocument
				};

				Layout.FloatingWindows.Add(fw);

				fwc = new LayoutDocumentFloatingWindowControl((LayoutDocumentFloatingWindow) fw)
				{
					Width = fwWidth,
					Height = fwHeight,
					Left = contentModel.FloatingLeft,
					Top = contentModel.FloatingTop
				};
			}

			_fwList.Add(fwc);

			Layout.CollectGarbage();

			UpdateLayout();

			Dispatcher.BeginInvoke(new Action(() =>
			{
				if (startDrag)
					fwc.AttachDrag();
				fwc.Show();
			}), DispatcherPriority.Send);
		}

		internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
		{
			if (paneModel.Children.Any(c => !c.CanFloat))
				return;
			var paneAsPositionableElement = paneModel as ILayoutPositionableElement;
			var paneAsWithActualSize = paneModel as ILayoutPositionableElementWithActualSize;

			var fwWidth = paneAsPositionableElement.FloatingWidth;
			var fwHeight = paneAsPositionableElement.FloatingHeight;


			if (fwWidth == 0.0)
				fwWidth = paneAsWithActualSize.ActualWidth;
			if (fwHeight == 0.0)
				fwHeight = paneAsWithActualSize.ActualHeight;

			var destPane = new LayoutAnchorablePane
			{
				DockWidth = paneAsPositionableElement.DockWidth,
				DockHeight = paneAsPositionableElement.DockHeight,
				DockMinHeight = paneAsPositionableElement.DockMinHeight,
				DockMinWidth = paneAsPositionableElement.DockMinWidth,
				FloatingLeft = paneAsPositionableElement.FloatingLeft,
				FloatingTop = paneAsPositionableElement.FloatingTop,
				FloatingWidth = paneAsPositionableElement.FloatingWidth,
				FloatingHeight = paneAsPositionableElement.FloatingHeight,
			};

			var savePreviousContainer = paneModel.FindParent<LayoutFloatingWindow>() == null;
			var currentSelectedContentIndex = paneModel.SelectedContentIndex;
			while (paneModel.Children.Count > 0)
			{
				var contentModel = paneModel.Children[paneModel.Children.Count - 1];

				if (savePreviousContainer)
				{
					var contentModelAsPreviousContainer = (ILayoutPreviousContainer) contentModel;
					contentModelAsPreviousContainer.PreviousContainer = paneModel;
					contentModel.PreviousContainerIndex = paneModel.Children.Count - 1;
				}

				paneModel.RemoveChildAt(paneModel.Children.Count - 1);
				destPane.Children.Insert(0, contentModel);
			}

			if (destPane.Children.Count > 0)
			{
				destPane.SelectedContentIndex = currentSelectedContentIndex;
			}


			LayoutFloatingWindow fw = new LayoutAnchorableFloatingWindow()
			{
				RootPanel = new LayoutAnchorablePaneGroup(
					destPane)
				{
					DockHeight = destPane.DockHeight,
					DockWidth = destPane.DockWidth,
					DockMinHeight = destPane.DockMinHeight,
					DockMinWidth = destPane.DockMinWidth,
				}
			};

			Layout.FloatingWindows.Add(fw);

			LayoutFloatingWindowControl fwc = new LayoutAnchorableFloatingWindowControl(
				(LayoutAnchorableFloatingWindow) fw)
			{
				Width = fwWidth,
				Height = fwHeight
			};

			_fwList.Add(fwc);

			Layout.CollectGarbage();

			InvalidateArrange();

			fwc.AttachDrag();
			fwc.Show();
		}

		internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
		{
			var parentWindow = Window.GetWindow(this);

			if (parentWindow == null)
				yield break;

			var windowParentHanlde = new WindowInteropHelper(parentWindow).Handle;

			var currentHandle = Win32Helper.GetWindow(windowParentHanlde,
				(uint) Win32Helper.GetWindowCmd.GwHwndfirst);
			while (currentHandle != IntPtr.Zero)
			{
				LayoutFloatingWindowControl ctrl =
					_fwList.FirstOrDefault(fw => new WindowInteropHelper(fw).Handle == currentHandle);
				if (ctrl != null && Equals(ctrl.Model.Root.Manager, this))
					yield return ctrl;

				currentHandle = Win32Helper.GetWindow(currentHandle, (uint) Win32Helper.GetWindowCmd.GwHwndnext);
			}
		}

		internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
		{
			_fwList.Remove(floatingWindow);
		}

		public IEnumerable<LayoutFloatingWindowControl> FloatingWindows => _fwList;

		bool IOverlayWindowHost.HitTest(Point dragPoint)
		{
			var detectionRect = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
				this.TransformActualSizeToAncestor());
			return detectionRect.Contains(dragPoint);
		}

		DockingManager IOverlayWindowHost.Manager => this;

		private OverlayWindow _overlayWindow;

		private void CreateOverlayWindow()
		{
			if (_overlayWindow == null)
			{
				_overlayWindow = new OverlayWindow(this);
			}
			var rectWindow = new Rect(this.PointToScreenDpiWithoutFlowDirection(new Point()),
				this.TransformActualSizeToAncestor());
			_overlayWindow.Left = rectWindow.Left;
			_overlayWindow.Top = rectWindow.Top;
			_overlayWindow.Width = rectWindow.Width;
			_overlayWindow.Height = rectWindow.Height;
		}

		private void DestroyOverlayWindow()
		{
			if (_overlayWindow == null)
				return;
			_overlayWindow.Close();
			_overlayWindow = null;
		}

		IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
		{
			CreateOverlayWindow();
			_overlayWindow.Owner = draggingWindow;
			_overlayWindow.EnableDropTargets();
			_overlayWindow.Show();
			return _overlayWindow;
		}

		void IOverlayWindowHost.HideOverlayWindow()
		{
			_areas = null;
			_overlayWindow.Owner = null;
			_overlayWindow.HideDropTargets();
		}

		private List<IDropArea> _areas;

		IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
		{
			if (_areas != null)
				return _areas;

			var isDraggingDocuments = draggingWindow.Model is LayoutDocumentFloatingWindow;

			_areas = new List<IDropArea>();

			if (!isDraggingDocuments)
			{
				_areas.Add(new DropArea<DockingManager>(
					this,
					DropAreaType.DockingManager));

				foreach (
					var areaHost in
						this.FindVisualChildren<LayoutAnchorablePaneControl>().Where(areaHost => areaHost.Model.Descendents().Any()))
				{
					_areas.Add(new DropArea<LayoutAnchorablePaneControl>(
						areaHost,
						DropAreaType.AnchorablePane));
				}
			}

			foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneControl>())
			{
				_areas.Add(new DropArea<LayoutDocumentPaneControl>(
					areaHost,
					DropAreaType.DocumentPane));
			}

			foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
			{
				var documentGroupModel = areaHost.Model as LayoutDocumentPaneGroup;
				if (documentGroupModel != null && !documentGroupModel.Children.Any(c => c.IsVisible))
				{
					_areas.Add(new DropArea<LayoutDocumentPaneGroupControl>(
						areaHost,
						DropAreaType.DocumentPaneGroup));
				}
			}

			return _areas;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_areas = null;
			return base.ArrangeOverride(arrangeBounds);
		}

		public static readonly DependencyProperty LayoutItemTemplateProperty =
			DependencyProperty.Register("LayoutItemTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutItemTemplateChanged));

		public DataTemplate LayoutItemTemplate
		{
			get { return (DataTemplate) GetValue(LayoutItemTemplateProperty); }
			set { SetValue(LayoutItemTemplateProperty, value); }
		}

		private static void OnLayoutItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutItemTemplateChanged(e);
		}

		protected virtual void OnLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty LayoutItemTemplateSelectorProperty =
			DependencyProperty.Register("LayoutItemTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutItemTemplateSelectorChanged));

		public DataTemplateSelector LayoutItemTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(LayoutItemTemplateSelectorProperty); }
			set { SetValue(LayoutItemTemplateSelectorProperty, value); }
		}

		private static void OnLayoutItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutItemTemplateSelectorChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the LayoutItemTemplateSelector property.
		/// </summary>
		protected virtual void OnLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		public static readonly DependencyProperty DocumentsSourceProperty =
			DependencyProperty.Register("DocumentsSource", typeof (IEnumerable), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentsSourceChanged));

		public IEnumerable DocumentsSource
		{
			get { return (IEnumerable) GetValue(DocumentsSourceProperty); }
			set { SetValue(DocumentsSourceProperty, value); }
		}

		private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentsSourceChanged(e);
		}

		protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			DetachDocumentsSource(Layout, e.OldValue as IEnumerable);
			AttachDocumentsSource(Layout, e.NewValue as IEnumerable);
		}

		private void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
		{
			if (documentsSource == null)
				return;

			if (layout == null)
				return;

			var documentsImported = layout.Descendents().OfType<LayoutDocument>().Select(d => d.Content).ToArray();
			var documents = documentsSource;
			var listOfDocumentsToImport = new List<object>(documents.OfType<object>());

			foreach (var document in listOfDocumentsToImport.ToArray().Where(document => documentsImported.Contains(document)))
			{
				listOfDocumentsToImport.Remove(document);
			}

			LayoutDocumentPane documentPane = null;
			if (layout.LastFocusedDocument != null)
			{
				documentPane = layout.LastFocusedDocument.Parent as LayoutDocumentPane;
			}

			if (documentPane == null)
			{
				documentPane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
			}

			_suspendLayoutItemCreation = true;
			foreach (var documentContentToImport in listOfDocumentsToImport)
			{
				var documentToImport = new LayoutDocument()
				{
					Content = documentContentToImport
				};

				bool added = false;
				if (LayoutUpdateStrategy != null)
				{
					added = LayoutUpdateStrategy.BeforeInsertDocument(layout, documentToImport, documentPane);
				}

				if (!added)
				{
					if (documentPane == null)
						throw new InvalidOperationException(
							"Layout must contains at least one LayoutDocumentPane in order to host documents");

					documentPane.Children.Add(documentToImport);
				}

				LayoutUpdateStrategy?.AfterInsertDocument(layout, documentToImport);


				CreateDocumentLayoutItem(documentToImport);
			}
			_suspendLayoutItemCreation = true;


			var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
			if (documentsSourceAsNotifier != null)
				documentsSourceAsNotifier.CollectionChanged += DocumentsSourceElementsChanged;
		}

		internal bool SuspendDocumentsSourceBinding = false;

		private void DocumentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Layout == null)
				return;

			//When deserializing documents are created automatically by the deserializer
			if (SuspendDocumentsSourceBinding)
				return;

			//handle remove
			if (e.Action == NotifyCollectionChangedAction.Remove ||
			    e.Action == NotifyCollectionChangedAction.Replace)
			{
				if (e.OldItems != null)
				{
					var documentsToRemove =
						Layout.Descendents()
							.OfType<LayoutDocument>()
							.Where(d => e.OldItems.Contains(d.Content))
							.ToArray();
					foreach (var documentToRemove in documentsToRemove)
					{
						documentToRemove.Parent.RemoveChild(
							documentToRemove);
					}
				}
			}

			//handle add
			if (e.NewItems != null &&
			    (e.Action == NotifyCollectionChangedAction.Add ||
			     e.Action == NotifyCollectionChangedAction.Replace))
			{
				if (e.NewItems != null)
				{
					LayoutDocumentPane documentPane = null;
					if (Layout.LastFocusedDocument != null)
					{
						documentPane = Layout.LastFocusedDocument.Parent as LayoutDocumentPane;
					}

					if (documentPane == null)
					{
						documentPane = Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
					}

					_suspendLayoutItemCreation = true;

					foreach (var documentContentToImport in e.NewItems)
					{
						var documentToImport = new LayoutDocument()
						{
							Content = documentContentToImport
						};

						bool added = false;
						if (LayoutUpdateStrategy != null)
						{
							added = LayoutUpdateStrategy.BeforeInsertDocument(Layout, documentToImport, documentPane);
						}

						if (!added)
						{
							if (documentPane == null)
								throw new InvalidOperationException(
									"Layout must contains at least one LayoutDocumentPane in order to host documents");

							documentPane.Children.Add(documentToImport);
						}

						LayoutUpdateStrategy?.AfterInsertDocument(Layout, documentToImport);

						var root = documentToImport.Root;

						if (root != null && Equals(root.Manager, this))
						{
							CreateDocumentLayoutItem(documentToImport);
						}
					}
					_suspendLayoutItemCreation = false;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				//NOTE: I'm going to clear every document present in layout but
				//some documents may have been added directly to the layout, for now I clear them too
				var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().ToArray();
				foreach (var documentToRemove in documentsToRemove)
				{
					documentToRemove.Parent.RemoveChild(
						documentToRemove);
				}
			}
			Layout?.CollectGarbage();
		}

		private void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
		{
			if (documentsSource == null)
				return;

			if (layout == null)
				return;

			var documentsToRemove = layout.Descendents().OfType<LayoutDocument>()
				.Where(d => documentsSource.Contains(d.Content)).ToArray();

			foreach (var documentToRemove in documentsToRemove)
			{
				documentToRemove.Parent.RemoveChild(
					documentToRemove);
			}

			var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
			if (documentsSourceAsNotifier != null)
				documentsSourceAsNotifier.CollectionChanged -= DocumentsSourceElementsChanged;
		}

		internal void _ExecuteCloseCommand(LayoutDocument document)
		{
			if (DocumentClosing != null)
			{
				var evargs = new DocumentClosingEventArgs(document);
				DocumentClosing(this, evargs);
				if (evargs.Cancel)
					return;
			}

			if (!document.TestCanClose())
				return;

			document.Close();

			if (DocumentClosed != null)
			{
				var evargs = new DocumentClosedEventArgs(document);
				DocumentClosed(this, evargs);
			}
		}

		public event EventHandler<DocumentClosingEventArgs> DocumentClosing;

		public event EventHandler<DocumentClosedEventArgs> DocumentClosed;

		public static readonly DependencyProperty DocumentContextMenuProperty =
			DependencyProperty.Register("DocumentContextMenu", typeof (ContextMenu), typeof (DockingManager),
				new FrameworkPropertyMetadata((ContextMenu) null));

		public ContextMenu DocumentContextMenu
		{
			get { return (ContextMenu) GetValue(DocumentContextMenuProperty); }
			set { SetValue(DocumentContextMenuProperty, value); }
		}

		public static readonly DependencyProperty AnchorablesSourceProperty =
			DependencyProperty.Register("AnchorablesSource", typeof (IEnumerable), typeof (DockingManager),
				new FrameworkPropertyMetadata(null,
					OnAnchorablesSourceChanged));

		public IEnumerable AnchorablesSource
		{
			get { return (IEnumerable) GetValue(AnchorablesSourceProperty); }
			set { SetValue(AnchorablesSourceProperty, value); }
		}

		private static void OnAnchorablesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnAnchorablesSourceChanged(e);
		}

		protected virtual void OnAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			DetachAnchorablesSource(Layout, e.OldValue as IEnumerable);
			AttachAnchorablesSource(Layout, e.NewValue as IEnumerable);
		}

		private void AttachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
		{
			if (anchorablesSource == null)
				return;

			if (layout == null)
				return;

			var anchorablesImported = layout.Descendents().OfType<LayoutAnchorable>().Select(d => d.Content).ToArray();
			var anchorables = anchorablesSource;
			var listOfAnchorablesToImport = new List<object>(anchorables.OfType<object>());

			foreach (
				var document in listOfAnchorablesToImport.ToArray().Where(document => anchorablesImported.Contains(document)))
			{
				listOfAnchorablesToImport.Remove(document);
			}

			LayoutAnchorablePane anchorablePane = null;
			if (layout.ActiveContent != null)
			{
				//look for active content parent pane
				anchorablePane = layout.ActiveContent.Parent as LayoutAnchorablePane;
			}

			if (anchorablePane == null)
			{
				//look for a pane on the right side
				anchorablePane =
					layout.Descendents()
						.OfType<LayoutAnchorablePane>()
						.FirstOrDefault(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right);
			}

			if (anchorablePane == null)
			{
				//look for an available pane
				anchorablePane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
			}

			_suspendLayoutItemCreation = true;
			foreach (var anchorableContentToImport in listOfAnchorablesToImport)
			{
				var anchorableToImport = new LayoutAnchorable()
				{
					Content = anchorableContentToImport
				};

				bool added = false;
				if (LayoutUpdateStrategy != null)
				{
					added = LayoutUpdateStrategy.BeforeInsertAnchorable(layout, anchorableToImport, anchorablePane);
				}

				if (!added)
				{
					if (anchorablePane == null)
					{
						var mainLayoutPanel = new LayoutPanel {Orientation = Orientation.Horizontal};
						if (layout.RootPanel != null)
						{
							mainLayoutPanel.Children.Add(layout.RootPanel);
						}

						layout.RootPanel = mainLayoutPanel;
						anchorablePane = new LayoutAnchorablePane
						{
							DockWidth = new GridLength(200.0, GridUnitType.Pixel)
						};
						mainLayoutPanel.Children.Add(anchorablePane);
					}

					anchorablePane.Children.Add(anchorableToImport);
				}
				LayoutUpdateStrategy?.AfterInsertAnchorable(layout, anchorableToImport);
				CreateAnchorableLayoutItem(anchorableToImport);
			}

			_suspendLayoutItemCreation = false;

			var anchorablesSourceAsNotifier = anchorablesSource as INotifyCollectionChanged;
			if (anchorablesSourceAsNotifier != null)
				anchorablesSourceAsNotifier.CollectionChanged += AnchorablesSourceElementsChanged;
		}

		internal bool SuspendAnchorablesSourceBinding = false;

		private void AnchorablesSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Layout == null)
				return;

			//When deserializing documents are created automatically by the deserializer
			if (SuspendAnchorablesSourceBinding)
				return;

			//handle remove
			if (e.Action == NotifyCollectionChangedAction.Remove ||
			    e.Action == NotifyCollectionChangedAction.Replace)
			{
				if (e.OldItems != null)
				{
					var anchorablesToRemove =
						Layout.Descendents()
							.OfType<LayoutAnchorable>()
							.Where(d => e.OldItems.Contains(d.Content))
							.ToArray();
					foreach (var anchorableToRemove in anchorablesToRemove)
					{
						anchorableToRemove.Parent.RemoveChild(
							anchorableToRemove);
					}
				}
			}

			//handle add
			if (e.NewItems != null &&
			    (e.Action == NotifyCollectionChangedAction.Add ||
			     e.Action == NotifyCollectionChangedAction.Replace))
			{
				if (e.NewItems != null)
				{
					LayoutAnchorablePane anchorablePane = null;

					if (Layout.ActiveContent != null)
					{
						//look for active content parent pane
						anchorablePane = Layout.ActiveContent.Parent as LayoutAnchorablePane;
					}

					if (anchorablePane == null)
					{
						//look for a pane on the right side
						anchorablePane =
							Layout.Descendents()
								.OfType<LayoutAnchorablePane>()
								.FirstOrDefault(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right);
					}

					if (anchorablePane == null)
					{
						//look for an available pane
						anchorablePane = Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
					}

					_suspendLayoutItemCreation = true;
					foreach (var anchorableContentToImport in e.NewItems)
					{
						var anchorableToImport = new LayoutAnchorable
						{
							Content = anchorableContentToImport
						};

						bool added = false;
						if (LayoutUpdateStrategy != null)
						{
							added = LayoutUpdateStrategy.BeforeInsertAnchorable(Layout, anchorableToImport,
								anchorablePane);
						}

						if (!added)
						{
							if (anchorablePane == null)
							{
								var mainLayoutPanel = new LayoutPanel {Orientation = Orientation.Horizontal};
								if (Layout.RootPanel != null)
								{
									mainLayoutPanel.Children.Add(Layout.RootPanel);
								}

								Layout.RootPanel = mainLayoutPanel;
								anchorablePane = new LayoutAnchorablePane
								{
									DockWidth = new GridLength(200.0, GridUnitType.Pixel)
								};
								mainLayoutPanel.Children.Add(anchorablePane);
							}

							anchorablePane.Children.Add(anchorableToImport);
						}

						LayoutUpdateStrategy?.AfterInsertAnchorable(Layout, anchorableToImport);

						var root = anchorableToImport.Root;

						if (root != null && Equals(root.Manager, this))
						{
							CreateAnchorableLayoutItem(anchorableToImport);
						}
					}
					_suspendLayoutItemCreation = false;
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				//NOTE: I'm going to clear every anchorable present in layout but
				//some anchorable may have been added directly to the layout, for now I clear them too
				var anchorablesToRemove = Layout.Descendents().OfType<LayoutAnchorable>().ToArray();
				foreach (var anchorableToRemove in anchorablesToRemove)
				{
					anchorableToRemove.Parent.RemoveChild(
						anchorableToRemove);
				}
			}
			Layout?.CollectGarbage();
		}

		private void DetachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
		{
			if (anchorablesSource == null)
				return;

			if (layout == null)
				return;

			var anchorablesToRemove = layout.Descendents().OfType<LayoutAnchorable>()
				.Where(d => anchorablesSource.Contains(d.Content)).ToArray();

			foreach (var anchorableToRemove in anchorablesToRemove)
			{
				anchorableToRemove.Parent.RemoveChild(
					anchorableToRemove);
			}

			var anchorablesSourceAsNotifier = anchorablesSource as INotifyCollectionChanged;
			if (anchorablesSourceAsNotifier != null)
				anchorablesSourceAsNotifier.CollectionChanged -= AnchorablesSourceElementsChanged;
		}

		public static readonly DependencyProperty ActiveContentProperty =
			DependencyProperty.Register("ActiveContent", typeof (object), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnActiveContentChanged));

		public object ActiveContent
		{
			get { return GetValue(ActiveContentProperty); }
			set { SetValue(ActiveContentProperty, value); }
		}

		private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).InternalSetActiveContent(e.NewValue);
			((DockingManager) d).OnActiveContentChanged(e);
		}

		protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
		{
			ActiveContentChanged?.Invoke(this, EventArgs.Empty);
		}


		private bool _insideInternalSetActiveContent;

		private void InternalSetActiveContent(object contentObject)
		{
			var layoutContent =
				Layout.Descendents()
					.OfType<LayoutContent>()
					.FirstOrDefault(lc => Equals(lc, contentObject) || lc.Content == contentObject);
			_insideInternalSetActiveContent = true;
			Layout.ActiveContent = layoutContent;
			_insideInternalSetActiveContent = false;
		}

		public event EventHandler ActiveContentChanged;

		public static readonly DependencyProperty AnchorableContextMenuProperty =
			DependencyProperty.Register("AnchorableContextMenu", typeof (ContextMenu), typeof (DockingManager),
				new FrameworkPropertyMetadata((ContextMenu) null));

		public ContextMenu AnchorableContextMenu
		{
			get { return (ContextMenu) GetValue(AnchorableContextMenuProperty); }
			set { SetValue(AnchorableContextMenuProperty, value); }
		}


		public virtual void OnThemeChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldTheme = e.OldValue as Theme;
			var newTheme = e.NewValue as Theme;
			var resources = Resources;
			if (oldTheme != null)
			{
				var resourceDictionaryToRemove =
					resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
				if (resourceDictionaryToRemove != null)
					resources.MergedDictionaries.Remove(
						resourceDictionaryToRemove);
			}

			if (newTheme != null)
			{
				resources.MergedDictionaries.Add(new ResourceDictionary() {Source = newTheme.GetResourceUri()});
			}

			foreach (var fwc in _fwList)
				fwc.UpdateThemeResources(oldTheme);

			_navigatorWindow?.UpdateThemeResources();
			_overlayWindow?.UpdateThemeResources();
		}

		public static readonly DependencyProperty GridSplitterWidthProperty =
			DependencyProperty.Register("GridSplitterWidth", typeof (double), typeof (DockingManager),
				new FrameworkPropertyMetadata(6.0));

		public double GridSplitterWidth
		{
			get { return (double) GetValue(GridSplitterWidthProperty); }
			set { SetValue(GridSplitterWidthProperty, value); }
		}

		public static readonly DependencyProperty GridSplitterHeightProperty =
			DependencyProperty.Register("GridSplitterHeight", typeof (double), typeof (DockingManager),
				new FrameworkPropertyMetadata(6.0));

		public double GridSplitterHeight
		{
			get { return (double) GetValue(GridSplitterHeightProperty); }
			set { SetValue(GridSplitterHeightProperty, value); }
		}

		public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateProperty =
			DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplate", typeof (DataTemplate),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateChanged,
					CoerceDocumentPaneMenuItemHeaderTemplateValue));

		public DataTemplate DocumentPaneMenuItemHeaderTemplate
		{
			get { return (DataTemplate) GetValue(DocumentPaneMenuItemHeaderTemplateProperty); }
			set { SetValue(DocumentPaneMenuItemHeaderTemplateProperty, value); }
		}

		private static void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentPaneMenuItemHeaderTemplateChanged(e);
		}

		protected virtual void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(DependencyObject d, object value)
		{
			if (value != null &&
			    d.GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
				return null;
			return value ?? d.GetValue(DocumentHeaderTemplateProperty);
		}

		public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateSelectorProperty =
			DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateSelectorChanged,
					CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue));

		public DataTemplateSelector DocumentPaneMenuItemHeaderTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty); }
			set { SetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty, value); }
		}

		private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
		}

		protected virtual void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null &&
			    DocumentPaneMenuItemHeaderTemplate != null)
				DocumentPaneMenuItemHeaderTemplate = null;
		}

		private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(DependencyObject d, object value)
		{
			return value;
		}

		public static readonly DependencyProperty IconContentTemplateProperty =
			DependencyProperty.Register("IconContentTemplate", typeof (DataTemplate), typeof (DockingManager),
				new FrameworkPropertyMetadata((DataTemplate) null));

		public DataTemplate IconContentTemplate
		{
			get { return (DataTemplate) GetValue(IconContentTemplateProperty); }
			set { SetValue(IconContentTemplateProperty, value); }
		}

		public static readonly DependencyProperty IconContentTemplateSelectorProperty =
			DependencyProperty.Register("IconContentTemplateSelector", typeof (DataTemplateSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata((DataTemplateSelector) null));

		public DataTemplateSelector IconContentTemplateSelector
		{
			get { return (DataTemplateSelector) GetValue(IconContentTemplateSelectorProperty); }
			set { SetValue(IconContentTemplateSelectorProperty, value); }
		}

		private readonly List<LayoutItem> _layoutItems = new List<LayoutItem>();

		private bool _suspendLayoutItemCreation;

		private void DetachLayoutItems()
		{
			if (Layout == null)
				return;
			_layoutItems.ForEach<LayoutItem>(i => i.Detach());
			_layoutItems.Clear();
			Layout.ElementAdded -= Layout_ElementAdded;
			Layout.ElementRemoved -= Layout_ElementRemoved;
		}

		private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
		{
			if (_suspendLayoutItemCreation)
				return;

			CollectLayoutItemsDeleted();
		}

		private void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
		{
			if (_suspendLayoutItemCreation)
				return;

			foreach (var content in Layout.Descendents().OfType<LayoutContent>())
			{
				if (content is LayoutDocument)
					CreateDocumentLayoutItem(content as LayoutDocument);
				else //if (content is LayoutAnchorable)
					CreateAnchorableLayoutItem(content as LayoutAnchorable);
			}

			CollectLayoutItemsDeleted();
		}


		private DispatcherOperation _collectLayoutItemsOperations;

		private void CollectLayoutItemsDeleted()
		{
			if (_collectLayoutItemsOperations != null)
				return;
			_collectLayoutItemsOperations = Dispatcher.BeginInvoke(new Action(() =>
			{
				_collectLayoutItemsOperations = null;
				foreach (var itemToRemove in _layoutItems.Where(item => !Equals(item.LayoutElement.Root, Layout)).ToArray())
				{
					if (itemToRemove?.Model is UIElement)
					{
					}

					if (itemToRemove == null)
						continue;
					itemToRemove.Detach();
					_layoutItems.Remove(itemToRemove);
				}
			}));
		}


		private void AttachLayoutItems()
		{
			if (Layout == null)
				return;
			foreach (var document in Layout.Descendents().OfType<LayoutDocument>().ToArray())
			{
				CreateDocumentLayoutItem(document);
			}
			foreach (var anchorable in Layout.Descendents().OfType<LayoutAnchorable>().ToArray())
			{
				CreateAnchorableLayoutItem(anchorable);
			}

			Layout.ElementAdded += Layout_ElementAdded;
			Layout.ElementRemoved += Layout_ElementRemoved;
		}

		private void ApplyStyleToLayoutItem(LayoutItem layoutItem)
		{
			layoutItem._ClearDefaultBindings();
			if (LayoutItemContainerStyle != null)
				layoutItem.Style = LayoutItemContainerStyle;
			else if (LayoutItemContainerStyleSelector != null)
				layoutItem.Style = LayoutItemContainerStyleSelector.SelectStyle(layoutItem.Model, layoutItem);
			layoutItem._SetDefaultBindings();
		}

		private void CreateAnchorableLayoutItem(LayoutAnchorable contentToAttach)
		{
			if (_layoutItems.Any(item => Equals(item.LayoutElement, contentToAttach)))
				return;

			var layoutItem = new LayoutAnchorableItem();
			layoutItem.Attach(contentToAttach);
			ApplyStyleToLayoutItem(layoutItem);
			_layoutItems.Add(layoutItem);

			if (contentToAttach?.Content is UIElement)
			{
				InternalAddLogicalChild(contentToAttach.Content);
			}
		}

		private void CreateDocumentLayoutItem(LayoutDocument contentToAttach)
		{
			if (_layoutItems.Any(item => Equals(item.LayoutElement, contentToAttach)))
				return;

			var layoutItem = new LayoutDocumentItem();
			layoutItem.Attach(contentToAttach);
			ApplyStyleToLayoutItem(layoutItem);
			_layoutItems.Add(layoutItem);

			if (contentToAttach?.Content is UIElement)
			{
				InternalAddLogicalChild(contentToAttach.Content);
			}
		}

		public static readonly DependencyProperty LayoutItemContainerStyleProperty =
			DependencyProperty.Register("LayoutItemContainerStyle", typeof (Style), typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleChanged));

		public Style LayoutItemContainerStyle
		{
			get { return (Style) GetValue(LayoutItemContainerStyleProperty); }
			set { SetValue(LayoutItemContainerStyleProperty, value); }
		}

		private static void OnLayoutItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutItemContainerStyleChanged(e);
		}

		protected virtual void OnLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
		{
			AttachLayoutItems();
		}

		public static readonly DependencyProperty LayoutItemContainerStyleSelectorProperty =
			DependencyProperty.Register("LayoutItemContainerStyleSelector", typeof (StyleSelector),
				typeof (DockingManager),
				new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleSelectorChanged));

		public StyleSelector LayoutItemContainerStyleSelector
		{
			get { return (StyleSelector) GetValue(LayoutItemContainerStyleSelectorProperty); }
			set { SetValue(LayoutItemContainerStyleSelectorProperty, value); }
		}

		private static void OnLayoutItemContainerStyleSelectorChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			((DockingManager) d).OnLayoutItemContainerStyleSelectorChanged(e);
		}

		protected virtual void OnLayoutItemContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
		{
			AttachLayoutItems();
		}

		public LayoutItem GetLayoutItemFromModel(LayoutContent content)
		{
			return _layoutItems.FirstOrDefault(item => Equals(item.LayoutElement, content));
		}

		private NavigatorWindow _navigatorWindow;

		private void ShowNavigatorWindow()
		{
			if (_navigatorWindow == null)
			{
				_navigatorWindow = new NavigatorWindow(this)
				{
					Owner = Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
			}

			_navigatorWindow.ShowDialog();
			_navigatorWindow = null;

			Trace.WriteLine("ShowNavigatorWindow()");
		}

		private bool IsNavigatorWindowActive => _navigatorWindow != null;


		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			Trace.WriteLine($"OnPreviewKeyDown({e.Key})");
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				if (e.IsDown && e.Key == Key.Tab)
				{
					if (!IsNavigatorWindowActive)
					{
						ShowNavigatorWindow();
						e.Handled = true;
					}
				}
			}
			base.OnPreviewKeyDown(e);
		}

		public static readonly DependencyProperty ShowSystemMenuProperty =
			DependencyProperty.Register("ShowSystemMenu", typeof (bool), typeof (DockingManager),
				new FrameworkPropertyMetadata(true));

		public bool ShowSystemMenu
		{
			get { return (bool) GetValue(ShowSystemMenuProperty); }
			set { SetValue(ShowSystemMenuProperty, value); }
		}

		public static readonly DependencyProperty AllowMixedOrientationProperty =
			DependencyProperty.Register("AllowMixedOrientation", typeof (bool), typeof (DockingManager),
				new FrameworkPropertyMetadata(false));

		public bool AllowMixedOrientation
		{
			get { return (bool) GetValue(AllowMixedOrientationProperty); }
			set { SetValue(AllowMixedOrientationProperty, value); }
		}
	}
}