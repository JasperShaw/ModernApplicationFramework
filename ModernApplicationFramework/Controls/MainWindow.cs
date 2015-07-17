using System;
using System.Globalization;
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
	    public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof (Theme),
			typeof (MainWindow), new FrameworkPropertyMetadata(null));

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

	    // TODO: Decide if abstract or not later (But Makes Sense...)
	    // TODO: Try to move Theme DProptery out of this class some day. Task to do first: think of how to implement the DockingManager and DockingManagerHost correct

	    static MainWindow()
		{
			Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
			RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
			DefaultStyleKeyProperty.OverrideMetadata(typeof (MainWindow),
				new FrameworkPropertyMetadata(typeof (MainWindow)));
			//TODO FloatingWindow event stuff
		}

	    public BitmapImage ActivatedFloatIcon { get; set; }
	    public BitmapImage DeactivatedFloatIcon { get; set; }
	    public DockingHost DockingHost { get; protected set; }
	    public DockingManager DockingManager { get; set; }

	    public Theme Theme
		{
			get { return (Theme) GetValue(ThemeProperty); }
			set { SetValue(ThemeProperty, value); }
		}

	    public bool UsesDockingManagerHost { get;  protected set; }
	    internal IntPtr MainWindowHandle => new WindowInteropHelper(this).Handle;

	    public override void OnApplyTemplate()
		{
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

	    protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MainWindowAutomationPeer(this);
        }

	    protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case 4242:
                    handled = HandleInputProcessing((int)lparam);
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

	    private void DockingHost_Loaded(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
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