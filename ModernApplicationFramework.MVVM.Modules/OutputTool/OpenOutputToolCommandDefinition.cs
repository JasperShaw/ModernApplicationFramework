using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Modules.OutputTool
{
    [Export(typeof(CommandDefinition))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public OpenOutputToolCommandDefinition()
        {
            Command = new GestureCommandWrapper(Open, CanOpen, new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt));
        }

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get; }

        public override string IconId => "OutputIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM.Modules;component/Resources/Icons/Output_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Output";
        public override string Text => Name;
        public override string ToolTip => Name;

        public string MyText { get; set; }

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