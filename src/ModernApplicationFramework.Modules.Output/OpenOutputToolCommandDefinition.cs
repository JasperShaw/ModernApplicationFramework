using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Modules.Output
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649
        public override ICommand Command { get; }

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "OutputIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Modules.Output;component/Resources/Output_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "View.Output";
        public override string NameUnlocalized => "Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;
        public override Guid Id => new Guid("{ED3DC8E1-F15B-4DBD-8C8E-272194C0642D}");

        public string MyText { get; set; }

        public OpenOutputToolCommandDefinition()
        {
            var command = new UICommand(Open, CanOpen);
            Command = command;

            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
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