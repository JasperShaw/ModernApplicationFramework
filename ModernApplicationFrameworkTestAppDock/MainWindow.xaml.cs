using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Themes.LightIDE;
using ModernApplicationFramework.ViewModels;
using Menu = ModernApplicationFramework.Controls.Menu;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;
using TextBox = ModernApplicationFramework.Controls.TextBox;
using ToolBar = ModernApplicationFramework.Controls.ToolBar;

namespace ModernApplicationFrameworkTestAppDock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			DockingManager = Manager;
			Icon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
        }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			//Application.Current.Resources[ModernApplicationFramework.Core.Themes.EnvironmentColors.MainWindowBackground] = new SolidColorBrush(Colors.Red);
			TextBox.Text = Application.Current.TryFindResource(ModernApplicationFramework.Core.Themes.EnvironmentColors.MainWindowBackground).ToString();
			string s = "";
			foreach (var resource in Application.Current.Resources)
			{
				s += resource.ToString();
			}

		}

		protected override void SetWindowIcons()
		{
            ((MainWindowViewModel)DataContext).ActiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
            ((MainWindowViewModel)DataContext).PassiveIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/test.jpg"));

            ActivatedIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
			DeactivatedIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/test.jpg"));

			DeactivatedFloatIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/Build.png"));
			ActivatedFloatIcon = new BitmapImage(new Uri("pack://application:,,,/ModernApplicationFrameworkTestAppDock;component/test.jpg"));
		}

		protected override void PopulateMenuAndToolBars()
		{
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Test1" }, true, Dock.Top);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing" }, true, Dock.Left);
            ((MainWindowViewModel)DataContext).ToolBarHostViewModel.AddToolBar(new ToolBar { IdentifierName = "Testing2" }, true, Dock.Left);

			var m = new Menu();
			m.Items.Add(new MenuItem { Header = "Test" });
            ((MainWindowViewModel)DataContext).MenuHostViewModel.Menu = m;
        }

		private void DockingManager_OnDocumentClosing(object sender, DocumentClosingEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to close the document?", "AvalonDock Sample", MessageBoxButton.YesNo) == MessageBoxResult.No)
				e.Cancel = true;
		}

		private void LayoutElement_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var activeContent = ((LayoutRoot)sender).ActiveContent;
			if (e.PropertyName == "ActiveContent")
			{
				Debug.WriteLine("ActiveContent-> {0}", activeContent);
			}
		}

		private void LayoutAnchorable_OnHiding(object sender, CancelEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to hide this tool?", "AvalonDock", MessageBoxButton.YesNo) == MessageBoxResult.No)
				e.Cancel = true;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Theme = new LightTheme();
		}
	}
}
