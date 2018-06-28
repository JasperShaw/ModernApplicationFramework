using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(FullScreenCommandDefinition))]
    public sealed class FullScreenCommandDefinition : CommandDefinition<IFullScreenCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "FullScreenIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/FitToScreen_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("FullScreenCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => Commands_Resources.FullScreenCommandDefinition_Text;
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{9EE995EC-45C6-40B9-A3D6-8A9F486D59C9}");

        public override bool IsChecked { get; set; }

        public FullScreenCommandDefinition()
        {
            DefaultKeyGestures = new[] {new MultiKeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface IFullScreenCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IFullScreenCommand))]
    internal class FullScreenCommand : CommandDefinitionCommand, IFullScreenCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return Application.Current.MainWindow is ModernChromeWindow;
        }

        protected override void OnExecute(object parameter)
        {
            ((ModernChromeWindow)Application.Current.MainWindow).FullScreen =
                !((ModernChromeWindow)Application.Current.MainWindow).FullScreen;
        }
    }
}