using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    [Export(typeof(DefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "OutputIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended.Modules;component/Resources/Icons/Output_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "View.Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public string MyText { get; set; }

        public OpenOutputToolCommandDefinition()
        {
            Command = new GestureCommandWrapper(Open, CanOpen,
                new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt));
        }

        private bool CanOpen()
        {
            return _shell != null;
        }

        private void Open()
        {
            _shell.DockingHost.ShowTool<IOutput>();
        }
    }
}