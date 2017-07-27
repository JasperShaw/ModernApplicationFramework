using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(FullScreenCommandDefinition))]
    public sealed class FullScreenCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override CommandGestureCategory DefaultGestureCategory { get; }

        public override string IconId => "FullScreenIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/FitToScreen_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.FullScreenCommandDefinition_Text;
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;

        public override bool IsChecked { get; set; }

        public FullScreenCommandDefinition()
        {
            var command = new UICommand(TriggerFullScreen, CanTriggerFullScreen);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt);
            DefaultGestureCategory = CommandGestureCategories.GlobalGestureCategory;
        }

        private bool CanTriggerFullScreen()
        {
            return Application.Current.MainWindow is ModernChromeWindow;
        }

        private void TriggerFullScreen()
        {
            ((ModernChromeWindow) Application.Current.MainWindow).FullScreen =
                !((ModernChromeWindow) Application.Current.MainWindow).FullScreen;
        }
    }
}