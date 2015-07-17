﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ModernApplicationFramework.Core.Converters;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.ViewModels;

namespace ModernApplicationFramework.Controls
{
	public abstract class MainWindow : ModernChromeWindow
	{
	    public new static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon", typeof (ImageSource), typeof (MainWindow), new PropertyMetadata(default(ImageSource)));

	    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof (Theme),
			typeof (MainWindow), new FrameworkPropertyMetadata(null, OnThemeChanged));

	    public static readonly DependencyProperty IsSimpleWindowTypeProperty = DependencyProperty.Register(
	        "IsSimpleWindowType", typeof (bool), typeof (MainWindow), new PropertyMetadata(default(bool)));

	    public static readonly DependencyProperty UseStatusBarProperty = DependencyProperty.Register(
	        "UseStatusBar", typeof (bool), typeof (MainWindow), new PropertyMetadata(true));

	    public static readonly DependencyProperty UseTitleBarProperty = DependencyProperty.Register(
	        "UseTitleBar", typeof (bool), typeof (MainWindow), new PropertyMetadata(true));

	    private bool _fullWindowMovement;

	    protected MainWindow()
		{
            DataContext = new MainWindowViewModel(this);
            IsVisibleChanged += OnVisibilityChanged;
			SetBinding(LeftProperty, new Binding
			{
				Path = new PropertyPath("Left"),
				Mode = BindingMode.TwoWay,
				Converter = new DeviceToLogicalXConverter()
			});
			SetBinding(TopProperty, new Binding
			{
				Path = new PropertyPath("Top"),
				Mode = BindingMode.TwoWay,
				Converter = new DeviceToLogicalYConverter()
			});
			SetBinding(WidthProperty, new Binding
			{
				Path = new PropertyPath("Width"),
				Mode = BindingMode.TwoWay,
				Converter = new DeviceToLogicalXConverter()
			});
			SetBinding(HeightProperty, new Binding
			{
				Path = new PropertyPath("Height"),
				Mode = BindingMode.TwoWay,
				Converter = new DeviceToLogicalYConverter()
			});
			UIElementAutomationPeer.CreatePeerForElement(this);

			Application.Current.MainWindow = this;
		}

	    //TODO: Decide if abstract or not later (But Makes Sense...)

	    static MainWindow()
		{
			Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
			RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
			DefaultStyleKeyProperty.OverrideMetadata(typeof (MainWindow),
				new FrameworkPropertyMetadata(typeof (MainWindow)));
			//TODO FloatingWindow event stuff
		}

	    public BitmapImage ActivatedFloatIcon { get; set; }
	    public BitmapImage ActivatedIcon { get; set; }
	    public BitmapImage DeactivatedFloatIcon { get; set; }
	    public BitmapImage DeactivatedIcon { get; set; }
	    public DockingHost DockingHost { get; protected set; }

	    public bool FullWindowMovement
	    {
            get { return _fullWindowMovement; }
	        set
	        {
	            _fullWindowMovement = value;
	            OnFullWindowMovementChanged();
	        }
	    }

	    /// <summary>
        /// Contains the current shown Icon
        /// </summary>
		//public new ImageSource Icon
		//{
		//	get { return (ImageSource) GetValue(IconProperty); }
		//	set { SetValue(IconProperty, value); }
		//}

	    public bool IsSimpleWindowType
	    {
	        get { return (bool) GetValue(IsSimpleWindowTypeProperty); }
	        set { SetValue(IsSimpleWindowTypeProperty, value); }
	    }

