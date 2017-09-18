using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class FullScreenCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public FullScreenCommandDefinition()
        {
            Command = new GestureCommandWrapper(TriggerFullScreen, CanTriggerFullScreen, new KeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt));
        }

        private bool _isFullScreen;

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get;}

        public override string IconId => "FullScreenIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/FitToScreen_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Fit to Screen";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanTriggerFullScreen()
        {
            return _shell != null;
        }

        private void TriggerFullScreen()
        {
            var menuBuilder = IoC.Get<IMenuCreator>();
            if (!_isFullScreen)
            {

                object vb = null;
                if (!string.IsNullOrEmpty(IconSource?.OriginalString))
                {
                    var myResourceDictionary = new ResourceDictionary { Source = IconSource };
                    vb = myResourceDictionary[IconId];
                }


                var item = new MenuItem
                {
                    Command = Command,
                    Header = "Restore to normal size",
                    Icon = vb
                };

                item.IsChecked = true;

                ((MenuCreator)menuBuilder).CreateMenuBar(_shell.MenuHostViewModel);
            }
            else
                menuBuilder.CreateMenuBar(_shell.MenuHostViewModel);


            ((ModernChromeWindow) Application.Current.MainWindow).FullScreen = !_isFullScreen;
            _isFullScreen = !_isFullScreen;
        }
    }
}