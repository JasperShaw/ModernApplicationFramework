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
    public sealed class NewCopyCommandDefinition : CommandDefinition
    {
        public NewCopyCommandDefinition()
        {
            Command = new NewCopyCommand();

            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(Key.C, ModifierKeys.Control)
            };

            DefaultGestureScope = TextEditorScope.TextEditor;
        }

        public override CommandCategory Category => new CommandCategory("Test");
        public override Guid Id => new Guid("{2FBAE249-6E98-463A-AEE8-44B2A106F768}");
        public override string Name => "TestCopyCommand";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }
    }

    internal class NewCopyCommand : CommandDefinitionCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            MessageBox.Show("Overwritten Ctrl+C Executed");
        }
    }
}