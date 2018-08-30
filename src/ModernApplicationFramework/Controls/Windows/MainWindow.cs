using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Focus;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities.Converters;
using ModernApplicationFramework.Utilities.Settings;

namespace ModernApplicationFramework.Controls.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// A main window implementation 
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.Windows.ModernChromeWindow" />
    public abstract class MainWindow : ModernChromeWindow
    {
        static MainWindow()
        {
            //Needed so we can use inputbinding in FloatingWindows as well
            Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindow),
                new FrameworkPropertyMetadata(typeof(MainWindow)));
            ImageThemingUtilities.IsImageThemingEnabled = !GetIsImageThemingSuppressed();
            RuntimeHelpers.RunClassConstructor(typeof(ScrollBarThemingUtilities).TypeHandle);

            InitializeCommandBar();
        }

        private static void InitializeCommandBar()
        {
            CaptureManager.Initialize();
            CommandFocusManager.Initialize();
            HwndSourceTracker.Initialize();
        }

        protected virtual bool ShouldAutoSize { get; set; } = true;

        protected MainWindow()
        {
            IsVisibleChanged += OnVisibilityChanged;
            if (ShouldAutoSize)
                GetGoodStartingSize();

            UIElementAutomationPeer.CreatePeerForElement(this);

            if (!(Application.Current.MainWindow is MainWindow))
                Application.Current.MainWindow = this;

            //DataContext = new MainWindowViewModel(this);
        }

        public BitmapImage ActivatedFloatIcon { get; set; }
        public BitmapImage DeactivatedFloatIcon { get; set; }

        protected IWindowViewModel ViewModel => DataContext as IWindowViewModel;

        internal IntPtr MainWindowHandle => new WindowInteropHelper(this).Handle;

        public override void OnApplyTemplate()
        {
            var viewModel = ViewModel;
            if (viewModel == null)
                return;

            if (GetTemplateChild("MinimizeButton") is Button minimizeButton)
                minimizeButton.Command = viewModel.MinimizeCommand;

            if (GetTemplateChild("MaximizeRestoreButton") is Button maximizeRestoreButton)
                maximizeRestoreButton.Command = viewModel.MaximizeResizeCommand;

            if (GetTemplateChild("CloseButton") is Button closeButton)
                closeButton.Command = viewModel.CloseCommand;

            if (!(viewModel is IMainWindowViewModel mainWindowViewModel))
                return;

            if (GetTemplateChild("MenuHostControl") is MenuHostControl menuHostControl)
            {
                var dataContext = menuHostControl.DataContext as IMenuHostViewModel;
                mainWindowViewModel.MenuHostViewModel = dataContext;
                if (dataContext != null)
                    dataContext.MainWindowViewModel = mainWindowViewModel;
            }

            if (GetTemplateChild("ToolbarHostControl") is ToolBarHostControl toolbarHostControl)
            {
                var dataContext = toolbarHostControl.DataContext as IToolBarHostViewModel;
                mainWindowViewModel.ToolBarHostViewModel = dataContext;
                if (dataContext != null)
                    dataContext.MainWindowViewModel = mainWindowViewModel;

                mainWindowViewModel.InfoBarHost = toolbarHostControl.InfoBarHost;
            }
            base.OnApplyTemplate();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //Needed so we can Bind key gestures to the Window
            Keyboard.Focus(this);
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetWindowIcon();
        }

        private static bool GetIsImageThemingSuppressed()
        {
            try
            {
                var settingsManager = IoC.Get<ISettingsManager>();
                if (settingsManager == null)
                    return false;

                settingsManager.GetOrCreatePropertyValue("Environment/General", "SuppressImageTheming",
                    out bool setting, false, true);
                return setting;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetWindowIcon()
        {
            IconHelper.UseWindowIconAsync(windowIcon => Icon = windowIcon);
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
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

#if DEBUG
            if (System.Windows.Forms.Screen.AllScreens.Length <= 1)
                return;
            var s2 = System.Windows.Forms.Screen.AllScreens[0];
            var workingArea = s2.WorkingArea;
            Top = workingArea.Top + 50;
            Left = workingArea.Left +100;
#else

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
#endif
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