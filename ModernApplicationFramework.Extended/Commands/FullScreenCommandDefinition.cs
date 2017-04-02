using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    public sealed class FullScreenCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        private readonly MenuItemDefinition _itemDefinition;

        public FullScreenCommandDefinition()
        {
            var command  = new MultiKeyGestureCommandWrapper(TriggerFullScreen, CanTriggerFullScreen, new MultiKeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt));
            Command = command;
            ShortcutText = command.GestureText;
            _itemDefinition = new CommandTopLevelMenuItemDefinition(Text, null, int.MaxValue, this) {IsChecked = true};
        }

        private bool _isFullScreen;

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
            var vm = IoC.Get<IMenuHostViewModel>();
            if (!_isFullScreen)
            {
                vm.MenuItemDefinitions.Add(_itemDefinition);
            }
            else
            {
                vm.MenuItemDefinitions.Remove(_itemDefinition);
            }
            ((ModernChromeWindow) Application.Current.MainWindow).FullScreen = !_isFullScreen;
            _isFullScreen = !_isFullScreen;
        }
    }
}