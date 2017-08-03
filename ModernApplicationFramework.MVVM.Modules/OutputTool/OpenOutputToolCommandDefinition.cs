﻿using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class OpenOutputToolCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "OutputIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended.Modules;component/Resources/Icons/Output_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "View.Output";
        public override string Text => "Output";
        public override string ToolTip => "Output";

        public override CommandCategory Category => CommandCategories.ViewCommandCategory;

        public string MyText { get; set; }

        public OpenOutputToolCommandDefinition()
        {
            var command = new UICommand(Open, CanOpen);
            Command = command;

            DefaultKeyGesture = new MultiKeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt);
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