using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Settings.SettingsDialog.ViewModels;

namespace ModernApplicationFramework.Extended.Demo.Modules.CommandOverride
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
            MessageBox.Show("Message first");
            _windowManager.ShowDialog(IoC.Get<SettingsWindowViewModel>());
        }
    }
}
