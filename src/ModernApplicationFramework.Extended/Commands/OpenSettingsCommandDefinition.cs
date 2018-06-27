using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Settings.SettingsDialog.ViewModels;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(OpenSettingsCommandDefinition))]
    public sealed class OpenSettingsCommandDefinition : CommandDefinition<IOpenSettingsCommand>
    {
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override string IconId => "SettingsIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Settings_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.OpenSettingsCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("OpenSettingsCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{71F57742-F483-4E3C-A5DD-79596A86CEC7}");
    }



    public interface IOpenSettingsCommand : ICommandDefinitionCommand
    {
    }


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