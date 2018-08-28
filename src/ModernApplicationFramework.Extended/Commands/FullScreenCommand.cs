using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IFullScreenCommand))]
    internal class FullScreenCommand : CommandDefinitionCommand, IFullScreenCommand
    {
        private readonly Lazy<IMenuHostViewModel> _menuHost;

        [ImportingConstructor]
        public FullScreenCommand(Lazy<IMenuHostViewModel> menuHost)
        {
            _menuHost = menuHost;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return Application.Current.MainWindow is ModernChromeWindow;
        }

        protected override void OnExecute(object parameter)
        {
            ((ModernChromeWindow)Application.Current.MainWindow).FullScreen =
                !((ModernChromeWindow)Application.Current.MainWindow).FullScreen;
            _menuHost.Value.Build();
        }
    }
}