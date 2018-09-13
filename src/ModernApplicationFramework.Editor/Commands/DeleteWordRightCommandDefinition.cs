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
    internal class DeleteWordRightCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteWordRightCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteWordRight))
        {
        }

        public override string NameUnlocalized => "DeleteToEnd";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{E335FDF7-40F2-4B29-BB91-8A572EA46DEB}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(TextEditorGestureScope.TextEditorScope, new MultiKeyGesture(Key.Delete, ModifierKeys.Control))
        });
    }
}