using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(CommandBarItemDefinition))]
    public sealed class SecondScopeCommandDefinition : CommandDefinition
    {
        public SecondScopeCommandDefinition()
        {
            Command = new SecondScopeCommand();
        }

        public override CommandCategory Category => new CommandCategory("Test");
        public override Guid Id => new Guid("{88686CCC-14B2-4D56-A50F-9AEEFD951E75}");
        public override string Name => "LesserPriorityCommand";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorScope.LesserPriority, new MultiKeyGesture(Key.M, ModifierKeys.Control))
        });
    }

    internal class SecondScopeCommand : CommandDefinitionCommand
    {
        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            MessageBox.Show("Hurray! Second priority works");
        }
    }
}