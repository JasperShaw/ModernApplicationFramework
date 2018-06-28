using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Settings.SettingsDialog.ViewModels;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(IOpenSettingsCommand))]
    internal class OpenSettingsCommand : CommandDefinitionCommand, IOpenSettingsCommand
    {
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public OpenSettingsCommand(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            _windowManager.ShowDialog(IoC.Get<SettingsWindowViewModel>());
        }
    }
}