using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ResetLayoutCommandDefinition))]
    public sealed class ResetLayoutCommandDefinition : CommandDefinition<IResetLayoutCommand>
    {
        public override string Name => WindowManagement_Resources.ResetLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ResetLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ResetLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{A2885FF1-870F-41A3-9259-8A3A2D84286E}");

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IResetLayoutCommand : ICommandDefinitionCommand
    {
    }

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
