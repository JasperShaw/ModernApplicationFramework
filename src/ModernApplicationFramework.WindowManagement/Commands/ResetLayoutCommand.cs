using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.WindowManagement.Interfaces.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(IResetLayoutCommand))]
    internal class ResetLayoutCommand : CommandDefinitionCommand, IResetLayoutCommand
    {
        private readonly IExtendedEnvironmentVariables _environmentVariables;

        [ImportingConstructor]
        public ResetLayoutCommand(IExtendedEnvironmentVariables environmentVariables)
        {
            _environmentVariables = environmentVariables;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (LayoutManagementService.Instance == null)
                return false;
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            var result = MessageBox.Show(WindowManagement_Resources.ResetLayoutConfirmation, _environmentVariables.ApplicationName, MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result != MessageBoxResult.Yes)
                return;
            LayoutManagementService.Instance.RestoreProfiles();
        }
    }
}