using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(CommandDefinitionBase))]
    public sealed class LessPriorityCommandDefinition : CommandDefinition
    {
        public LessPriorityCommandDefinition()
        {
            Command = new LessPriorityCommand();

            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(Key.C, ModifierKeys.Control)
            };

            DefaultGestureScope = TextEditorScope.LesserPriority;
        }

        public override CommandCategory Category => new CommandCategory("Test");
        public override Guid Id => new Guid("{7008D16C-FEE0-421A-AD09-2F299A33987A}");
        public override string Name => "LesserPriorityCommand";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }
    }

    internal class LessPriorityCommand : CommandDefinitionCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            MessageBox.Show("This Message should never appear!");
        }
    }
}