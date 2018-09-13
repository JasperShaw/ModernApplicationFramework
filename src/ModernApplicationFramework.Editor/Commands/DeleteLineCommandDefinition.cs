using System;
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
    internal class DeleteLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteLine))
        {
        }

        public override string NameUnlocalized => "Line Delete";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{1E25D443-0BF0-4415-900C-CAE5711A9AE6}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.L, ModifierKeys.Control | ModifierKeys.Shift))
        });
    }
}