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
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Controls.Primitives;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Converters.General;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls
{
    public abstract class MainWindow : ModernChromeWindow
    {
        static MainWindow()
        {
            //Needed so we can use inputbinding in FloatingWindows as well
            Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindow),
                new FrameworkPropertyMetadata(typeof(MainWindow)));
        }

        protected virtual bool ShouldAutoSize { get; set; } = true;

        protected MainWindow()
        {
            DataContext = new MainWindowViewModel(this);
            IsVisibleChanged += OnVisibilityChanged;
            if (ShouldAutoSize)
                GetGoodStartingSize();

            UIElementAutomationPeer.CreatePeerForElement(this);

            if (!(Application.Current.MainWindow is MainWindow))
                Application.Current.MainWindow = this;
        }

        public BitmapImage ActivatedFloatIcon { get; set; }
        public BitmapImage DeactivatedFloatIcon { get; set; }

        protected IMainWindowViewModel ViewModel => (IMainWindowViewModel) DataContext;

        internal IntPtr MainWindowHandle => new WindowInteropHelper(this).Handle;

        public override void OnApplyTemplate()
        {
            var viewModel = ViewModel;
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
                var dataContext = menuHostControl.DataContext as IMenuHostViewModel;
                viewModel.MenuHostViewModel = dataContext;
                if (dataContext != null)
                    dataContext.MainWindowViewModel = viewModel;
            }

            var toolbarHostControl = GetTemplateChild("ToolbarHostControl") as ToolBarHostControl;
            if (toolbarHostControl != null)
            {
                var dataContext = toolbarHostControl.DataContext as IToolBarHostViewModel;
                viewModel.ToolBarHostViewModel = dataContext;
                if (dataContext != null)
                    dataContext.MainWindowViewModel = viewModel;
            }

            var statusBar = GetTemplateChild("StatusBar") as StatusBar;
            if (statusBar != null)
                viewModel.StatusBar = statusBar;

            base.OnApplyTemplate();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //Needed so we can Bind Keygestures to the Window
            Keyboard.Focus(this);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MainWindowAutomationPeer(this);
        }

        protected override IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case 4242:
                    handled = HandleInputProcessing((int) lparam);
                    break;
            }
            return base.HwndSourceHook(hwnd, msg, wparam, lparam, ref handled);
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

        private void GetGoodStartingSize()
        {
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
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            if (!User32.IsWindow(handle))
                return;
            User32.ShowOwnedPopups(handle, IsVisible);
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