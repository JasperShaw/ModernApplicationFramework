using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Commands
{
    [Export(typeof(CommandDefinition))]
    public class TestCommandDefinition : CommandDefinition
    {
        public TestCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(Test, CanTest, new MultiKeyGesture(new[] { Key.K, Key.D }, ModifierKeys.Control));
        }

        private bool CanTest()
        {
            return true;
        }

        private void Test()
        {
            MessageBox.Show("Test");
        }

        public override bool CanShowInMenu => false;
        public override bool CanShowInToolbar => false;
        public override string IconId => null;
        public override Uri IconSource => null;
        public override string Name => "Test";
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ICommand Command { get; }
    }
}
