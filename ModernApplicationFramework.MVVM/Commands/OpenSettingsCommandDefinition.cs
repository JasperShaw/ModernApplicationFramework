using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.MVVM.ViewModels;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class OpenSettingsCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IWindowManager _windowManager;
#pragma warning restore 649

        public OpenSettingsCommandDefinition()
        {
            Command = new CommandWrapper(OpenSettings, CanOpenSettings);
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "SettingsIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/Settings_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Settings";
        public override string Text => Name;
        public override string ToolTip => Name;

        public string MyText { get; set; }

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