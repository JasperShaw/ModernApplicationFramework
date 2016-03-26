using System.Windows;
using ModernApplicationFramework.Themes;
using ModernApplicationFramework.Themes.LightIDE;

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
            if (((ModernApplicationFramework.ViewModels.MainWindowViewModel)DataContext).Theme is LightTheme)
                ((ModernApplicationFramework.ViewModels.MainWindowViewModel)DataContext).Theme = new GenericTheme();
            else
                ((ModernApplicationFramework.ViewModels.MainWindowViewModel)DataContext).Theme = new LightTheme();
        }
	}
}
