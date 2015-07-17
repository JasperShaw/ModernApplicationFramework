using System.IO;
using System.Windows;
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
			DockingManager = DockManager;

			Loaded += MainWindow_Loaded;
			Unloaded += MainWindow_Unloaded;

            DataContext = new MainWindowViewModel(this);
        }



		private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			var serializer = new XmlLayoutSerializer(DockManager);
			serializer.Serialize(@".\AvalonDock.config");
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var serializer = new XmlLayoutSerializer(DockManager);
			serializer.LayoutSerializationCallback += (s, args) =>
			{
				args.Content = args.Content;
			};
			if (File.Exists(@".\AvalonDock.config"))
				serializer.Deserialize(@".\AvalonDock.config");
		}
	}
}
