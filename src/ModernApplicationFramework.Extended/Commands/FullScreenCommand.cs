using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IFullScreenCommand))]
    internal class FullScreenCommand : CommandDefinitionCommand, IFullScreenCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return Application.Current.MainWindow is ModernChromeWindow;
        }

        protected override void OnExecute(object parameter)
        {
            ((ModernChromeWindow)Application.Current.MainWindow).FullScreen =
                !((ModernChromeWindow)Application.Current.MainWindow).FullScreen;
        }
    }
}