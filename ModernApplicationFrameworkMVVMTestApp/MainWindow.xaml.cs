using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Docking.Layout.Serialization;
using ModernApplicationFramework.Model;

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


		protected override void PopulateMenuAndToolBars()
		{
			var m = new Menu();
			m.Items.Add(new MenuItem { Header = "Test" });
			MenuHostControl.Menu = m;
		}

		protected override void SetWindowIcons()
		{
		}
	}
}
