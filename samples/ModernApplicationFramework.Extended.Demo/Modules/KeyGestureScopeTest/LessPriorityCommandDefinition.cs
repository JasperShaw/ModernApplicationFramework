using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(CommandBarItemDefinition))]
    public sealed class LessPriorityCommandDefinition : CommandDefinition
    {
        public LessPriorityCommandDefinition() : base(new LessPriorityCommand())
        {
        }

        public override CommandBarCategory Category => new CommandBarCategory("Test");
        public override Guid Id => new Guid("{7008D16C-FEE0-421A-AD09-2F299A33987A}");
        public override string Name => "LesserPriorityCommand";
        public override string NameUnlocalized => Name;
        public override string Text => Name;
        public override string ToolTip => Name;

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorScope.LesserPriority, new MultiKeyGesture(Key.C, ModifierKeys.Control))
        });
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