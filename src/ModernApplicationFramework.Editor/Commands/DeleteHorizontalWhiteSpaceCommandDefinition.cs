using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Input;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class DeleteHorizontalWhiteSpaceCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteHorizontalWhiteSpaceCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteWhiteSpace))
        {
        }

        public override string NameUnlocalized => "DeleteHorizontalWhiteSpace";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{15991222-7881-4BD9-A257-7EDD94DF65ED}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(new List<KeySequence>
            {
                new KeySequence(ModifierKeys.Control, Key.K),
                new KeySequence(ModifierKeys.Control, Key.Oem5)
            }))
        });
    }
}