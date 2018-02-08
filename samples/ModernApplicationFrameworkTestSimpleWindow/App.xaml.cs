using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFrameworkTestSimpleWindow
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var m = new WindowManager();
            m.ShowWindow(IoC.Get<IWindowViewModel>());
        }
    }
}
