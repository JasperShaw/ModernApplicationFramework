using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.SettingsDialog.ViewModels;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(OpenSettingsCommandDefinition))]
    public sealed class OpenSettingsCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IWindowManager _windowManager;
#pragma warning restore 649
        public override ICommand Command { get; }

        public override string IconId => "SettingsIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/Settings_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => Text;
        public override string Text => Commands_Resources.OpenSettingsCommandDefinition_Text;
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;

        public OpenSettingsCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(OpenSettings, CanOpenSettings);
        }

        private bool CanOpenSettings()
        {
            return true;
        }

        private void OpenSettings()
        {
            _windowManager.ShowDialog(IoC.Get<SettingsWindowViewModel>());
        }
    }
}