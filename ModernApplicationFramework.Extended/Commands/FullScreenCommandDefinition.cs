using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class FullScreenCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public FullScreenCommandDefinition()
        {
            var command  = new MultiKeyGestureCommandWrapper(TriggerFullScreen, CanTriggerFullScreen, new MultiKeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt));
            Command = command;
            ShortcutText = command.GestureText;
        }

        public override ICommand Command { get;}

        public override string IconId => "FullScreenIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/FitToScreen_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "View.FullScreen";
        public override string Text => "Full Screen";
        public override string ToolTip => Text;

        public override bool IsChecked { get; set; }

        private bool CanTriggerFullScreen()
        {
            return _shell != null;
        }

        private void TriggerFullScreen()
        {
            ((ModernChromeWindow) Application.Current.MainWindow).FullScreen = !((ModernChromeWindow)Application.Current.MainWindow).FullScreen;
        }
    }
}