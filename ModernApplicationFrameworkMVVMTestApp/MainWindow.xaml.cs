using System.IO;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Commands.Base;
using ModernApplicationFramework.Docking.Layout.Serialization;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

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


		protected override void PopulateMenuAndToolBars()
		{
			var m = new Menu();
			m.Items.Add(new MenuItem { Header = "Test" });
		    ((MainWindowViewModel) DataContext).MenuHostViewModel.Menu = m;
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
        }

		protected override void SetWindowIcons()
		{
		}
	}
}