	    public Theme Theme
		{
			get { return (Theme) GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}

	    public bool UsesDockingManagerHost { get;  protected set; }

	    public bool UseStatusBar
	    {
	        get { return (bool) GetValue(UseStatusBarProperty); }
	        set { SetValue(UseStatusBarProperty, value); }
	    }

	    public bool UseTitleBar
	    {
	        get { return (bool) GetValue(UseTitleBarProperty); }
	        set { SetValue(UseTitleBarProperty, value); }
	    }

	    protected DockingManager DockingManager { get; set; }
	    internal IntPtr MainWindowHandle => new WindowInteropHelper(this).Handle;

	    public override void OnApplyTemplate()
		{
			SetWindowIcons();
		    var viewModel = DataContext as MainWindowViewModel;

            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

			var minimizeButton = GetTemplateChild("MinimizeButton") as System.Windows.Controls.Button;
		    if (minimizeButton != null)
		        minimizeButton.Command = viewModel.MinimizeCommand;

			var maximizeRestoreButton = GetTemplateChild("MaximizeRestoreButton") as System.Windows.Controls.Button;
			if (maximizeRestoreButton != null)
				maximizeRestoreButton.Command = viewModel.MaximizeResizeCommand;

			var closeButton = GetTemplateChild("CloseButton") as System.Windows.Controls.Button;
			if (closeButton != null)
				closeButton.Command = viewModel.CloseCommand;

			var menuHostControl = GetTemplateChild("MenuHostControl") as MenuHostControl;
		    if (menuHostControl != null)
		    {
		        var dataContext = menuHostControl.DataContext as MenuHostViewModel;
		        viewModel.MenuHostViewModel = dataContext;
		        if (dataContext != null)
                    dataContext.MainWindowViewModel = viewModel;
		    }

			var toolbarHostControl = GetTemplateChild("ToolbarHostControl") as ToolBarHostControl;
		    if (toolbarHostControl != null)
		    {
                var dataContext = toolbarHostControl.DataContext as ToolBarHostViewModel;
		        viewModel.ToolBarHostViewModel = dataContext;
		        if (dataContext != null)
		            dataContext.MainWindowViewModel = viewModel;
		    }

			var statusBar = GetTemplateChild("StatusBar") as StatusBar;
			if (statusBar != null)
				viewModel.StatusBar = statusBar;

			var dockingHost = GetTemplateChild("DockingHost") as DockingHost;
			if (dockingHost != null)
			{
				UsesDockingManagerHost = true;
				DockingHost = dockingHost;
				//TODO Add events		
			}
			base.OnApplyTemplate();
		}

	    protected override void OnActivated(EventArgs e)
		{
			Icon = ActivatedIcon;
			base.OnActivated(e);
		}

	    protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new MainWindowAutomationPeer(this);
		}

	    protected override void OnDeactivated(EventArgs e)
		{
			Icon = DeactivatedIcon;
			base.OnDeactivated(e);
		}

	    protected override void OnSourceInitialized(EventArgs e)
		{
			PopulateMenuAndToolBars();
			base.OnSourceInitialized(e);
		}

	    protected abstract void PopulateMenuAndToolBars();
	    protected abstract void SetWindowIcons();

	    protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			switch (msg)
			{
				case 4242:
					handled = HandleInputProcessing((int) lparam);
					break;
			}
			return base.WindowProc(hwnd, msg, wparam, lparam, ref handled);
		}

	    private static bool HandleInputProcessing(int timeoutMs)
		{
			var num = timeoutMs <= 0 ? 500 : timeoutMs;
			var timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
			{
				Interval = TimeSpan.FromMilliseconds(num)
			};
			timer.Tick += (sender, args) =>
			{
				using (
					var eventWaitHandle = EventWaitHandle.OpenExisting("InputProcessed",
						EventWaitHandleRights.Modify))
					eventWaitHandle.Set();
				timer.Stop();
			};
			timer.Start();
			return true;
		}

	    private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
	        ((MainWindow) d).OnThemeChanged(e);
	    }

	    private void DockingHost_Loaded(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

	    private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

	    private void OnFullWindowMovementChanged()
	    {
	        if (FullWindowMovement)
                MouseDown += MainWindow_MouseDown;
            else
                MouseDown -= MainWindow_MouseDown;
        }

	    private void OnThemeChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldTheme = e.OldValue as Theme;
			var newValue = e.NewValue as Theme;

			var resources = Resources;
			resources.Clear();
			resources.MergedDictionaries.Clear();
			if (oldTheme != null)
			{
				var resourceDictionaryToRemove =
					resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
				if (resourceDictionaryToRemove != null)
					resources.MergedDictionaries.Remove(resourceDictionaryToRemove);
			}
			if (newValue != null)
				resources.MergedDictionaries.Add(new ResourceDictionary {Source = newValue.GetResourceUri()});

			DockingManager?.OnThemeChanged(e);
		}

	    private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var handle = new WindowInteropHelper(this).Handle;
			if (!NativeMethods.IsWindow(handle))
				return;
			NativeMethods.ShowOwnedPopups(handle, IsVisible);
		}

	    private class DeviceToLogicalXConverter : ValueConverter<int, double>
		{
		    protected override double Convert(int value, object parameter, CultureInfo culture)
			{
				return value*DpiHelper.DeviceToLogicalUnitsScalingFactorX;
			}

		    protected override int ConvertBack(double value, object parameter, CultureInfo culture)
			{
				return (int) (value*DpiHelper.LogicalToDeviceUnitsScalingFactorX);
			}
		}

	    private class DeviceToLogicalYConverter : ValueConverter<int, double>
		{
		    protected override double Convert(int value, object parameter, CultureInfo culture)
			{
				return value*DpiHelper.DeviceToLogicalUnitsScalingFactorY;
			}

		    protected override int ConvertBack(double value, object parameter, CultureInfo culture)
			{
				return (int) (value*DpiHelper.LogicalToDeviceUnitsScalingFactorY);
			}
		}
	}
}