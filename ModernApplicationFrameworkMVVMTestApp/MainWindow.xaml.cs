using System.IO;
using System.Threading;
using System.Windows;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Docking.Layout.Serialization;

namespace ModernApplicationFrameworkMVVMTestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
            
			InitializeComponent();

            DataContext = new MainWindowViewModel(this);

            OnThemeChanged += MainWindow_OnThemeChanged;
        }

        private void MainWindow_OnThemeChanged(object sender, ModernApplicationFramework.Core.Events.ThemeChangedEventArgs e)
        {
            if (DockManager?.DockingManager != null && e != null)
                DockManager.DockingManager.Theme = e.NewTheme;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //FullScreen = !FullScreen;

            var wd = new WaitDialog();
            wd.ShowDialog(() =>
            {
                Thread.Sleep(5000);
                MessageBox.Show("Test");
            });
            MessageBox.Show(wd.ActionWasAborted.ToString());


            //if (((ModernApplicationFramework.ViewModels.UseDockingHost)DataContext).Theme is LightTheme)
            //    ((ModernApplicationFramework.ViewModels.UseDockingHost)DataContext).Theme = new GenericTheme();
            //else
            //    ((ModernApplicationFramework.ViewModels.UseDockingHost)DataContext).Theme = new LightTheme();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
	    {
            var serializer = new XmlLayoutSerializer(DockManager.DockingManager);
            serializer.LayoutSerializationCallback += (s, args) =>
            {
                args.Content = args.Content;
            };

            if (File.Exists(@".\AvalonDock.config"))
                serializer.Deserialize(@".\AvalonDock.config");
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
	    {
            var serializer = new XmlLayoutSerializer(DockManager.DockingManager);
            serializer.Serialize(@".\AvalonDock.config");
        }
	}
}
